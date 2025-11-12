using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Entities;
using API.Helpers;
using API.Helpers.Enums;
using X.PagedList;

namespace API.Interfaces
{
    public interface IChargingSessionRepository
    {
        Task<ChargingSession> CreateAsync(ChargingSession session);
        Task<List<ChargingSession>> GetAllAsync();
        Task<IPagedList<ChargingSessionHistoryDto>> GetSessionsByDriverAsync(string ownerId, PagingParams pagingParams);
        Task<List<ChargingSessionHistoryDto>> GetSessionsByStationAsync(int stationId);

        // Task<List<ChargingSessionDetailDto>> GetDetailSessionByIdAsync(int sessionId);
        Task<ChargingSession?> GetByIdAsync(int id);
        void Update(ChargingSession session);
        Task<ChargingSession?> FindIdleSessionForUserAtPost(string vehiclePlate, int postId);
        Task<ChargingSession?> FindLatestIdleSessionAtPostAsync(int postId);
        Task UpdatePayingStatusAsync(List<int> sessionId);
        Task<ChargingSession?> GetActiveSessionByPostIdAsync(int postId);
    }
}