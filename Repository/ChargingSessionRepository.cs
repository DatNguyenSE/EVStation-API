using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.ChargingSession;
using API.Entities;
using API.Helpers;
using API.Helpers.Enums;
using API.Interfaces;
using API.Mappers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.EF;

namespace API.Repository
{
    public class ChargingSessionRepository : IChargingSessionRepository
    {
        private readonly AppDbContext _context;
        public ChargingSessionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ChargingSession> CreateAsync(ChargingSession sessionModel)
        {
            await _context.ChargingSessions.AddAsync(sessionModel);
            return sessionModel;
        }

        public async Task<ChargingSession?> GetActiveSessionByPostIdAsync(int postId)
        {
            // Lấy session kèm theo Vehicle (quan trọng để lấy OwnerId)
            return await _context.ChargingSessions
                .Include(s => s.Vehicle)
                .Where(s => s.ChargingPostId == postId &&
                            (s.Status == SessionStatus.Charging || s.Status == SessionStatus.Full))
                // === THÊM DÒNG NÀY ===
                .OrderByDescending(s => s.StartTime) // Lấy session mới nhất
                .FirstOrDefaultAsync();
        }

        public async Task<ChargingSession?> GetByIdAsync(int id)
        {
            return await _context.ChargingSessions.Include(s => s.ChargingPost)
                                                .Include(s => s.Reservation)
                                                .Include(s => s.Vehicle)
                                                    .ThenInclude(v => v!.Owner).FirstOrDefaultAsync(s => s.Id == id);
        }

        public void Update(ChargingSession session)
        {
            _context.ChargingSessions.Update(session);
        }

        public async Task<ChargingSession?> FindIdleSessionForUserAtPost(string vehiclePlate, int postId)
        {
            return await _context.ChargingSessions
                .FirstOrDefaultAsync(s =>
                    s.VehiclePlate == vehiclePlate &&
                    s.ChargingPostId == postId &&
                    s.Status == SessionStatus.Idle);
        }

        public async Task<ChargingSession?> FindLatestIdleSessionAtPostAsync(int postId)
        {
            return await _context.ChargingSessions
                .Where(s => s.ChargingPostId == postId && s.Status == SessionStatus.Idle)
                .OrderByDescending(s => s.EndTime)
                .FirstOrDefaultAsync();
        }

        public async Task<List<ChargingSession>> GetAllAsync()
        {
            return await _context.ChargingSessions.Include(cs => cs.Reservation).ToListAsync();
        }

        public async Task UpdatePayingStatusAsync(List<int> sessionIds)
        {
            foreach (int sessionId in sessionIds)
            {
                var sessionModel = await _context.ChargingSessions.FindAsync(sessionId);
                if (sessionModel != null)
                {
                    sessionModel.IsPaid = true;
                }
            }

        }

        public async Task<IPagedList<ChargingSessionHistoryDto>> GetSessionsByDriverAsync(string ownerId, PagingParams pagingParams)
        {
            var query = _context.ChargingSessions
                .AsNoTracking()
                .Where(cs => cs.Vehicle != null && cs.Vehicle.OwnerId == ownerId)
                .OrderByDescending(cs => cs.StartTime)
                .Select(cs => new ChargingSessionHistoryDto
                {
                    Id = cs.Id,
                    VehiclePlate = cs.VehiclePlate,
                    StartTime = cs.StartTime,
                    StationName = cs.ChargingPost.StationName,
                    ChargingPostCode = cs.ChargingPost.Code,
                    Status = cs.Status,
                    TotalCost = cs.TotalCost,
                    EnergyConsumed = cs.EnergyConsumed
                });

            return await query.ToPagedListAsync(pagingParams.PageNumber, pagingParams.PageSize);
        }

        public async Task<List<ChargingSessionHistoryDto>> GetSessionsByStationAsync(int stationId)
        {
            var result = await _context.ChargingSessions
                                        .AsNoTracking()
                                        .Where(cs => cs.ChargingPost.StationId == stationId)
                                        .Include(cs => cs.ChargingPost)
                                        .ToListAsync();

            List<ChargingSessionHistoryDto> sessions = new();
            foreach (var session in result)
            {
                sessions.Add(session.MapToHistoryDto());
            }

            return sessions;
        }
    }
}