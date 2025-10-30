using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs.Pricing
{
    public class PricingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public PriceType PriceType { get; set; }
        public decimal PricePerKwh { get; set; }
        public decimal? PricePerMinute { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime EffectiveTo { get; set; }
        public bool IsActive { get; set; }
    }
}