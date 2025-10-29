using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Enums;

namespace API.Interfaces
{
    public interface IDriverPackageRepository
    {
        Task<List<DriverPackage>> GetAllAsync();
        Task<DriverPackage?> GetByIdAsync(int id);
        Task<DriverPackage?> GetActiveSubscriptionForUserAsync(string ownerId, VehicleType vehicleType);
        Task<List<DriverPackage>> GetByUserAsync(string userId);
        Task<DriverPackage> CreateAsync(string appUserId, int packageId);
        Task<DriverPackage?> DeactiveAsync(int id);

        Task<bool> HasActivePackageAsync(string userId, VehicleType vehicleType);
    }
}