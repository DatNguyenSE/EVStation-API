using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Report
{
    // DTO để Staff tạo báo cáo
    public class CreateReportDto
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        public string? Description { get; set; }
    }

    // DTO để Admin đánh giá báo cáo (nút Critical / No)
    public class EvaluateReportDto
    {
        [Required]
        public bool IsCritical { get; set; }
        public DateTime? MaintenanceStartTime { get; set; } // Giờ bắt đầu bảo trì dự kiến
        public DateTime? MaintenanceEndTime { get; set; }   // Giờ kết thúc bảo trì dự kiến
        // (Hai trường này sẽ bắt buộc nếu IsCritical = false)
    }

    // DTO để Admin gán việc cho Technician
    public class AssignTechnicianDto
    {
        [Required]
        public string TechnicianId { get; set; } = null!;
    }

    // DTO để Technician báo cáo đã sửa xong
    public class CompleteFixDto
    {
        [Required]
        public string? FixedNote { get; set; }
    }

    public class ReportSummaryDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime? MaintenanceStartTime { get; set; }
        public DateTime? MaintenanceEndTime { get; set; }

        // Thông tin trụ sạc
        public int PostId { get; set; }
        public string? PostCode { get; set; }
    }
    public class UserSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty; // Giả sử AppUser có trường FullName
        public string Email { get; set; } = string.Empty;
    }

    public class ReportDetailDto
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; }
        public DateTime? MaintenanceStartTime { get; set; }
        public DateTime? MaintenanceEndTime { get; set; }
        public DateTime? FixedAt { get; set; }
        public string? FixedNote { get; set; }

        // Thông tin các bảng liên quan (dưới dạng DTO)
        public required PostSummaryDto Post { get; set; }
        public required UserSummaryDto Staff { get; set; }
        public UserSummaryDto? Technician { get; set; }
    }

    public class PostSummaryDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int StationId { get; set; }
        // Bạn có thể thêm StationName nếu repo có Include Station
    }
}