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
                PostId = session.ChargingPostId,
                ReservationId = session.ReservationId,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                StartBatteryPercentage = (float)session.StartBatteryPercentage,
                EndBatteryPercentage = session.EndBatteryPercentage.HasValue
                        ? (float)session.EndBatteryPercentage.Value
                        : (float?)null,
                EnergyConsumed = session.EnergyConsumed,
                Status = session.Status,
                Cost = session.Cost
            };
        }

        public static ChargingSessionHistoryDto MapToHistoryDto(this ChargingSession session)
        {
            return new ChargingSessionHistoryDto
            {
                Id = session.Id,
                VehiclePlate = session.VehiclePlate,
                StartTime = session.StartTime,
                StationName = session.ChargingPost.StationName,
                ChargingPostCode = session.ChargingPost.Code,
                Status = session.Status,
                TotalCost = session.TotalCost,
                EnergyConsumed = session.EnergyConsumed
            };
        }
        
        public static ChargingSessionDetailDto MapToDetailDto(this ChargingSession session)
        {            
            string postCode = session.ChargingPost?.Code ?? "N/A";
            string stationName = session.ChargingPost?.StationName ?? "N/A";

            return new ChargingSessionDetailDto
            {
                // Thừa kế từ HistoryDto
                Id = session.Id,
                VehiclePlate = session.VehiclePlate,
                StartTime = session.StartTime,
                StationName = stationName, // Lấy từ ChargingPost
                ChargingPostCode = postCode, // Lấy từ ChargingPost
                Status = session.Status,
                TotalCost = session.TotalCost,
                EnergyConsumed = session.EnergyConsumed,
                
                // Các trường chi tiết thêm
                EndTime = session.EndTime,
                CompletedTime = session.CompletedTime,
                StartBatteryPercentage = (double) session.StartBatteryPercentage,
                EndBatteryPercentage = (double?) session.EndBatteryPercentage,
                ChargingCost = session.Cost,
                IdleFee = session.IdleFee,
                OverstayFee = session.OverstayFee,
                StopReason = session.StopReason,
                IsWalkInSession = session.IsWalkInSession,
                IsPaid = session.IsPaid,
                ReservationId = session.ReservationId,
                ReceiptId = session.ReceiptId
            };
        }
    }
}