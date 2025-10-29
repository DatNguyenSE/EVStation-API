using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities
{
    public class ChargingSession
    {
        [Key]
        public int Id { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public int ChargingPostId { get; set; }
        public ChargingPost ChargingPost { get; set; } = null!;

        public int? ReservationId { get; set; } // nếu có đặt chỗ
        public Reservation? Reservation { get; set; }

        public int? ReceiptId { get; set; }     // nullable nếu session có thể chưa có receipt
        public Receipt? Receipt { get; set; }   // navigation

        public string VehiclePlate { get; set; } = string.Empty;

        public DateTime StartTime { get; set; } // Thời điểm bắt đầu cấp nguồn
        public DateTime? EndTime { get; set; } // Thời điểm dừng cấp nguồn (chuyển sang Idle)

        public DateTime? CompletedTime { get; set; } // Thời điểm người dùng Hoàn tất (rời trụ)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal StartBatteryPercentage { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal? EndBatteryPercentage { get; set; }
        public double EnergyConsumed { get; set; }

        public int Cost { get; set; }

        [Column(TypeName = "nvarchar(30)")]
        public SessionStatus Status { get; set; }

        public DateTime? IdleFeeStartTime { get; set; } // thời điểm bắt đầu tính phạt
        public int IdleFee { get; set; } // tổng phí phạt hiện tại

        public StopReason? StopReason { get; set; } // Lý do dừng

        public bool IsWalkInSession { get; set; } = false;

        public bool IsPaid { get; set; } = false;

        // Nếu overstay (chiếm slot người khác) - chỉ áp dụng cho trụ thường
        public bool IsOverstay { get; set; } = false;
        public int? OverstayFee { get; set; }

        [NotMapped]
        public int TotalCost => Cost + IdleFee + (OverstayFee ?? 0);
    }
}