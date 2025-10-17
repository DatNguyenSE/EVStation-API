using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IReservationRepository Reservations { get; }
        public IChargingPostRepository ChargingPosts { get; }
        public IStationRepository Stations { get; }
        public IVehicleRepository Vehicles { get; }
        public IWalletRepository Wallets { get; }
        public IWalletTransactionRepository WalletTransactions { get; }
        public IChargingPackageRepository ChargingPackages { get; }
        public IDriverPackageRepository DriverPackages { get; }
        public IVehicleModelRepository VehicleModels { get; }

        public UnitOfWork(
            AppDbContext context,
            IReservationRepository reservations,
            IChargingPostRepository chargingPosts,
            IStationRepository stations,
            IVehicleRepository vehicles,
            IWalletRepository wallets,
            IWalletTransactionRepository walletTransactions,
            IChargingPackageRepository chargingPackages,
            IDriverPackageRepository driverPackages,
            IVehicleModelRepository vehicleModels)
        {
            _context = context;
            Reservations = reservations;
            ChargingPosts = chargingPosts;
            Stations = stations;
            Vehicles = vehicles;
            Wallets = wallets;
            WalletTransactions = walletTransactions;
            ChargingPackages = chargingPackages;
            DriverPackages = driverPackages;
            VehicleModels = vehicleModels;
        }

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level)
        {
            return await _context.Database.BeginTransactionAsync(level);
        }
    }
}