using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Pricing;
using API.Entities;

namespace API.Interfaces
{
    public interface IPricingService
    {
        // === Chức năng nghiệp vụ ===

        /// <summary>
        /// Lấy giá đang hiệu lực cho một loại hình sạc cụ thể.
        /// </summary>
        Task<PricingDto> GetCurrentActivePriceByTypeAsync(PriceType priceType);

        // === Chức năng Quản lý (Admin) ===

        /// <summary>
        /// Lấy tất cả cấu hình giá.
        /// </summary>
        Task<IEnumerable<PricingDto>> GetAllPricingsAsync();

        /// <summary>
        /// Lấy một cấu hình giá theo ID.
        /// </summary>
        Task<PricingDto> GetPricingByIdAsync(int id);

        /// <summary>
        /// Tạo một cấu hình giá mới, có kiểm tra logic trùng lặp.
        /// </summary>
        Task<PricingDto> CreatePricingAsync(CreatePricingDto createDto);

        /// <summary>
        /// Cập nhật một cấu hình giá, có kiểm tra logic trùng lặp.
        /// </summary>
        Task UpdatePricingAsync(int id, UpdatePricingDto updateDto);

        /// <summary>
        /// "Xóa mềm" - Tắt kích hoạt một cấu hình giá.
        /// </summary>
        Task DeactivatePricingAsync(int id);
    }
}