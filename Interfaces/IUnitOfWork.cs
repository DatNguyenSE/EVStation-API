using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IReservationRepository Reservations { get; }
        IChargingPostRepository ChargingPosts { get; }
        IStationRepository Stations { get; }
        IVehicleRepository Vehicles { get; }
        IWalletRepository Wallets { get; }
        IWalletTransactionRepository WalletTransactions { get; }
        IChargingPackageRepository ChargingPackages { get; }
        IDriverPackageRepository DriverPackages { get; }

        Task<IDbContextTransaction> BeginTransactionAsync(IsolationLevel level);

        // Hàm duy nhất thực hiện SaveChangesAsync cho toàn bộ DbContext
        Task<bool> Complete();
    }
}