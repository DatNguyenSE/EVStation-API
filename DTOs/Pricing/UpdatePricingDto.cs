using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs.Pricing
{
    public class UpdatePricingDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public PriceType PriceType { get; set; } // Sử dụng enum

        [Required]
        public decimal PricePerKwh { get; set; }

        public decimal? PricePerMinute { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; }

        [Required]
        public DateTime EffectiveTo { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}