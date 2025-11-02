using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.ChargingSession;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using API.Mappers;
using Microsoft.EntityFrameworkCore;

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
            return await _context.ChargingSessions.ToListAsync();
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

        public async Task<List<ChargingSessionHistoryDto>> GetSessionsByDriverAsync(string ownerId)
        {
            var result = await _context.ChargingSessions
                                        .AsNoTracking()
                                        .Include(cs => cs.ChargingPost)
                                        .Where(cs => cs.Vehicle != null && cs.Vehicle.OwnerId == ownerId).ToListAsync();

            List<ChargingSessionHistoryDto> mySessions = new();
            foreach (var session in result)
            {
                mySessions.Add(session.MapToHistoryDto());
            }

            return mySessions;
        }

        public async Task<List<ChargingSessionHistoryDto>> GetSessionByStationAsync(int stationId)
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