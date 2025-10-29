using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Enums;

namespace API.Interfaces
{
    public interface IChargingSessionRepository
    {
        Task<ChargingSession> CreateAsync(ChargingSession session);
        Task<List<ChargingSession>> GetAllAsync();
        Task<ChargingSession?> GetByIdAsync(int id);
        void Update(ChargingSession session);
<<<<<<< HEAD
        Task<ChargingSession?> FindIdleSessionForUserAtPost(string vehiclePlate, int postId);
        Task<ChargingSession?> FindLatestIdleSessionAtPostAsync(int postId);
        Task UpdatePayingStatusAsync(List<int> sessionId);
        Task<ChargingSession?> GetByIdAsyncNoTracking(int id);
=======
        Task<ChargingSession?> GetActiveSessionByPostIdAsync(int postId);
>>>>>>> a39fc31 (seed operator, manager, technician account and completed report flow)
    }
}