using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Revenue;
using API.Helpers;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/revenue")]
    [ApiController]
    [Authorize(Roles = $"{AppConstant.Roles.Manager}, {AppConstant.Roles.Admin}")]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;
        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RevenueReportDto>>> GetRevenueReport(
            [FromQuery] int? stationId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] string granularity = "Month")
        {
            if (startDate == default || endDate == default)
            {
                return BadRequest("Phải cung cấp ngày bắt đầu và ngày kết thúc.");
            }

            // Kiểm tra tính hợp lệ của tham số granularity
            var validGranularities = new[] { "day", "month", "year" };
            if (!validGranularities.Contains(granularity.ToLower()))
            {
                return BadRequest("Độ chi tiết (granularity) phải là 'Day', 'Month', hoặc 'Year'.");
            }
            
            // Gọi Service và chờ kết quả
            var report = await _analyticsService.GetRevenueReportAsync(stationId, startDate, endDate, granularity);
            
            if (report == null || !report.Any())
            {
                // Trả về 204 No Content nếu không có dữ liệu phù hợp
                return NoContent(); 
            }

            return Ok(report);
        }
    }
}