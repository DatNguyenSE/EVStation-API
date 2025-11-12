using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Receipt
{
    public class ReceiptSummaryDto
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public ReceiptStatus Status { get; set; }
        public string? AppUserId { get; set; }
        public string? AppUserName { get; set; } // Tên người dùng để hiển thị
        public decimal TotalCost { get; set; }
        public string PricingName { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? ConfirmedByStaffName { get; set; } // Tên nhân viên để hiển thị
    }
}