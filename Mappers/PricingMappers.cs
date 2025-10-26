using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Pricing;
using API.Entities;
using Microsoft.AspNetCore.Authentication;

namespace API.Mappers
{
    public static class PricingMappers
    {
        public static PricingDto ToPricingDto(this Pricing pricing)
        {
            return new PricingDto
            {
                Id = pricing.Id,
                Name = pricing.Name,
                PriceType = pricing.PriceType,
                PricePerKwh = pricing.PricePerKwh,
                PricePerMinute = pricing.PricePerMinute,
                EffectiveFrom = pricing.EffectiveFrom,
                EffectiveTo = pricing.EffectiveTo,
                IsActive = pricing.IsActive,
            };
        }

        public static Pricing ToPricingModel(this CreatePricingDto dto)
        {
            return new Pricing
            {
                Name = dto.Name,
                PriceType = dto.PriceType,
                PricePerKwh = dto.PricePerKwh,
                PricePerMinute = dto.PricePerMinute,
                EffectiveFrom = dto.EffectiveFrom,
                EffectiveTo = dto.EffectiveTo,
                IsActive = dto.IsActive,
            };
        }
        
        public static Pricing ToPricingModel(this UpdatePricingDto dto)
        {
            return new Pricing
            {
                Name = dto.Name,
                PriceType = dto.PriceType,
                PricePerKwh = dto.PricePerKwh,
                PricePerMinute = dto.PricePerMinute,
                EffectiveFrom = dto.EffectiveFrom,
                EffectiveTo = dto.EffectiveTo,
                IsActive = dto.IsActive,
            };
        }
    }
}