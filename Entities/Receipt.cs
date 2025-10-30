using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Entities.Wallet;
using API.Helpers.Enums;

namespace API.Entities
{
    public class Receipt
    {
        [Key]
        public int Id { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [MaxLength(15)]
        public ReceiptStatus Status { get; set; } = ReceiptStatus.Pending;

        public string? AppUserId { get; set; }
        public AppUser? AppUser { get; set; }
        public int? PackageId { get; set; }
        public DriverPackage? Package { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal EnergyConsumed { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal EnergyCost { get; set; }

        public DateTime? IdleStartTime { get; set; }
        public DateTime? IdleEndTime { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal IdleFee { get; set; }
        public decimal OverstayFee { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCost { get; set; }

        [Required]
        [MaxLength(100)]
        public string PricingName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerKwhSnapshot { get; set; }

        public ICollection<WalletTransaction> WalletTransactions { get; set; } = new List<WalletTransaction>();

        // 1 Receipt -> N ChargingSessions
        public ICollection<ChargingSession> ChargingSessions { get; set; } = new List<ChargingSession>();
    }
}
