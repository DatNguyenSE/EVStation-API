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

        private IReservationRepository _reservations;
        private IChargingPostRepository _chargingPosts;
        private IQRCodeService _qrService;
        private IStationRepository _stations;
        private IVehicleRepository _vehicles;
        private IWalletRepository _wallets;
        private IWalletTransactionRepository _walletTransactions;
        private IChargingPackageRepository _chargingPackages;
        private IDriverPackageRepository _driverPackages;
        private IVehicleModelRepository _vehicleModels;
        private IChargingSessionRepository _chargingSessions;

        public UnitOfWork(AppDbContext context, IQRCodeService qrService)
        {
            _context = context;
            _qrService = qrService;
        }

        public IReservationRepository Reservations =>
            _reservations ??= new ReservationRepository(_context);

        public IChargingPostRepository ChargingPosts =>
            _chargingPosts ??= new ChargingPostRepository(_context, _qrService);

        public IStationRepository Stations =>
            _stations ??= new StationRepository(_context, ChargingPosts);

        public IVehicleRepository Vehicles =>
            _vehicles ??= new VehicleRepository(_context);

        public IWalletRepository Wallets =>
            _wallets ??= new WalletRepository(_context);

        public IWalletTransactionRepository WalletTransactions =>
            _walletTransactions ??= new WalletTransactionRepository(_context);

        public IChargingPackageRepository ChargingPackages =>
            _chargingPackages ??= new ChargingPackageRepository(_context);

        public IDriverPackageRepository DriverPackages =>
            _driverPackages ??= new DriverPackageRepository(_context);

        public IVehicleModelRepository VehicleModels =>
            _vehicleModels ??= new VehicleModelRepository(_context);
            
        public IChargingSessionRepository ChargingSessions =>
            _chargingSessions ??= new ChargingSessionRepository(_context);

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