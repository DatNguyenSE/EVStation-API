using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Vnpay;
using API.DTOs.Wallet;
using API.Helpers;
using API.Helpers.Enums;
using API.Interfaces.IServices;
using X.PagedList;

namespace API.Interfaces
{
    public interface IWalletService
    {
        Task<WalletDto?> GetWalletForUserAsync(string userId);
        Task<ServiceResult<IPagedList<TransactionDto>>> GetUserTransactionsAsync(string userId, PagingParams paging);

        Task<string> CreatePaymentAsync(PaymentInformationModel model, string username, HttpContext context);
        Task<PaymentResponseModel> HandleVnpayCallbackAsync(IQueryCollection query);
        Task<(bool Success, string Message)> PayingChargeWalletAsync(int receiptId, string driverId, int total, TransactionType transactionType);
        Task<(bool Success, string Message)> CreateRefundTransactionAsync(string userId, decimal amount, string description, int receiptId);
    }
}