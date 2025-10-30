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
        public IChargingSessionRepository ChargingSessions { get; }
        public IPricingRepository Pricings { get; }
<<<<<<< HEAD
        public IReceiptRepository Receipts { get; }
        public IReportRepository Reports { get; }
=======
        public IReportRepository Reports { get; }
        public IReceiptRepository Receipts { get; }
>>>>>>> 47e74898948af53b25e50ff2d61794ab9f0c2115

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
            IVehicleModelRepository vehicleModels,
            IChargingSessionRepository chargingSession,
            IPricingRepository pricing,
<<<<<<< HEAD
            IReceiptRepository receipts,
            IReportRepository report)
=======
            IReportRepository report,
            IReceiptRepository receipts)
>>>>>>> 47e74898948af53b25e50ff2d61794ab9f0c2115
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
            ChargingSessions = chargingSession;
            Pricings = pricing;
            Receipts = receipts;
            Reports = report;
        }

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level)
        {
            return await _context.Database.BeginTransactionAsync(level);
        }

        public void DetachAllEntities()
        {
            var changedEntriesCopy = _context.ChangeTracker.Entries()
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
    }
}