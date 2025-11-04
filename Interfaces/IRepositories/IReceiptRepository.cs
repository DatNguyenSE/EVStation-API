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
        IQueryable<Receipt> GetAllReceiptsQueryable();
        Task<Receipt?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, ReceiptStatus receiptStatus);
        void Update(Receipt receipt);
        Task<Receipt?> GetReceiptWithChargingSessionsAsync(int id);

        Task<Receipt?> GetReceiptWithDetailsAsync(int id);

        IQueryable<Receipt> GetReceiptsQuery();

        Task<IEnumerable<Receipt>> GetPendingReceiptForOperator();
        Task<List<Receipt>> GetReceiptsByPlateAsync(string plate);
    }
}