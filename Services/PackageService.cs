using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Entities.Wallet;
using API.Helpers.Enums;
using API.Interfaces;

namespace API.Services
{
    public class PackageService : IPackageService
    {
        private readonly IUnitOfWork _uow;
        public PackageService(IUnitOfWork uow)
        {
            _uow = uow;
        }
        public async Task<(bool Success, string Message)> PurchasePackageAsync(string userId, int packageId)
        {
            await using var dbTransaction = await _uow.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                // VALIDATION
                // tìm gói sạc
                var packageToPurchase = await _uow.ChargingPackages.GetByIdAsync(packageId);
                if (packageToPurchase == null || !packageToPurchase.IsActive)
                {
                    return (false, "Gói sạc không tồn tại hoặc không còn hoạt động.");
                }

                // Tìm ví của người dùng
                var userWallet = await _uow.Wallets.GetWalletByUserIdAsync(userId);
                if (userWallet == null)
                {
                    return (false, "Không tìm thấy ví của người dùng.");
                }

                // Kiểm tra số dư
                if (userWallet.Balance < packageToPurchase.Price)
                {
                    return (false, "Số dư trong ví không đủ để thực hiện giao dịch.");
                }

                // Kiểm tra gói active trùng loại xe
                var hasActivePackage = await _uow.DriverPackages
                    .HasActivePackageAsync(userId, packageToPurchase.VehicleType);

                if (hasActivePackage)
                {
                    return (false, "Bạn đã có một gói sạc đang hoạt động cho loại xe này.");
                }

                // XỬ LÝ GIAO DỊCH VÀ CẬP NHẬT DỮ LIỆU
                var balanceBefore = userWallet.Balance;

                // Trừ tiền trong ví
                userWallet.Balance -= packageToPurchase.Price;
                // Tạo WalletTransaction
                var transaction = new WalletTransaction
                {
                    WalletId = userWallet.Id,
                    TransactionType = TransactionType.BuyPackage,
                    Amount = packageToPurchase.Price,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = userWallet.Balance,
                    Description = $"Mua gói sạc: {packageToPurchase.Name}",
                    ReferenceId = packageToPurchase.Id,
                    Status = TransactionStatus.Success,
                    PaymentMethod = "Wallet",
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.WalletTransactions.AddTransactionAsync(transaction);

                // Tạo DriverPackage
                var driverPackage = await _uow.DriverPackages.CreateAsync(userId, packageId);

                //LƯU THAY ĐỔI VÀ COMMIT TRANSACTION
                if (await _uow.Complete())
                {
                    // nếu lưu thành công => commit transaction
                    await dbTransaction.CommitAsync();
                    return (true, "Mua gói thành công.");
                }
                
                // Nếu không lưu được, rollback và báo lỗi
                await dbTransaction.RollbackAsync();
                return (false, "Đã xảy ra lỗi hệ thống trong quá trình xử lý.");
                
            }
            catch (Exception e)
            {
                await dbTransaction.RollbackAsync();
                return (false, "Đã xảy ra lỗi hệ thống trong quá trình xử lý.");
            }
        }
    }
}