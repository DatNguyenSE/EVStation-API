using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Vehicle
{
    public class VehicleResponseDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public VehicleType Type { get; set; }           // Car / Bike
        public double BatteryCapacityKWh { get; set; }
        public double MaxChargingPowerKW { get; set; }
        public ConnectorType ConnectorType { get; set; }
        public string Plate { get; set; } = string.Empty;
    }
}