using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.ChargingSession
{
    public class EnergyUpdateDto
    {
        public int SessionId { get; set; }
        public double EnergyConsumed { get; set; }
        public double BatteryPercentage { get; set; }
        public int TimeRemain { get; set; }
        public double Cost { get; set; }
    }
}