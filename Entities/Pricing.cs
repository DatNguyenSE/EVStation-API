using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Pricing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public PriceType PriceType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PricePerKwh { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? PricePerMinute { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; }

        [Required]
        public DateTime EffectiveTo { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
    public enum PriceType
    {
        // 1. Khách vãng lai
        Guest_AC,       // Sạc thường AC
        Guest_DC,       // Sạc nhanh DC

        // 2. Khách đặt chỗ (Trả qua ví)
        Member_AC,      // Sạc thường AC (đặt chỗ)
        Member_DC,      // Sạc nhanh DC (đặt chỗ)

        // 3. Phí chiếm dụng
        // Đây là một loại phí đặc biệt, tính theo phút
        // thay vì kWh như các loại trên.
        OccupancyFee    // Phí chiếm dụng (tính theo phút)
    }
}