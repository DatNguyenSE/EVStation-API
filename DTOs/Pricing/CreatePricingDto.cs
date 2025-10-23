using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs.Pricing
{
    public class CreatePricingDto
    {
        [Required(ErrorMessage = "Tên (Name) là bắt buộc.")]
        [MaxLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Loại giá (PriceType) là bắt buộc.")]
        [EnumDataType(typeof(PriceType), ErrorMessage = "Loại giá không hợp lệ.")]
        public PriceType PriceType { get; set; } // Sử dụng enum

        [Required(ErrorMessage = "Giá mỗi kWh (PricePerKwh) là bắt buộc.")]
        [Range(0.00, 1000000.00, ErrorMessage = "Giá mỗi kWh phải là số không âm.")]
        public decimal PricePerKwh { get; set; }

        // Cho phép null, nhưng nếu có giá trị thì phải > 0
        [Range(0.01, 100000.00, ErrorMessage = "Giá mỗi phút (PricePerMinute) (nếu có) phải là số dương.")]
        public decimal? PricePerMinute { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu hiệu lực (EffectiveFrom) là bắt buộc.")]
        [DataType(DataType.DateTime)] // Chỉ định rõ kiểu dữ liệu
        public DateTime EffectiveFrom { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc hiệu lực (EffectiveTo) là bắt buộc.")]
        [DataType(DataType.DateTime)]
        public DateTime EffectiveTo { get; set; }

        public bool IsActive { get; set; } = true;
    }
}