using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.DTOs.Wallet;
using API.Helpers.Enums;

namespace API.DTOs.Receipt
{
    public class ReceiptDetailsDto
    {
        public int Id { get; set; }
        public DateTime CreateAt { get; set; }
        public ReceiptStatus Status { get; set; }

        // Thông tin người dùng (nếu có)
        public string? AppUserId { get; set; }
        public string? AppUserName { get; set; } // Tên người dùng để hiển thị

        // Thông tin gói cước (nếu có)
        public int? PackageId { get; set; }
        public string? PackageName { get; set; } // Tên gói cước để hiển thị

        public decimal EnergyConsumed { get; set; }
        public decimal EnergyCost { get; set; }

        public DateTime? IdleStartTime { get; set; }
        public DateTime? IdleEndTime { get; set; }
        public decimal IdleFee { get; set; }
        public decimal OverstayFee { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal TotalCost { get; set; }

        public string PricingName { get; set; } = string.Empty;
        public decimal PricePerKwhSnapshot { get; set; }

        public string? PaymentMethod { get; set; }

        // Thông tin xác nhận bởi nhân viên (nếu có)
        public string? ConfirmedByStaffId { get; set; }
        public string? ConfirmedByStaffName { get; set; } // Tên nhân viên để hiển thị
        public DateTime? ConfirmedAt { get; set; }

        // Danh sách các phiên sạc liên quan (tóm tắt)
        public ICollection<ChargingSessionSummaryDto> ChargingSessions { get; set; } = new List<ChargingSessionSummaryDto>();

        // Danh sách các giao dịch ví liên quan (tóm tắt)
        public ICollection<WalletTransactionSummaryDto> WalletTransactions { get; set; } = new List<WalletTransactionSummaryDto>();
    }
}