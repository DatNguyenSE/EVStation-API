using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Report;
using API.Entities;
using X.PagedList;

namespace API.Interfaces
{
    public interface IReportService
    {
        Task<Report> CreateReportAsync(CreateReportDto dto, string staffId);
        Task<bool> EvaluateReportAsync(int reportId, EvaluateReportDto dto);
        Task<bool> AssignTechnicianAsync(int reportId, AssignTechnicianDto dto);
        Task<bool> CompleteFixAsync(int reportId, CompleteFixDto dto, string technicianId);
        Task<bool> CloseReportAsync(int reportId);
        Task<ReportDetailDto?> GetReportDetailsAsync(int id);
        Task<IEnumerable<ReportSummaryDto>> GetNewReportsAsync();
        Task<IEnumerable<ReportSummaryDto>> GetMyTasksAsync(string technicianId);
        Task<bool> StartRepairAsync(int reportId, string technicianId);
        /// <summary>
        /// (Admin) Lấy danh sách tất cả report, hỗ trợ lọc và phân trang.
        /// </summary>
        Task<IPagedList<ReportSummaryDto>> GetAllReportsAsync(ReportFilterParams filterParams);
        
        /// <summary>
        /// Lấy lịch sử bảo trì/sự cố của một trụ sạc cụ thể.
        /// </summary>
        Task<IEnumerable<ReportSummaryDto>> GetReportHistoryForPostAsync(int postId);
    }
}