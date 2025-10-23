using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Vnpay;
using API.DTOs.Wallet;
using API.Entities;
using API.Entities.Wallet;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _uow;
        private readonly IVnPayService _vnPayService;
        private readonly UserManager<AppUser> _userManager;

        public WalletService(IUnitOfWork uow, IVnPayService vnPayService, UserManager<AppUser> userManager)
        {
            _uow = uow;
            _vnPayService = vnPayService;
            _userManager = userManager;
        }   
        public async Task<WalletDto?> GetWalletForUserAsync(string userId)
        {
            var wallet = await _uow.Wallets.GetWalletByUserIdAsync(userId);

            if (wallet == null)
            {
                await _uow.Wallets.CreateWalletAsync(userId);
                if (await _uow.Complete())
                {
                    wallet = await _uow.Wallets.GetWalletByUserIdAsync(userId);
                }
            }
            
            if (wallet == null) return null;

            return new WalletDto { Balance = wallet.Balance };
        }
        
        public async Task<IEnumerable<TransactionDto>> GetUserTransactionsAsync(string userId)
        {
            var wallet = await _uow.Wallets.GetWalletByUserIdAsync(userId);
            if (wallet == null)
            {
                // Tạo ví mới nếu chưa có, nhưng coi như không có giao dịch cũ
                await _uow.Wallets.CreateWalletAsync(userId);
                await _uow.Complete();
                return Enumerable.Empty<TransactionDto>();
            }

            var transactions = await _uow.WalletTransactions.GetTransactionsByWalletIdAsync(wallet.Id);
            return transactions.Select(t => new TransactionDto
            {
                TransactionType = t.TransactionType,
                BalanceBefore = t.BalanceBefore,
                BalanceAfter = t.BalanceAfter,
                Amount = t.Amount,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                Status = t.Status,
            });
        }

        public async Task<string> CreatePaymentAsync(PaymentInformationModel model, string username, HttpContext context)
        {
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
                throw new Exception("User not found");

            // Tạo hoặc lấy ví
            var wallet = await _uow.Wallets.GetWalletByUserIdAsync(appUser.Id);
            if (wallet == null)
                wallet = await _uow.Wallets.CreateWalletAsync(appUser.Id);
            await _uow.Complete();

            // Tạo giao dịch Pending
            var txnRef = DateTime.UtcNow.Ticks.ToString();
            var txn = new WalletTransaction
            {
                WalletId = wallet.Id,
                TransactionType = Helpers.Enums.TransactionType.Topup,
                Amount = (decimal)model.Amount,
                BalanceBefore = wallet.Balance,
                BalanceAfter = wallet.Balance + (decimal)model.Amount,
                Description = "Nạp tiền vào ví",
                Status = Helpers.Enums.TransactionStatus.Pending,
                PaymentMethod = "VNPAY",
                VnpTxnRef = txnRef,
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };
            await _uow.WalletTransactions.AddTransactionAsync(txn);
            if (!await _uow.Complete())
            {
                throw new Exception("Lỗi hệ thống: Không thể lưu giao dịch Pending.");
            }
            // Gán thông tin bổ sung
            model.Name = username;
            model.OrderType = "other"; 
            // Tạo URL thanh toán
            return _vnPayService.CreatePaymentUrl(model, context, txnRef);
        }

        public async Task<PaymentResponseModel> HandleVnpayCallbackAsync(IQueryCollection query)
        {
            var response = _vnPayService.PaymentExecute(query);
            var txn = await _uow.WalletTransactions.GetByVnpTxnRefAsync(response.OrderId);
            if (txn == null)
                throw new Exception("Không tìm thấy giao dịch");

            var wallet = txn.Wallet;

            if (response.Success)
            {
                txn.Status = Helpers.Enums.TransactionStatus.Success;
                wallet.Balance += txn.Amount;

                await _uow.WalletTransactions.UpdateTransactionAsync(txn);
                await _uow.Wallets.UpdateWalletAsync(wallet);
            }
            else
            {
                txn.Status = Helpers.Enums.TransactionStatus.Failed;
                await _uow.WalletTransactions.UpdateTransactionAsync(txn);
            }

            await _uow.Complete();
            return response;
        }
    }
}