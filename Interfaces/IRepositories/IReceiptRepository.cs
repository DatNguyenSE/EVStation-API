using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Enums;

namespace API.Interfaces
{
    public interface IReceiptRepository
    {
        Task AddAsync(Receipt receipt);
        Task<Receipt?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, ReceiptStatus receiptStatus);
    }
}