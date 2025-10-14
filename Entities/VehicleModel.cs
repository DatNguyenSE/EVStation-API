using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities
{
    public class VehicleModel
    {
        public int Id { get; set; }
        public VehicleType Type { get; set; }
        public string Model { get; set; } = string.Empty;
        public double BatteryCapacityKWh { get; set; }
        public bool HasDualBattery { get; set; } = false;
        public double? MaxChargingPowerKW { get; set; } // dùng cho xe máy
        public double? MaxChargingPowerAC_KW { get; set; } // ô tô - sạc chậm
        public double? MaxChargingPowerDC_KW { get; set; } // ô tô - sạc nhanh
        public ConnectorType ConnectorType { get; set; }
    }
}