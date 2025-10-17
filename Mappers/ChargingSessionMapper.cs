using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Entities;

namespace API.Mappers
{
    public static class ChargingSessionMapper
    {
        public static ChargingSessionDto MapToDto(this ChargingSession session)
        {
            return new ChargingSessionDto
            {
                Id = session.Id,
                VehicleId = session.VehicleId,
                VehiclePlate = session.VehiclePlate,
                PostId = session.PostId,
                ReservationId = session.ReservationId,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                StartBatteryPercentage = session.StartBatteryPercentage,
                EndBatteryPercentage = session.EndBatteryPercentage,
                EnergyConsumed = session.EnergyConsumed,
                Status = session.Status,
                Cost = session.Cost
            };
        }
    }
}