using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Report;
using API.Entities;

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
    }
}