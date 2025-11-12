using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly ILogger<ReservationCleanupService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public ReservationCleanupService(ILogger<ReservationCleanupService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory; // Tiêm factory để tạo scope
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForOverdueReservations();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking for overdue reservations.");
                }

                // Chờ 1 phút trước khi chạy lại
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckForOverdueReservations()
        {
            _logger.LogInformation("Checking for overdue reservations at {time}", DateTime.UtcNow.AddHours(7));

            // BƯỚC 1: TẠO SCOPE MỚI ĐỂ SỬ DỤNG UNIT OF WORK
            using (var scope = _scopeFactory.CreateScope())
            {
                // BƯỚC 2: LẤY MỘT INSTANCE UNIT OF WORK MỚI TỪ SCOPE
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var gracePeriod = AppConstant.ReservationRules.NoShowGracePeriodMinutes;

                // BƯỚC 3: SỬ DỤNG REPOSITORY THÔNG QUA UNIT OF WORK ĐỂ LẤY DỮ LIỆU
                var overdueReservations = await unitOfWork.Reservations.GetOverdueReservationsAsync(gracePeriod);

                if (!overdueReservations.Any())
                {
                    return; // Không có gì để làm
                }

                // BƯỚC 4: THAY ĐỔI DỮ LIỆU TRÊN CÁC ĐỐI TƯỢNG
                // EF Core Change Tracker sẽ tự động theo dõi các thay đổi này
                foreach (var reservation in overdueReservations)
                {
                    _logger.LogWarning($"Reservation ID {reservation.Id} is overdue. Setting status to NoShow.");
                    reservation.Status = ReservationStatus.Expired;
                }

                // BƯỚC 5: GỌI COMPLETE TRÊN UNIT OF WORK ĐỂ LƯU TẤT CẢ THAY ĐỔI
                // Đây là lúc SaveChangesAsync() được gọi, tất cả các cập nhật sẽ được lưu trong một transaction.
                if (await unitOfWork.Complete())
                {
                    _logger.LogInformation($"Successfully updated {overdueReservations.Count()} reservations to NoShow status.");
                }
                else
                {
                    _logger.LogError("Failed to save changes for overdue reservations via UnitOfWork.");
                }

                await CheckAndBanDriversAsync(unitOfWork, userManager, emailService);
            } // Scope và UnitOfWork sẽ tự động được giải phóng ở đây
        }

        private async Task CheckAndBanDriversAsync(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IEmailService emailService)
        {
            const int MaxExpiredReservations = AppConstant.ReservationRules.MaxExpiredReservations;
            const int BanDays = AppConstant.ReservationRules.ExpiredBanDays;

            // 1. Lấy tất cả các Reservation chưa được xử lý kỷ luật
            // Sử dụng Repository Reservations (giả định có thể truy cập tất cả)
            var violationData = await unitOfWork.Reservations.FindAllAsync(
                r => r.Status == ReservationStatus.Expired && r.IsProcessedByDiscipline == false,
                asNoTracking: true);

            if (!violationData.Any()) return;

            // 2. Nhóm các vi phạm theo DriverId
            var driversToBan = violationData
                .GroupBy(r => r.DriverId)
                .Where(g => g.Count() >= MaxExpiredReservations)
                .ToList();

            if (!driversToBan.Any()) return;

            // Lấy tất cả IDs của Reservation cần đánh dấu (để tải lại chỉ một lần)
            var allViolationIdsToProcess = driversToBan.SelectMany(g => g.Select(r => r.Id)).ToList();

            // TẢI LẠI CÁC ENTITIES CẦN THAY ĐỔI TRẠNG THÁI (với Tracking)
            // Dùng FindAllAsync mà KHÔNG dùng asNoTracking: true
            var reservationsToUpdate = (await unitOfWork.Reservations.FindAllAsync(
                r => allViolationIdsToProcess.Contains(r.Id),
                asNoTracking: false)).ToDictionary(r => r.Id);

            // 3. Thực thi Ban
            foreach (var driverGroup in driversToBan)
            {
                var driverId = driverGroup.Key;
                var driver = await userManager.FindByIdAsync(driverId);

                bool shouldMarkAsProcessed = false;

                if (driver != null)
                {
                    if (!await userManager.IsLockedOutAsync(driver))
                    {
                        var banUntil = DateTimeOffset.UtcNow.AddDays(BanDays);
                        var result = await userManager.SetLockoutEndDateAsync(driver, banUntil);
                        if (result.Succeeded)
                        {
                            shouldMarkAsProcessed = true;
                            _logger.LogWarning($"Driver ID {driverId} banned until {banUntil:HH:mm dd/MM/yyyy}. Sending ban email.");
                            
                            // GỌI HÀM GỬI EMAIL THÔNG BÁO BAN
                            try
                            {
                                await emailService.SendAccountBannedEmailAsync(driver.Email, driver.UserName, MaxExpiredReservations, BanDays, banUntil);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, $"Failed to send ban email to driver {driver.Email}.");
                            }
                        }
                    }
                    else // Đã bị ban
                    {
                        shouldMarkAsProcessed = true;
                    }
                }

                // 4. Đánh dấu các vi phạm đã được xử lý trên các đối tượng đã được tải lại
                if (shouldMarkAsProcessed)
                {
                    foreach (var violationDataPoint in driverGroup)
                    {
                        if (reservationsToUpdate.TryGetValue(violationDataPoint.Id, out var reservationToMark))
                        {
                            reservationToMark.IsProcessedByDiscipline = true;
                        }
                    }
                }
                // Nếu Ban thất bại, Log lỗi và tiếp tục.
            }
            // Nếu driver đã bị ban, chỉ cần đánh dấu vi phạm là đã xử lý
            // 5. Lưu các thay đổi (cập nhật IsProcessedByDiscipline = true)
            if (!await unitOfWork.Complete())
            {
                _logger.LogError("Failed to mark discipline violations as processed.");
            }
        }
    }
}