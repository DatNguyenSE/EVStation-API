using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Report;
using API.Entities;
using API.Helpers;
using API.Helpers.Enums;
using API.Hubs;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _uow;
        private readonly IHubContext<NotificationHub, INotificationClient> _notificationHubContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IChargingSimulationService _simulationService;
        private readonly IHubContext<ChargingHub> _chargingHubContext;

        public ReportService(
            IUnitOfWork uow,
            IHubContext<NotificationHub,
            INotificationClient> hubContext,
            UserManager<AppUser> userManager,
            IEmailService emailService,
            IChargingSimulationService simulationService,
            IHubContext<ChargingHub> chargingHubContext)
        {
            _uow = uow;
            _notificationHubContext = hubContext;
            _userManager = userManager;
            _emailService = emailService;
            _simulationService = simulationService;
            _chargingHubContext = chargingHubContext;
        }

        // Luồng 1: Staff tạo Report
        public async Task<Report> CreateReportAsync(CreateReportDto dto, string staffId)
        {
            var post = await _uow.ChargingPosts.GetByIdAsync(dto.PostId);
            if (post == null) throw new Exception("Charging post not found");

            var report = new Report
            {
                PostId = dto.PostId,
                Description = dto.Description,
                CreatedById = staffId,
                Status = ReportStatus.New,
                Severity = ReportSeverity.Normal,
                CreateAt = DateTime.UtcNow
            };

            _uow.Reports.Add(report); // Dùng repo
            await _uow.Complete(); // Dùng UoW SaveChanges

            await _notificationHubContext.Clients.Group("Admins").NewReportReceived(
                $"Có báo cáo sự cố mới tại trụ {post.Code}.");

            return report;
        }

        // Luồng 2: Admin đánh giá (Critical / No)
        public async Task<bool> EvaluateReportAsync(int reportId, EvaluateReportDto dto)
        {
            Console.WriteLine("--- !!! ĐANG CHẠY EvaluateReportAsync PHIÊN BẢN MỚI NHẤT !!! ---");
            var report = await _uow.Reports.GetReportWithPostAsync(reportId);
            if (report == null || report.Status != ReportStatus.New)
                throw new Exception("Report not found or already evaluated");

            if (dto.IsCritical)
            {
                // === LUỒNG YES (CRITICAL) ===
                var activeSession = await _uow.ChargingSessions.GetActiveSessionByPostIdAsync(report.PostId);
                var notifiedUserIds = new List<string>();
                // Bắt đầu transaction từ UoW
                await using var transaction = await _uow.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                try
                {
                    // 1. Cập nhật Report
                    report.Severity = ReportSeverity.Critical;
                    report.Status = ReportStatus.Pending;

                    // 2. Cập nhật Trụ sạc -> BẢO TRÌ (Quan trọng)
                    report.ChargingPost.Status = PostStatus.Maintenance;


                    // 3. Hủy các Reservation (đặt chỗ) trong tương lai
                    var upcomingReservations = await _uow.Reservations.GetUpcomingReservationsForPostAsync(report.PostId);
                    // var notifiedUserIds = new List<string>();
                    foreach (var res in upcomingReservations)
                    {
                        res.Status = ReservationStatus.Cancelled;
                        notifiedUserIds.Add(res.Vehicle.OwnerId);
                    }

                    // 4. Lấy thông tin user của phiên sạc đang chạy (nếu có)
                    if (activeSession != null)
                    {
                        // activeSession.Status = SessionStatus.Completed; // Dừng phiên
                        // activeSession.EndTime = DateTime.UtcNow.AddHours(7);
                        // activeSession.StopReason = "Trụ sạc bảo trì khẩn cấp.";
                        if (activeSession.VehicleId != null && activeSession.Vehicle != null)
                        {
                            notifiedUserIds.Add(activeSession.Vehicle.OwnerId); // Thêm user này vào danh sách thông báo
                        }
                    }

                    // 5. LƯU THAY ĐỔI CỦA TRANSACTION NÀY
                    // (Lưu: Report, Trụ sạc, Reservation)
                    await _uow.Complete();
                    await transaction.CommitAsync();

                    // Logic SignalR (GIỮ NGUYÊN)
                    // foreach (var userId in notifiedUserIds.Distinct())
                    // {
                    //     // 1. GỬI SIGNALR
                    //     var message = $"Lượt đặt/phiên sạc của bạn tại trụ {report.ChargingPost.Code} đã bị hủy/dừng do bảo trì khẩn cấp.";
                    //     await _notificationHubContext.Clients.User(userId).ReservationCancelled(message);

                    //     // 2. GỬI EMAIL
                    //     var user = await _userManager.FindByIdAsync(userId);
                    //     if (user != null && !string.IsNullOrEmpty(user.Email))
                    //     {
                    //         var subject = "Thông báo khẩn: Hủy lượt sạc/đặt chỗ EV Station";
                    //         var body = $@"
                    //         <p>Xin chào {user.FullName},</p>
                    //         <p>{message}</p>
                    //         <p>Chúng tôi thành thật xin lỗi vì sự bất tiện này. Vui lòng kiểm tra ứng dụng để đặt lại lịch hoặc tìm trụ sạc khác.</p>
                    //         <p>Trân trọng,</p>
                    //         <p>Đội ngũ EV Station.</p>";

                    //         // Dùng kỹ thuật "Fire-and-Forget" để không làm chậm API
                    //         _ = _emailService.SendEmailAsync(user.Email, subject, body);
                    //     }
                    // }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception($"Lỗi Transaction: {ex.Message}");
                }

                // --- CÁC HÀNH ĐỘNG SAU TRANSACTION ---
                // (Chỉ chạy khi transaction đã thành công)

                try
                {
                    // 6. DỪNG SIMULATION (Hàm này sẽ tự flush data xuống DB)
                    if (activeSession != null)
                    {
                        // Hàm này sẽ tự set Status=Completed, EndTime, Cost, Energy...
                        await _simulationService.StopSimulationAsync(activeSession.Id, setCompleted: true);

                        // Gửi thông báo "ReceiveSessionEnded" cho UI sạc (lấy từ EndSessionAsync)
                        await _chargingHubContext.Clients.Group($"session-{activeSession.Id}")
                            .SendAsync("ReceiveSessionEnded", activeSession.Id, SessionStatus.Completed);
                    }

                    // 7. GỬI THÔNG BÁO CHUNG & EMAIL
                    foreach (var userId in notifiedUserIds.Distinct())
                    {
                        var message = $"Lượt đặt/phiên sạc của bạn tại trụ {report.ChargingPost.Code} đã bị hủy/dừng do bảo trì khẩn cấp.";

                        // 1. GỬI SIGNALR (qua NotificationHub)
                        await _notificationHubContext.Clients.User(userId).ReservationCancelled(message);

                        // 2. GỬI EMAIL (Fire-and-Forget)
                        var user = await _userManager.FindByIdAsync(userId);
                        if (user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            var subject = "Thông báo khẩn: Hủy lượt sạc/đặt chỗ EV Station";
                            var body = $@"<p>Xin chào {user.FullName},</p><p>{message}</p><p>Chúng tôi thành thật xin lỗi vì sự bất tiện này.</p>";
                            _ = _emailService.SendEmailAsync(user.Email, subject, body);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ReportService] Lỗi khi gửi thông báo: {ex.Message}");
                }
            }
            else
            {
                // === LUỒNG NO (KHÔNG KHẨN CẤP) ===
                if (dto.ScheduledTime == null)
                    throw new Exception("Scheduled time is required for non-critical reports");

                report.Severity = ReportSeverity.Normal;
                report.Status = ReportStatus.Pending;
                report.ScheduledTime = dto.ScheduledTime;

                // === CHANGED ===
                await _uow.Complete(); // Lưu thay đổi
            }
            return true;
        }

        // Luồng 3: Admin gán việc cho Technician
        public async Task<bool> AssignTechnicianAsync(int reportId, AssignTechnicianDto dto)
        {
            // === CHANGED ===
            var report = await _uow.Reports.GetByIdAsync(reportId);
            if (report == null || report.Status != ReportStatus.Pending)
                throw new Exception("Report not found or invalid status");

            var techUser = await _userManager.FindByIdAsync(dto.TechnicianId);
            if (techUser == null || !await _userManager.IsInRoleAsync(techUser, AppConstant.Roles.Technician))
                throw new Exception("Invalid technician");

            report.TechnicianId = dto.TechnicianId;
            report.Status = ReportStatus.InProgress;

            // === CHANGED ===
            await _uow.Complete();

            // Logic SignalR 
            await _notificationHubContext.Clients.User(dto.TechnicianId).NewTaskAssigned(
                $"Bạn có một công việc sửa chữa mới tại trụ {report.PostId}");

            return true;
        }

        // Luồng 4: Technician báo cáo đã sửa xong
        public async Task<bool> CompleteFixAsync(int reportId, CompleteFixDto dto, string technicianId)
        {
            // === CHANGED ===
            var report = await _uow.Reports.GetByIdAsync(reportId);

            if (report == null || report.TechnicianId != technicianId)
                throw new Exception("Unauthorized or report not found");
            if (report.Status != ReportStatus.InProgress)
                throw new Exception("Invalid report status");

            report.Status = ReportStatus.Resolved;
            report.FixedNote = dto.FixedNote;
            report.FixedAt = DateTime.UtcNow;

            // === CHANGED ===
            await _uow.Complete();

            // ... SignalR logic (thông báo cho Admin) ...
            await _notificationHubContext.Clients.Group("Admins").FixCompleted(
                $"Sự cố tại trụ {report.PostId} đã được KTV xử lý. Chờ bạn xác nhận.");

            return true;
        }

        // Luồng 5: Admin xác nhận và kích hoạt lại trụ
        public async Task<bool> CloseReportAsync(int reportId)
        {
            // === CHANGED ===
            var report = await _uow.Reports.GetReportWithPostAsync(reportId);

            if (report == null || report.Status != ReportStatus.Resolved)
                throw new Exception("Report not found or not in resolved status");

            report.Status = ReportStatus.Closed;
            report.ChargingPost.Status = PostStatus.Available;

            // === CHANGED ===
            await _uow.Complete();

            // 1. Thông báo cho Admin (để UI cập nhật)
            await _notificationHubContext.Clients.Group("Admins").ReportClosed(
                $"Sự cố tại trụ {report.ChargingPost.Code} đã được đóng thành công.");

            // 2. Thông báo cho Technician đã sửa
            if (!string.IsNullOrEmpty(report.TechnicianId))
            {
                await _notificationHubContext.Clients.User(report.TechnicianId).TaskCompleted(
                    $"Công việc của bạn tại trụ {report.ChargingPost.Code} đã được Admin xác nhận.");
            }

            // ... SignalR logic (thông báo cho Drivers) ...
            return true;
        }

        // Luồng 6: Lấy chi tiết Report
        public async Task<ReportDetailDto?> GetReportDetailsAsync(int id)
        {
            var report = await _uow.Reports.GetReportDetailsAsync(id);
            if (report == null) return null;

            return new ReportDetailDto
            {
                Id = report.Id,
                Description = report.Description,
                Status = report.Status.ToString(), // Chuyển Enum thành string
                Severity = report.Severity.ToString(),
                CreateAt = report.CreateAt,
                ScheduledTime = report.ScheduledTime,
                FixedAt = report.FixedAt,
                FixedNote = report.FixedNote,

                // Map đối tượng Post
                Post = new PostSummaryDto
                {
                    Id = report.ChargingPost.Id,
                    Code = report.ChargingPost.Code,
                    Status = report.ChargingPost.Status.ToString(),
                    StationId = report.ChargingPost.StationId
                },

                // Map đối tượng Staff
                Staff = new UserSummaryDto
                {
                    Id = report.CreatedByStaff.Id,
                    FullName = report.CreatedByStaff.FullName, // Đảm bảo bạn có trường FullName
                    Email = report.CreatedByStaff.Email!
                },

                // Map Technician (có thể null)
                Technician = report.Technician == null ? null : new UserSummaryDto
                {
                    Id = report.Technician.Id,
                    FullName = report.Technician.FullName,
                    Email = report.Technician.Email!
                }
            };
        }

        // Luồng 7: (Admin) Lấy các report mới
        public async Task<IEnumerable<ReportSummaryDto>> GetNewReportsAsync()
        {
            var reports = await _uow.Reports.GetNewReportsAsync();

            // Dùng LINQ .Select để map cả danh sách
            return reports.Select(report => new ReportSummaryDto
            {
                Id = report.Id,
                Description = report.Description,
                Status = report.Status.ToString(),
                Severity = report.Severity.ToString(),
                CreateAt = report.CreateAt,
                ScheduledTime = report.ScheduledTime,
                PostId = report.ChargingPost.Id,
                PostCode = report.ChargingPost.Code
            });
        }

        // Luồng 8: (Technician) Lấy các task của tôi
        public async Task<IEnumerable<ReportSummaryDto>> GetMyTasksAsync(string technicianId)
        {
            var reports = await _uow.Reports.GetMyTasksAsync(technicianId);
            return reports.Select(report => new ReportSummaryDto
            {
                Id = report.Id,
                Description = report.Description,
                Status = report.Status.ToString(),
                Severity = report.Severity.ToString(),
                CreateAt = report.CreateAt,
                ScheduledTime = report.ScheduledTime,
                PostId = report.ChargingPost.Id,
                PostCode = report.ChargingPost.Code
            });
        }
    }
}