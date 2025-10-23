using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VehicleId { get; set; }
        [Required]
        public int ChargingPostId { get; set; }
        [Required]
        public string? DriverId { get; set; }
        [Required]
        public DateTime TimeSlotStart { get; set; }
        [Required]
        public DateTime TimeSlotEnd { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public ReservationStatus Status { get; set; } = ReservationStatus.Confirmed;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [ForeignKey(nameof(VehicleId))]
        public Vehicle Vehicle { get; set; } = null!;
        [ForeignKey(nameof(ChargingPostId))]
        public ChargingPost Post { get; set; } = null!;
    }

    public enum ReservationStatus
    {
        Confirmed, // Đã xác nhận và đang chờ đến giờ sạc
        Cancelled, // Đã hủy (trước hoặc sau khi hết giờ)
        Completed, // Đã hoàn thành phiên sạc
        Expired    // Đã quá giờ bắt đầu mà xe không đến
    }
}