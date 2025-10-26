using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

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
    }
}