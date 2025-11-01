using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Receipt;
using API.DTOs.Vnpay;
using API.DTOs.Wallet;
using API.Entities;
using API.Entities.Wallet;
using API.Helpers.Enums;
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
                Amount = (decimal)model.Amount!,
                BalanceBefore = wallet.Balance,
                BalanceAfter = wallet.Balance,
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
            var txn = await _uow.WalletTransactions.GetByVnpTxnRefAsync(response.OrderId!);
            if (txn == null)
                throw new Exception("Không tìm thấy giao dịch");

            var wallet = txn.Wallet;

            if (response.Success)
            {
                txn.Status = Helpers.Enums.TransactionStatus.Success;
                wallet.Balance += txn.Amount;
                txn.BalanceAfter = wallet.Balance;

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

        public async Task<(bool Success, string Message)> PayingChargeWalletAsync(int receiptId, string driverId, int total, TransactionType transactionType = TransactionType.PayCharging)
        {
            await using var dbTransaction = await _uow.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var receiptOfSession = await _uow.Receipts.GetByIdAsync(receiptId);
                if (receiptOfSession == null || receiptOfSession.Status == ReceiptStatus.Paid)
                {
                    return (false, "Biên lai không tồn tại hoặc đã được thanh toán.");
                }

                // Tìm ví của người dùng
                var userWallet = await _uow.Wallets.GetWalletByUserIdAsync(driverId);
                if (userWallet == null)
                {
                    return (false, "Không tìm thấy ví của người dùng.");
                }

                // Kiểm tra số dư
                if (userWallet.Balance < total)
                {
                    return (false, "Số dư trong ví không đủ để thực hiện giao dịch."); // mốt có nợ thì bỏ vô nợ
                }

                // XỬ LÝ GIAO DỊCH VÀ CẬP NHẬT DỮ LIỆU
                var balanceBefore = userWallet.Balance;

                // Trừ tiền trong ví
                userWallet.Balance -= total;
                // Tạo WalletTransaction
                var transaction = new WalletTransaction
                {
                    WalletId = userWallet.Id,
                    TransactionType = transactionType,
                    Amount = total,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = userWallet.Balance,
                    Description = $"Thanh toán phiên sạc: {string.Join(", ", receiptOfSession.ChargingSessions.Select(cs => cs.Id))}",
                    ReferenceId = receiptOfSession.Id,
                    Status = TransactionStatus.Success,
                    PaymentMethod = "Wallet",
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };
                await _uow.WalletTransactions.AddTransactionAsync(transaction);

                // Tạo DriverPackage
                await _uow.Receipts.UpdateStatusAsync(receiptId, ReceiptStatus.Paid);
                var receipt = await _uow.Receipts.GetByIdAsync(receiptId);
                if(receipt != null ) receipt.WalletTransactions.Add(transaction);
                await _uow.ChargingSessions.UpdatePayingStatusAsync(receiptOfSession.ChargingSessions.Select(cs => cs.Id).ToList());

                //LƯU THAY ĐỔI VÀ COMMIT TRANSACTION
                if (await _uow.Complete())
                {
                    // nếu lưu thành công => commit transaction
                    await dbTransaction.CommitAsync();
                    return (true, "Thanh toán thành công.");
                }

                // Nếu không lưu được, rollback và báo lỗi
                await dbTransaction.RollbackAsync();
                return (false, "Đã xảy ra lỗi hệ thống trong quá trình xử lý.");

            }
            catch (Exception)
            {
                await dbTransaction.RollbackAsync();
                return (false, "Đã xảy ra lỗi hệ thống trong quá trình xử lý.");
            }
        }
        
        /// <summary>
        /// Tạo một giao dịch HOÀN TIỀN (cộng tiền vào ví).
        /// Hàm này KHÔNG gọi _uow.Complete() để đảm bảo tính toàn vẹn 
        /// khi được gọi từ một transaction lớn hơn (như ReceiptService).
        /// </summary>
        public async Task<(bool Success, string Message)> CreateRefundTransactionAsync(string userId, decimal amount, string description, int receiptId)
        {
            // 1. Tìm ví của người dùng
            var wallet = await _uow.Wallets.GetWalletByUserIdAsync(userId);
            if (wallet == null)
            {
                // Không thể hoàn tiền nếu không có ví
                return (false, "Không tìm thấy ví của người dùng.");
            }

            // 2. Ghi lại số dư trước
            var balanceBefore = wallet.Balance;

            // 3. Cộng tiền hoàn vào ví
            wallet.Balance += amount;

            // 4. Tạo đối tượng giao dịch mới
            var transaction = new WalletTransaction
            {
                WalletId = wallet.Id,
                TransactionType = Helpers.Enums.TransactionType.Refund, // <-- Cần thêm giá trị "Refund" vào Enum TransactionType
                Amount = amount, // Số tiền được cộng
                BalanceBefore = balanceBefore,
                BalanceAfter = wallet.Balance, // Số dư mới
                Description = description,
                ReferenceId = receiptId, // ID của Receipt được hoàn tiền
                Status = Helpers.Enums.TransactionStatus.Success, // Giao dịch nội bộ, thành công ngay
                PaymentMethod = "Refund",
                CreatedAt = DateTime.UtcNow.AddHours(7)
            };

            // 5. Thêm giao dịch vào UoW (chưa lưu)
            await _uow.WalletTransactions.AddTransactionAsync(transaction);

            // 6. Thông báo thành công (chưa commit)
            // Service gọi hàm này (ReceiptService) sẽ chịu trách nhiệm
            // gọi _uow.Complete() và Commit/Rollback transaction.
            return (true, "Giao dịch hoàn tiền đã được tạo.");
        }
    }
}