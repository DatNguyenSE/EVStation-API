using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.ChargingSession
{
    public class ChargingSessionDto
    {
        public int Id { get; set; }
        public int? VehicleId { get; set; }
        public string VehiclePlate { get; set; } = string.Empty;
        public int PostId { get; set; }
        public int? ReservationId { get; set; } // sạc vãng lai kh cần đặt trước    
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public float? StartBatteryPercentage { get; set; }
        public float? EndBatteryPercentage { get; set; }
        public double EnergyConsumed { get; set; }
        public SessionStatus Status { get; set; }
        public int Cost { get; set; }
    }
}