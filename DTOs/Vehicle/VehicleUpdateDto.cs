using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Vehicle
{
    public class VehicleUpdateDto
    {
        [Required]
        public string Model { get; set; } = string.Empty;      // VD: VF e34, Klara S
        [Required]
        [RegularExpression("Car|Motorbike", ErrorMessage = "Loại xe phải là 'Car' hoặc 'Motorbike'.")]
<<<<<<< HEAD
        public VehicleType Type { get; set; }    // "Car" hoặc "Motorbike"
=======
        public VehicleType Type { get; set; } 
>>>>>>> 2479b0f35c247d50308223bea54573b15487d31a
        [Required]
        [Range(0.1, 1000, ErrorMessage = "Dung lượng không hợp lệ.")]
        public double BatteryCapacityKWh { get; set; }         // dung lượng pin
        [Required]
        [Range(0.1, 500, ErrorMessage = "Công suất sạc tối đa không hợp lệ.")]
        public double MaxChargingPowerKW { get; set; }         // công suất sạc tối đa xe hỗ trợ
        [Required]
        [EnumDataType(typeof(ConnectorType), ErrorMessage = "Loại cổng sạc không hợp lệ.")]
        public ConnectorType ConnectorType { get; set; } // Type2 / CCS2 / Portable
        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Biển số xe phải có từ 6 đến 15 ký tự.")]
        public string Plate { get; set; } = string.Empty;
    }
}