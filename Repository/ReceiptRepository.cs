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