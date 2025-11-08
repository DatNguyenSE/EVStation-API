using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities
{
    public class Report
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public ReportSeverity Severity { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime? MaintenanceStartTime { get; set; }
        public DateTime? MaintenanceEndTime { get; set; }
        public DateTime? FixedAt { get; set; }
        public ReportStatus Status { get; set; }
        public string? FixedNote { get; set; }
        public string? CreateImageUrl { get; set; } 
        public string? CompletedImageUrl { get; set; }

        // FK trỏ đến AppUser (người tạo)
        public string CreatedById { get; set; } = string.Empty;
        [ForeignKey("CreatedById")]
        public AppUser CreatedByStaff { get; set; } = null!;

        // FK trỏ đến AppUser (kỹ thuật viên)
        public string? TechnicianId { get; set; } // Kiểu string (có thể null)
        [ForeignKey("TechnicianId")]
        public AppUser? Technician { get; set; }

        // FK trỏ đến ChargingPost
        public int PostId { get; set; }
        [ForeignKey("PostId")]
        public ChargingPost ChargingPost { get; set; } = null!;
    }
}