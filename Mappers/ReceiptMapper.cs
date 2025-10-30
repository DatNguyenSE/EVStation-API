using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Receipt;
using API.Entities;

namespace API.Mappers
{
    public static class ReceiptMapper
    {
        public static ReceiptDto MapToDto(this Receipt r)
        {
            var session = r.ChargingSessions.FirstOrDefault();
            return new ReceiptDto
            {
                Id = r.Id,
                SessionIds = r.ChargingSessions.Select(cs => cs.Id).ToList(),
                CreateAt = r.CreateAt,
                Status = r.Status,
                DriverId = r.AppUserId,
                DriverName = r.AppUser?.FullName,
                DriverEmail = r.AppUser?.Email,
                PackageId = r.PackageId,
                PackageName = r.Package?.Package.Name,
                EnergyConsumed = (double) r.EnergyConsumed,
                EnergyCost = r.EnergyCost,
                IdleFee = r.IdleFee,
                OverstayFee = r.OverstayFee,
                DiscountAmount = r.DiscountAmount,
                TotalCost = r.TotalCost,
                IdleStartTime = r.IdleStartTime,
                IdleEndTime = r.IdleEndTime,
                PricingName = r.PricingName,
                PricePerKwhSnapshot = r.PricePerKwhSnapshot,
                VehiclePlate = session?.VehiclePlate,
                StationName = session?.ChargingPost?.StationName,
                PostCode = session?.ChargingPost?.Code
            };
        }
    }
}