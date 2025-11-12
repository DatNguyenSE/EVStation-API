using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IPricingRepository
    {
        // === Chức năng CRUD cơ bản ===

        /// <summary>
        /// Lấy một bản ghi Pricing theo Id
        /// </summary>
        Task<Pricing?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy tất cả các bản ghi Pricing
        /// </summary>
        Task<IEnumerable<Pricing>> GetAllAsync();

        /// <summary>
        /// Thêm một bản ghi Pricing mới vào context
        /// </summary>
        Task AddAsync(Pricing entity);

        /// <summary>
        /// Đánh dấu một bản ghi Pricing là đã thay đổi (Modified)
        /// </summary>
        void Update(Pricing entity);


        // === Chức năng nghiệp vụ ===

        /// <summary>
        /// Lấy mức giá đang hiệu lực (active, trong khoảng ngày) cho một loại cụ thể.
        /// </summary>
        Task<Pricing?> GetActivePriceByTypeAsync(PriceType priceType);

        /// <summary>
        /// Kiểm tra xem có cấu hình giá nào (cùng loại, đang active)
        /// bị trùng lặp ngày hiệu lực hay không.
        /// </summary>
        /// <param name="excludeId">Dùng khi Update, để loại trừ chính bản ghi đang sửa.</param>
        Task<bool> CheckForOverlappingPriceAsync(
            PriceType priceType,
            DateTime effectiveFrom,
            DateTime effectiveTo,
            int excludeId = 0);
    }
}