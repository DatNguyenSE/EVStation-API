using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class PricingRepository : IPricingRepository
    {
        private readonly AppDbContext _context;
        public PricingRepository(AppDbContext context)
        {
            _context = context;
        }

        // === Triển khai CRUD ===

        public async Task<Pricing?> GetByIdAsync(int id)
        {
            return await _context.Pricings.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Pricing>> GetAllAsync()
        {
            return await _context.Pricings.ToListAsync();
        }

        public async Task AddAsync(Pricing entity)
        {
            await _context.Pricings.AddAsync(entity);
        }

        public void Update(Pricing entity)
        {
            // Báo cho EF Core biết rằng entity này đã bị thay đổi
            _context.Pricings.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        // === Triển khai Nghiệp vụ ===

        public async Task<Pricing?> GetActivePriceByTypeAsync(PriceType priceType)
        {
            var now = DateTime.Now; // Hoặc DateTime.UtcNow

            return await _context.Pricings
                .Where(p => p.PriceType == priceType &&
                            p.IsActive &&
                            p.EffectiveFrom <= now &&
                            p.EffectiveTo >= now)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckForOverlappingPriceAsync(
            PriceType priceType,
            DateTime effectiveFrom,
            DateTime effectiveTo,
            int excludeId = 0)
        {
            // Logic kiểm tra chồng lấn: (StartA < EndB) AND (EndA > StartB)
            // Tìm bất kỳ bản ghi nào (cùng loại, đang active, không phải chính nó)
            // mà khoảng thời gian của nó bị chồng lấn với khoảng thời gian mới.
            return await _context.Pricings
                .Where(p => p.PriceType == priceType &&
                            p.IsActive &&
                            p.Id != excludeId) // Bỏ qua chính nó khi cập nhật
                .Where(p => p.EffectiveFrom < effectiveTo &&
                            p.EffectiveTo > effectiveFrom)
                .AnyAsync();
        }
    }
}