using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IChargingService
    {
        Task<(bool CanStart, string Message, int? ReservationId, int? VehicleId)> ValidateScanAsync(int postId, string driverId);
        Task StartChargingAsync(string chargerId);
        Task StopChargingAsync(string chargerId);
    }
}