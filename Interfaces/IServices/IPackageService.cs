using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingPackage;

namespace API.Interfaces
{
    public interface IPackageService
    {
        Task<(bool Success, string Message)> PurchasePackageAsync(string userId, int packageId);
        Task<IEnumerable<ChargingPackageDto>> GetAvailablePackagesAsync();
    }
}