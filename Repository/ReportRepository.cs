using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Report;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.EF;

namespace API.Repository
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;
        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }
        public void Add(Report report)
        {
            _context.Reports.Add(report);
        }

        public async Task<IPagedList<Report>> GetAllReportsAsync(ReportFilterParams filterParams)
        {
            var query = _context.Reports
                .Include(r => r.ChargingPost)
                .Include(r => r.Technician)
                .Include(r => r.CreatedByStaff)
                .AsQueryable();

            // --- Áp dụng Filter (giữ nguyên logic) ---
            if (!string.IsNullOrEmpty(filterParams.PostCode))
            {
                query = query.Where(r => r.ChargingPost.Code.Contains(filterParams.PostCode));
            }
            if (filterParams.Status.HasValue)
            {
                query = query.Where(r => r.Status == filterParams.Status.Value);
            }
            if (filterParams.Severity.HasValue)
            {
                query = query.Where(r => r.Severity == filterParams.Severity.Value);
            }
            if (!string.IsNullOrEmpty(filterParams.TechnicianId))
            {
                query = query.Where(r => r.TechnicianId == filterParams.TechnicianId);
            }
            if (filterParams.FromDate.HasValue)
            {
                query = query.Where(r => r.CreateAt >= filterParams.FromDate.Value);
            }
            if (filterParams.ToDate.HasValue)
            {
                query = query.Where(r => r.CreateAt <= filterParams.ToDate.Value.AddDays(1));
            }

            // --- Sắp xếp ---
            query = query.OrderByDescending(r => r.CreateAt);

            // --- THỰC THI QUERY VÀ PHÂN TRANG ---
            return await query.ToPagedListAsync(
                filterParams.PageNumber,
                filterParams.PageSize
            );
        }

        public async Task<IEnumerable<Report>> GetReportHistoryByPostIdAsync(int postId)
        {
            return await _context.Reports
                .Where(r => r.PostId == postId)
                .Include(r => r.ChargingPost) // Include để lấy PostCode
                .Include(r => r.Technician)   // Include để lấy TechnicianName
                                            // Lịch sử là các report đã đóng hoặc đã hủy
                .Where(r => r.Status == ReportStatus.Closed || r.Status == ReportStatus.Cancelled)
                .OrderByDescending(r => r.CreateAt)
                .ToListAsync();
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports.FindAsync(id);
        }

        public async Task<IEnumerable<Report>> GetMyTasksAsync(string technicianId)
        {
            // Lấy các report được gán cho KTV này và đang ở trạng thái InProgress
            return await _context.Reports
                .Where(r => r.TechnicianId == technicianId && r.Status == Helpers.Enums.ReportStatus.InProgress)
                .Include(r => r.ChargingPost)
                .OrderBy(r => r.MaintenanceStartTime) // Ưu tiên task có lịch hẹn sớm hơn
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetNewReportsAsync()
        {
            // Lấy các report có Status = New, kèm thông tin trụ
            return await _context.Reports
                .Where(r => r.Status == Helpers.Enums.ReportStatus.New)
                .Include(r => r.ChargingPost)
                .OrderByDescending(r => r.CreateAt) // Mới nhất lên đầu
                .ToListAsync();
        }

        public async Task<Report?> GetReportDetailsAsync(int id)
        {
            // Lấy report kèm theo thông tin trụ, người tạo (Staff) và KTV (Technician)
            return await _context.Reports
                .Include(r => r.ChargingPost)
                .Include(r => r.CreatedByStaff) // Giả sử navigation property tên là CreatedByStaff
                .Include(r => r.Technician)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Hàm này sẽ Eager-Loading (dùng Include) để lấy luôn ChargingPost
        public async Task<Report?> GetReportWithPostAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.ChargingPost)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Hàm kiểm tra lịch bảo trì
        public async Task<bool> IsPostScheduledForMaintenanceAsync(int postId, DateTime requestedStart, DateTime requestedEnd)
        {
            // Tìm xem có Report nào đang chờ xử lý (Pending) hoặc đang xử lý (InProgress)
            return await _context.Reports
                .AnyAsync(r => r.PostId == postId &&
                               (r.Status == ReportStatus.Pending || r.Status == ReportStatus.InProgress) &&
                               r.MaintenanceStartTime != null &&
                               r.MaintenanceEndTime != null &&
                               // Logic kiểm tra trùng lặp: (StartA < EndB) and (EndA > StartB)
                               requestedStart < r.MaintenanceEndTime.Value &&
                               requestedEnd > r.MaintenanceStartTime.Value);
        }

        public void Update(Report report)
        {
            _context.Entry(report).State = EntityState.Modified;
        }
    }
}