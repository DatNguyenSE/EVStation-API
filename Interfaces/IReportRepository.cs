using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

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
    }
}