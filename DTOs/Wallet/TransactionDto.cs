using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Wallet
{
    public class TransactionDto
    {
        public TransactionType TransactionType { get; set; }          // "Topup", "BuyPackage", "PayCharging", "Refund"...
        public decimal BalanceBefore { get; set; }
        public decimal BalanceAfter { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public TransactionStatus Status { get; set; } = TransactionStatus.Pending; // Pending, Success, Failed, Canceled
    }
}