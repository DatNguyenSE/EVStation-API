using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.Wallet;

namespace API.Entities
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }
        public int SessionId { get; set; }
        public ChargingSession ChargingSession { get; set; } = null!;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [MaxLength(15)]
        public string Status { get; set; } = "Pending";// Trạng thái thanh toán: Paid, Pending, Refunded, etc.

        [Required]
        public string? DriverId { get; set; }
        public AppUser? AppUser { get; set; }
        public int? PackageId { get; set; } // FK DriverPackageId
        public DriverPackage? Package { get; set; }

        public double EnergyConsumed { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal EnergyCost { get; set; }

        public DateTime? IdleStartTime { get; set; }
        public DateTime? IdleEndTime { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal IdleFee { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCost { get; set; }

        // Rất quan trọng, để đảm bảo hóa đơn không bị thay đổi khi bảng Pricing cập nhật giá mới
        [Required]
        [MaxLength(100)]
        public string PricingName { get; set; } = string.Empty; // Tên của Bảng giá (ví dụ: "Giờ cao điểm")

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerKwhSnapshot { get; set; } // Giá điện tại thời điểm đó

        public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();
    }
}
