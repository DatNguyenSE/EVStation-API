using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.DTOs.Revenue;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using API.Interfaces.IServices;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _uow;
        public AnalyticsService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<decimal> GetPackageRevenueAsync(DateTime startDate, DateTime endDate)
        {
            endDate = endDate.Date.AddDays(1);
            var soldPackages = await _uow.DriverPackages.GetPackagesSoldAsync(startDate, endDate);

            if (!soldPackages.Any())
            {
                return 0;
            }

            var totalRevenue = soldPackages.Sum(dp => dp.Package.Price);

            return totalRevenue;
        }

        public async Task<IEnumerable<RevenueReportDto>> GetRevenueReportAsync(
    int? stationId,
    DateTime startDate,
    DateTime endDate,
    string granularity)
        {
            var effectiveEndDate = endDate.Date.AddDays(1);

            var receiptsQuery = _uow.Receipts
                .GetAllReceiptsQueryable()
                .Include(r => r.Station)
                .Where(r => r.Status == ReceiptStatus.Paid &&
                            r.CreateAt >= startDate &&
                            r.CreateAt < effectiveEndDate);

            if (stationId.HasValue)
                receiptsQuery = receiptsQuery.Where(r => r.StationId == stationId.Value);

            // ⚡ Thực thi phần EF có thể dịch được (lọc & include)
            var receipts = await receiptsQuery.AsNoTracking().ToListAsync();

            // ⚡ Phần còn lại: xử lý grouping bằng LINQ thuần C#
            var report = receipts
                .GroupBy(r =>
                {
                    string period = granularity.ToLower() switch
                    {
                        "day" => $"{r.CreateAt:yyyy-MM-dd}",
                        "month" => $"{r.CreateAt:yyyy-MM}",
                        _ => $"{r.CreateAt:yyyy}"
                    };
                    return new
                    {
                        Period = period,
                        r.StationId,
                        StationName = r.Station?.Name ?? string.Empty
                    };
                })
                .Select(g => new RevenueReportDto
                {
                    Period = g.Key.Period,
                    StationId = g.Key.StationId,
                    StationName = g.Key.StationName ?? string.Empty,
                    TotalRevenue = g.Sum(r => r.TotalCost)
                })
                .OrderBy(r => r.Period)
                .ThenBy(r => r.StationId)
                .ToList();

            return report;
        }
    }
}