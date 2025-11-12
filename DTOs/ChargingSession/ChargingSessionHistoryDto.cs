using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.ChargingSession
{
    public class ChargingSessionHistoryDto
    {
        public int Id { get; set; }
        public string VehiclePlate { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string ChargingPostCode { get; set; } = string.Empty;
        public SessionStatus Status { get; set; }
        public decimal TotalCost { get; set; }

        public double EnergyConsumed { get; set; }
    }
}