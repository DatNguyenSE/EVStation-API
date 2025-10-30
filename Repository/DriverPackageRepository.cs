using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class DriverPackageRepository : IDriverPackageRepository
    {
        private readonly AppDbContext _context;
        public DriverPackageRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<DriverPackage> CreateAsync(string appUserId, int packageId)
        {
            var packageModel = await _context.ChargingPackages.FindAsync(packageId);
            if (packageModel == null)
            {
                throw new KeyNotFoundException("Không tìm thấy gói");
            }

            var userPackageModel = new DriverPackage
            {
                AppUserId = appUserId,
                PackageId = packageId,
                VehicleType = packageModel.VehicleType,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(packageModel.DurationDays),
                IsActive = true
            };

            await _context.DriverPackages.AddAsync(userPackageModel);

            return userPackageModel;
        }

        public async Task<DriverPackage?> DeactiveAsync(int id)
        {
            var userPackageModel = await _context.DriverPackages.FindAsync(id);
            if (userPackageModel == null)
            {
                return null;
            }

            userPackageModel.IsActive = false;
            return userPackageModel;
        }

        public async Task<DriverPackage?> GetActiveSubscriptionForUserAsync(string ownerId, VehicleType vehicleType)
        {
            return await _context.DriverPackages.Include(dp => dp.Package).FirstOrDefaultAsync(dp => dp.AppUserId == ownerId 
                                                       && dp.VehicleType == vehicleType
                                                       && dp.IsActive == true);
        }

        public Task<List<DriverPackage>> GetAllAsync()
        {
            return _context.DriverPackages.Include(p => p.Package).ToListAsync();
        }

        public async Task<DriverPackage?> GetByIdAsync(int id)
        {
            return await _context.DriverPackages.FindAsync(id);
        }

        public Task<List<DriverPackage>> GetByUserAsync(string userId)
        {
            return _context.DriverPackages.Where(p => p.AppUserId == userId).Include(p => p.Package).ToListAsync();
        }

        public async Task<bool> HasActivePackageAsync(string userId, VehicleType vehicleType)
        {
            return await _context.DriverPackages.AnyAsync(d =>
                d.AppUserId == userId &&
                d.VehicleType == vehicleType &&
                d.IsActive &&
                d.EndDate > DateTime.UtcNow);
        }
    }
}