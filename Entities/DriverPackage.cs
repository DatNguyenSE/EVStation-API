using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities
{
    public class DriverPackage
    {
        [Key]
        public int Id { get; set; }

        // Khóa ngoại tới người dùng
        [ForeignKey("AppUser")]
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;

        // Khóa ngoại tới gói
        [ForeignKey("Package")]
        public int PackageId { get; set; }
        public ChargingPackage Package { get; set; } = null!;

        // Ngày bắt đầu hiệu lực
        public DateTime StartDate { get; set; } = DateTime.UtcNow.AddHours(7);

        // Ngày hết hạn
        public DateTime EndDate { get; set; }

        // Gói này hiện còn hiệu lực hay không
        public bool IsActive { get; set; } = true;

        // Dành cho loại xe nào (Car/Bike) – giúp check nhanh
        public VehicleType VehicleType { get; set; }
    }
}