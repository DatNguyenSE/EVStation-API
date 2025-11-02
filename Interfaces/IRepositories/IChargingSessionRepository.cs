using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Entities;
using API.Helpers.Enums;

namespace API.Interfaces
{
    public interface IChargingSessionRepository
    {
        Task<ChargingSession> CreateAsync(ChargingSession session);
        Task<List<ChargingSession>> GetAllAsync();
        Task<List<ChargingSessionHistoryDto>> GetSessionsByDriverAsync(string ownerId);
        Task<List<ChargingSessionHistoryDto>> GetSessionByStationAsync(int stationId);

        // Task<List<ChargingSessionDetailDto>> GetDetailSessionByIdAsync(int sessionId);
        Task<ChargingSession?> GetByIdAsync(int id);
        void Update(ChargingSession session);
        Task<ChargingSession?> FindIdleSessionForUserAtPost(string vehiclePlate, int postId);
        Task<ChargingSession?> FindLatestIdleSessionAtPostAsync(int postId);
        Task UpdatePayingStatusAsync(List<int> sessionId);
        Task<ChargingSession?> GetActiveSessionByPostIdAsync(int postId);
    }
}