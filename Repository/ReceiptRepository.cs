using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly AppDbContext _context;

        public ReceiptRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Receipt receipt)
        {
            await _context.Receipts.AddAsync(receipt);

        }

        public async Task<Receipt?> GetByIdAsync(int id)
        {
            return await _context.Receipts.Include(r => r.ChargingSessions).ThenInclude(cs => cs.ChargingPost).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Receipt>> GetPendingReceiptForOperator()
        {
            return await _context.Receipts
            .AsNoTracking()
            .Where(r => r.Status == ReceiptStatus.Pending)
            .OrderByDescending(r => r.CreateAt)
            .ToListAsync();

        public async Task<List<Receipt>> GetReceiptsByPlateAsync(string plate)
        {
            var normalizedPlate = plate.Trim().ToUpper();

            return await _context.Receipts.Where(r => r.ChargingSessions.Any(cs =>
                                                                            cs.Vehicle!.Plate.ToUpper() == normalizedPlate))
                                                                        .Include(r => r.ChargingSessions)
                                                                        .ThenInclude(cs => cs.Vehicle)
                                                                        .ToListAsync();
        }

        public IQueryable<Receipt> GetReceiptsQuery()
        {
            // AsNoTracking() tốt cho các truy vấn chỉ đọc
            return _context.Receipts
                .AsNoTracking()
                .Include(r => r.AppUser) // Cần cho AppUserName
                .Include(r => r.ConfirmedByStaff); // Cần cho ConfirmedByStaffName
        }

        public async Task<Receipt?> GetReceiptWithChargingSessionsAsync(int id)
        {
            return await _context.Receipts
                .Include(r => r.ChargingSessions)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Receipt?> GetReceiptWithDetailsAsync(int receiptId)
        {
            return await _context.Receipts
                .Include(r => r.AppUser) // Chi tiết người dùng
                .Include(r => r.ConfirmedByStaff) // Chi tiết nhân viên
                .Include(r => r.Package) // Chi tiết gói cước
                    .ThenInclude(dp => dp.Package)
                .Include(r => r.ChargingSessions) // Chi tiết phiên sạc
                    .ThenInclude(cs => cs.ChargingPost) // Chi tiết trụ sạc
                .Include(r => r.WalletTransactions) // Chi tiết giao dịch ví
                .FirstOrDefaultAsync(r => r.Id == receiptId);
        }

        public void Update(Receipt receipt)
        {
            _context.Receipts.Update(receipt);
        }

        public async Task UpdateStatusAsync(int id, ReceiptStatus receiptStatus)
        {
            var receiptModel = await _context.Receipts.FindAsync(id);
            if (receiptModel != null)
            {
                receiptModel.Status = receiptStatus;
            }
        }
    }
}