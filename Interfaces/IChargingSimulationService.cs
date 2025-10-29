using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;
using API.Services;

namespace API.Interfaces
{
    public interface IChargingSimulationService
    {
        Task StartSimulationAsync(int sessionId, double batteryCapacity, bool isFreeCharging, bool guestMode, double chargerPowerKW,
            ConnectorType connectorType, double initialPercentage, double initialEnergy, int initialCost, string? ownerId, decimal walletBalance);
        Task StopSimulationAsync(int sessionId, bool setCompleted);
        bool IsRunning(int sessionId);
    }
}