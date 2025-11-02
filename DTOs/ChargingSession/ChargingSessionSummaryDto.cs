using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.ChargingSession
{
    public class ChargingSessionSummaryDto
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal EnergyUsed { get; set; }
        public string ChargingPostCode { get; set; } = string.Empty; // Tên trụ sạc
    }
}