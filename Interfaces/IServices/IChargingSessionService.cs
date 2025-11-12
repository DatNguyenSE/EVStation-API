using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.DTOs.Receipt;
using API.Entities;
using API.Helpers.Enums;

namespace API.Interfaces
{
    public interface IChargingSessionService
    {
        Task<ChargingSessionDto> CreateSessionAsync(CreateChargingSessionDto dto);
        Task StopChargingAsync(int sessionId, StopReason stopReason = StopReason.ManualStop);
        Task HandleSessionFullAsync(int sessionId);
        Task<ReceiptDto> CompleteSessionAsync(int sessionId, bool endReservation);
        Task<ChargingSession> UpdatePlateAsync(int sessionId, string plate);
    }
}