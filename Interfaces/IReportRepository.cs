using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Report;
using API.Entities;
using X.PagedList;

namespace API.Interfaces
{
    public interface IReportRepository
    {
        void Add(Report report);
        Task<Report?> GetByIdAsync(int id);
        // Phương thức này thay thế cho .Include() trong service
        Task<Report?> GetReportWithPostAsync(int id);
        // Hàm lấy chi tiết 
        Task<Report?> GetReportDetailsAsync(int id);

        // Hàm lấy report mới cho Admin 
        Task<IEnumerable<Report>> GetNewReportsAsync();

        // Hàm lấy task cho Technician 
        Task<IEnumerable<Report>> GetMyTasksAsync(string technicianId);
        Task<bool> IsPostScheduledForMaintenanceAsync(int postId, DateTime requestedStart, DateTime requestedEnd);

        /// <summary>
        /// Lấy tất cả report (có phân trang và lọc) cho màn hình quản lý của Admin.
        /// Hàm này sẽ thay thế cho nhiều hàm Get nhỏ lẻ.
        /// </summary>
        Task<IPagedList<Report>> GetAllReportsAsync(ReportFilterParams filterParams);

        /// <summary>
        /// Lấy lịch sử bảo trì/sự cố của một trụ sạc cụ thể.
        /// </summary>
        Task<IEnumerable<Report>> GetReportHistoryByPostIdAsync(int postId);
        void Update(Report report);
    }
}