using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;

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