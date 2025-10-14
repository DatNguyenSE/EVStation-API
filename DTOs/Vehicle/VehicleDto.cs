using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Vehicle
{
    public class VehicleDto
    {
        [Required]
        public string Model { get; set; } = string.Empty;      // VD: VF e34, Klara S

        [Required]
        [RegularExpression("Car|Motorbike", ErrorMessage = "Loại xe phải là 'Car' hoặc 'Motorbike'.")]
        public VehicleType Type { get; set; }    // "Car" hoặc "Motorbike"

        public double BatteryCapacityKWh { get; set; } // optional: FE có thể chỉnh
        public double MaxChargingPowerKW { get; set; } // optional: FE có thể chỉnh
        public ConnectorType ConnectorType { get; set; } // BE sẽ tự set

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Biển số xe phải có từ 6 đến 15 ký tự.")]
        public string Plate { get; set; } = string.Empty;
    }
}