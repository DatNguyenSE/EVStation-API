using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Assignment
    {
        public int Id { get; set; }

        // Ngày bắt đầu phân công (Hợp đồng)
        public DateTime EffectiveFrom { get; set; }

        // Ngày kết thúc phân công (Có thể null)
        public DateTime? EffectiveTo { get; set; } // Dùng DateTime? để cho phép null

        // Trạng thái (để dễ dàng vô hiệu hóa mà không cần xóa)
        public bool IsActive { get; set; } = true;

        // FK trỏ đến AppUser (nhân viên)
        public string StaffId { get; set; } = string.Empty;
        [ForeignKey("StaffId")]
        public AppUser Staff { get; set; } = null!;

        // FK trỏ đến Station
        public int StationId { get; set; }
        public Station Station { get; set; } = null!;
    }
}