using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IChargingSimulationService
    {
        Task StartSimulationAsync(int sessionId, double batteryCapacity);
        Task StopSimulation(int sessionId);
        bool IsRunning(int sessionId);
    }
}