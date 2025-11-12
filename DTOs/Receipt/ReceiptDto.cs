using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Receipt
{
    public class ReceiptDto
    {
        public int Id { get; set; }
        public List<int>? SessionIds { get; set; } 
        public DateTime CreateAt { get; set; }
        public ReceiptStatus Status { get; set; }

        public string? DriverId { get; set; }
        public string? DriverName { get; set; }
        public string? DriverEmail { get; set; }

        public int? PackageId { get; set; }
        public string? PackageName { get; set; }

        public double EnergyConsumed { get; set; }
        public decimal EnergyCost { get; set; }
        public decimal IdleFee { get; set; }
        public decimal OverstayFee { get; set; } // thêm mới
        public decimal DiscountAmount { get; set; }
        public decimal TotalCost { get; set; }

        public DateTime? IdleStartTime { get; set; }
        public DateTime? IdleEndTime { get; set; }

        public string PricingName { get; set; } = string.Empty;
        public decimal PricePerKwhSnapshot { get; set; }

        // Thông tin bổ sung từ session để hiển thị tiện hơn (nếu cần)
        public string? VehiclePlate { get; set; }
        public string? StationName { get; set; }
        public string? PostCode { get; set; }

        // để đề nghị đăng kí tài khoản
        public bool ShouldSuggestRegistration { get; set; }
    }
}