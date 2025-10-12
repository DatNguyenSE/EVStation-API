using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Wallet
{
    public class CreateWalletTransactionDto
    {
        [Required]
        public int WalletId { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; } // "Topup", "BuyPackage", "PayCharging", ...

        public int? ReferenceId { get; set; } // ID liên kết (nếu có, ví dụ gói cước hoặc hóa đơn)

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0.")]
        public decimal Amount { get; set; }

        [MaxLength(255, ErrorMessage = "Mô tả không được vượt quá 255 ký tự.")]
        public string? Description { get; set; }

        [MaxLength(50, ErrorMessage = "Phương thức thanh toán không được vượt quá 50 ký tự.")]
        public string? PaymentMethod { get; set; } // VNPAY, WALLET, SYSTEM, v.v.

        [MaxLength(50, ErrorMessage = "Mã giao dịch VNPAY không được vượt quá 50 ký tự.")]
        public string? VnpTxnRef { get; set; }
    }
}