using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Entities;

namespace API.Interfaces
{
    public interface IChargingSessionService
    {
        Task<ChargingSessionDto> CreateSessionAsync(CreateChargingSessionDto dto);  // Tạo session
        Task<ChargingSession> UpdatePlateAsync(int sessionId, string plate);
        Task<ChargingSessionDto> EndSessionAsync(int sessionId);                    // Kết thúc session
    }
}