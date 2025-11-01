using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.DTOs.Receipt;
using API.DTOs.Wallet;
using API.Entities;
using API.Entities.Wallet;

namespace API.Mappers
{
    public static class ReceiptMappers
    {
        /// <summary>
        /// Chuyển đổi từ entity Receipt sang ReceiptDetailsDto (Chi tiết).
        /// </summary>
        public static ReceiptDetailsDto ToReceiptDetailsDto(this Receipt receipt)
        {
            var appUserName = receipt.AppUser?.FullName ?? "Khách vãng lai";
            var packageName = receipt.Package?.Package?.Name ?? "N/A";
            var staffName = receipt.ConfirmedByStaff?.FullName ?? "N/A";
            return new ReceiptDetailsDto
            {
                Id = receipt.Id,
                CreateAt = receipt.CreateAt,
                Status = receipt.Status,
                AppUserId = receipt.AppUserId,
                AppUserName = appUserName,
                PackageId = receipt.PackageId,
                PackageName = packageName,
                EnergyConsumed = receipt.EnergyConsumed,
                EnergyCost = receipt.EnergyCost,
                IdleStartTime = receipt.IdleStartTime,
                IdleEndTime = receipt.IdleEndTime,
                IdleFee = receipt.IdleFee,
                OverstayFee = receipt.OverstayFee,
                DiscountAmount = receipt.DiscountAmount,
                TotalCost = receipt.TotalCost, // Thêm TotalCost
                PricingName = receipt.PricingName,
                PricePerKwhSnapshot = receipt.PricePerKwhSnapshot,
                PaymentMethod = receipt.PaymentMethod,
                ConfirmedByStaffId = receipt.ConfirmedByStaffId,
                ConfirmedByStaffName = staffName,
                ConfirmedAt = receipt.ConfirmedAt,

                // Hoàn thiện phần collection mapping
                ChargingSessions = receipt.ChargingSessions
                                        .Select(cs => cs.ToChargingSessionSummaryDto())
                                        .ToList(),

                WalletTransactions = receipt.WalletTransactions
                                        .Select(wt => wt.ToWalletTransactionSummaryDto())
                                        .ToList()
            };
        }
        
        /// <summary>
        /// Chuyển đổi từ entity Receipt sang ReceiptSummaryDto (Tóm tắt).
        /// </summary>
        public static ReceiptSummaryDto ToReceiptSummaryDto(this Receipt receipt)
        {
            var appUserName = receipt.AppUser?.FullName ?? "Khách vãng lai";
            var staffName = receipt.ConfirmedByStaff?.FullName ?? "N/A";

            return new ReceiptSummaryDto
            {
                Id = receipt.Id,
                CreateAt = receipt.CreateAt,
                Status = receipt.Status,
                AppUserId = receipt.AppUserId,
                AppUserName = appUserName,
                TotalCost = receipt.TotalCost,
                PricingName = receipt.PricingName,
                PaymentMethod = receipt.PaymentMethod,
                ConfirmedAt = receipt.ConfirmedAt,
                ConfirmedByStaffName = staffName
            };
        }

        // --- MAPPERS PHỤ (HELPER MAPPERS) ---

        /// <summary>
        /// Chuyển đổi ChargingSession sang DTO tóm tắt.
        /// (Giả định ChargingSession có navigation property 'ChargingPost' và 'ChargingPost' có 'Name')
        /// </summary>
        public static ChargingSessionSummaryDto ToChargingSessionSummaryDto(this ChargingSession session)
        {
            return new ChargingSessionSummaryDto
            {
                Id = session.Id,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                EnergyUsed = (decimal)session.EnergyConsumed,
                ChargingPostCode = session.ChargingPost?.Code ?? "N/A"
            };
        }

        /// <summary>
        /// Chuyển đổi WalletTransaction sang DTO tóm tắt.
        /// </summary>
        public static WalletTransactionSummaryDto ToWalletTransactionSummaryDto(this WalletTransaction transaction)
        {
            return new WalletTransactionSummaryDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Type = transaction.TransactionType,
                TransactionDate = transaction.CreatedAt,
                Description = transaction.Description ?? string.Empty
            };
        }
    }
}