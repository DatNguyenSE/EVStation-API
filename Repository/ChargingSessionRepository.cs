using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
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
            return await _context.ChargingSessions.FindAsync(id);
        }

        public void Update(ChargingSession session)
        {
            _context.ChargingSessions.Update(session);
        }

    }
}