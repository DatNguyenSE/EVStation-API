using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IChargingSessionRepository
    {
        Task<ChargingSession> CreateAsync(ChargingSession session);
        Task<ChargingSession?> GetByIdAsync(int id);
        void Update(ChargingSession session);
    }
}