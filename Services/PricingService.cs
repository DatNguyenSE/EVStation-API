using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Pricing;
using API.Entities;
using API.Interfaces;
using API.Mappers;

namespace API.Services
{
    public class PricingService : IPricingService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PricingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PricingDto> CreatePricingAsync(CreatePricingDto createDto)
        {
            // ### Logic nghiệp vụ 1: Kiểm tra ngày hợp lệ ###
            if (createDto.EffectiveFrom >= createDto.EffectiveTo)
            {
                throw new InvalidOperationException("EffectiveFrom must be earlier than EffectiveTo.");
            }

            // ### Logic nghiệp vụ 2: Kiểm tra trùng lặp ngày ###
            bool isOverlapping = await _unitOfWork.Pricings.CheckForOverlappingPriceAsync(
                createDto.PriceType,
                createDto.EffectiveFrom,
                createDto.EffectiveTo);

            if (isOverlapping)
            {
                throw new InvalidOperationException("Pricing date range overlaps with an existing active configuration for this price type.");
            }

            var pricing = createDto.ToPricingModel();

            // ### Logic nghiệp vụ 3: Xử lý logic đặc biệt cho OccupancyFee ###
            if (pricing.PriceType == PriceType.OccupancyFee)
            {
                pricing.PricePerKwh = 0; // Phí chiếm dụng không tính theo kWh
                if (!pricing.PricePerMinute.HasValue || pricing.PricePerMinute <= 0)
                {
                    throw new InvalidOperationException("Occupancy fee must have a valid PricePerMinute > 0.");
                }
            }
            else if (pricing.PricePerKwh <= 0) // Các loại phí khác phải có PricePerKwh
            {
                throw new InvalidOperationException("PricePerKwh must be > 0 for this price type.");
            }

            await _unitOfWork.Pricings.AddAsync(pricing);
            await _unitOfWork.Complete(); // Lưu thay đổi vào CSDL

            return pricing.ToPricingDto(); // Trả về DTO đã có Id
        }

        public async Task DeactivatePricingAsync(int id)
        {
            var pricing = await _unitOfWork.Pricings.GetByIdAsync(id);
            if (pricing == null)
            {
                throw new KeyNotFoundException("Pricing configuration not found.");
            }

            // Nếu đã tắt rồi thì không cần làm gì
            if (pricing.IsActive == false)
            {
                return;
            }

            pricing.IsActive = false; // Đây là "xóa mềm"
            _unitOfWork.Pricings.Update(pricing);
            await _unitOfWork.Complete();
        }

        public async Task<IEnumerable<PricingDto>> GetAllPricingsAsync()
        {
            var pricings = await _unitOfWork.Pricings.GetAllAsync();
            return pricings.Select(p => p.ToPricingDto());
        }

        public async Task<PricingDto> GetCurrentActivePriceByTypeAsync(PriceType priceType)
        {
            var pricing = await _unitOfWork.Pricings.GetActivePriceByTypeAsync(priceType);

            // Nếu không tìm thấy, ném lỗi để Controller bắt
            if (pricing == null)
            {
                throw new KeyNotFoundException($"No active pricing found for type '{priceType}'.");
            }

            return pricing.ToPricingDto();
        }

        public async Task<PricingDto> GetPricingByIdAsync(int id)
        {
            var pricing = await _unitOfWork.Pricings.GetByIdAsync(id);

            if (pricing == null)
            {
                throw new KeyNotFoundException("Pricing configuration not found.");
            }

            return pricing.ToPricingDto();
        }

        public async Task UpdatePricingAsync(int id, UpdatePricingDto updateDto)
        {
            var pricing = await _unitOfWork.Pricings.GetByIdAsync(id);
            if (pricing == null)
            {
                throw new KeyNotFoundException("Pricing configuration not found.");
            }

            // ### Logic nghiệp vụ 1: Kiểm tra ngày hợp lệ ###
            if (updateDto.EffectiveFrom >= updateDto.EffectiveTo)
            {
                throw new InvalidOperationException("EffectiveFrom must be earlier than EffectiveTo.");
            }

            // ### Logic nghiệp vụ 2: Kiểm tra trùng lặp (loại trừ chính nó) ###
            bool isOverlapping = await _unitOfWork.Pricings.CheckForOverlappingPriceAsync(
                updateDto.PriceType,
                updateDto.EffectiveFrom,
                updateDto.EffectiveTo,
                id); // <-- Quan trọng: Loại trừ ID hiện tại

            if (isOverlapping)
            {
                throw new InvalidOperationException("Pricing date range overlaps with an existing active configuration for this price type.");
            }

            pricing.Name = updateDto.Name;
            pricing.PriceType = updateDto.PriceType;
            pricing.PricePerKwh = updateDto.PricePerKwh;
            pricing.PricePerMinute = updateDto.PricePerMinute;
            pricing.EffectiveFrom = updateDto.EffectiveFrom;
            pricing.EffectiveTo = updateDto.EffectiveTo;
            pricing.IsActive = updateDto.IsActive;

            // ### Logic nghiệp vụ 3: Xử lý logic OccupancyFee (tương tự Create) ###
            if (pricing.PriceType == PriceType.OccupancyFee)
            {
                pricing.PricePerKwh = 0;
                if (!pricing.PricePerMinute.HasValue || pricing.PricePerMinute <= 0)
                {
                    throw new InvalidOperationException("Occupancy fee must have a valid PricePerMinute > 0.");
                }
            }
            else if (pricing.PricePerKwh <= 0)
            {
                throw new InvalidOperationException("PricePerKwh must be > 0 for this price type.");
            }

            _unitOfWork.Pricings.Update(pricing);
            await _unitOfWork.Complete(); // Lưu thay đổi
        }
    }
}