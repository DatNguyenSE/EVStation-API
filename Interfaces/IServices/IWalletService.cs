using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Vnpay;
using API.DTOs.Wallet;
using API.Helpers.Enums;

namespace API.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDto?> GetWalletForUserAsync(string userId);
        Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(string userId);

        Task<string> CreatePaymentAsync(PaymentInformationModel model, string username, HttpContext context);
        Task<PaymentResponseModel> HandleVnpayCallbackAsync(IQueryCollection query);
        Task<(bool Success, string Message)> PayingChargeWalletAsync(int receiptId, string driverId, int total, TransactionType transactionType);
        Task<(bool Success, string Message)> CreateRefundTransactionAsync(string userId, decimal amount, string description, int receiptId);
    }
}