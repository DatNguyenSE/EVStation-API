using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Revenue
{
    public class RevenueReportDto
    {
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;

        // Period: Chu kỳ báo cáo (ví dụ: "2024-05-15" cho ngày, "2024-05" cho tháng, "2024" cho năm)
        public string Period { get; set; } = string.Empty;
        // TotalRevenue: Tổng doanh thu trong chu kỳ đó
        public decimal TotalRevenue { get; set; }
    }
}