using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Revenue;

namespace API.Interfaces.IServices
{
    public interface IAnalyticsService
    {
        Task<IEnumerable<RevenueReportDto>> GetRevenueReportAsync(
            int? stationId, 
            DateTime startDate, 
            DateTime endDate, 
            string granularity); // Độ chi tiết tổng hợp: 'Day', 'Month', hoặc 'Year'
    }
}