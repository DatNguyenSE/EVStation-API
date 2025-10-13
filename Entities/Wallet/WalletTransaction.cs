using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities.Wallet
{
    public class WalletTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int WalletId { get; set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; set; }

        [Required]
        [MaxLength(50)]
        public TransactionType TransactionType { get; set; }          // "Topup", "BuyPackage", "PayCharging", "Refund"...

        // ID tham chiếu (VD: Id của Order hoặc Package)
        public int? ReferenceId { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal BalanceBefore { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal BalanceAfter { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0.")]
        public decimal Amount { get; set; }

        [MaxLength(255)]
        public string? Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending; // Pending, Success, Failed, Canceled

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [MaxLength(50)]
        public string? VnpTxnRef { get; set; }
    }
}