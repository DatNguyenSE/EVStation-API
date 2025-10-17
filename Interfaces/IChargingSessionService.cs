using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;

namespace API.Interfaces
{
    public interface IChargingSessionService
    {
        Task<ChargingSessionDto> CreateSessionAsync(CreateChargingSessionDto dto);  // Tạo session
        Task UpdateEnergyAsync(EnergyUpdateDto update);                             // Cập nhật pin/năng lượng
        Task<ChargingSessionDto> EndSessionAsync(int sessionId);                    // Kết thúc session
    }
}