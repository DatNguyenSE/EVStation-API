using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Receipt
{
    public class ReceiptFilterParams
    {
        public ReceiptStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsWalkInOnly { get; set; } // Lọc chỉ hóa đơn của khách vãng lai (AppUserId == null)
        // Bạn có thể thêm các trường lọc khác như StationId, StaffId, v.v.
        public string? AppUserName { get; set; } // Tìm kiếm theo tên người dùng, ID hóa đơn, v.v.
    }
}