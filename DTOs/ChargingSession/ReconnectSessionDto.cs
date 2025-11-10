using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.ChargingSession
{
    public class ReconnectSessionDto
    {
        public int SessionId { get; set; }
        public int PostId { get; set; }
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string StationAddress { get; set; } = string.Empty;
        public PostDto? PostInfo { get; set; }
        public VehicleInfoDto? VehicleInfo { get; set; }
        public SessionStateDto? CurrentState { get; set; }
    }

    public class PostDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal PowerKW { get; set; }
        public string ConnectorType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class VehicleInfoDto
    {
        public string Plate { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public double BatteryCapacityKWh { get; set; }
    }

    public class SessionStateDto
    {
        public double BatteryPercent { get; set; }
        public double ChargedKwh { get; set; }
        public int TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? IdleFeeStartTime { get; set; }
        public int? GraceTimeRemainingSeconds { get; set; }
    }
}