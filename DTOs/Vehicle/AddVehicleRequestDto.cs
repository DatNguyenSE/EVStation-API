using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Vehicle
{
    public class AddVehicleRequestDto
    {
        [Required]
        public string Model { get; set; } = string.Empty;

        [Required]
        [RegularExpression("Car|Motorbike", ErrorMessage = "Loại xe phải là 'Car' hoặc 'Motorbike'.")]
        public VehicleType Type { get; set; }

        public double BatteryCapacityKWh { get; set; }
        public double MaxChargingPowerKW { get; set; }
        public ConnectorType ConnectorType { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Biển số xe phải có từ 6 đến 15 ký tự.")]
        public string Plate { get; set; } = string.Empty;

        // ----- TRƯỜNG MỚI ĐỂ UPLOAD FILE -----
        [Required(ErrorMessage = "Vui lòng cung cấp ảnh mặt trước cà vẹt xe.")]
        public IFormFile RegistrationImageFront { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng cung cấp ảnh mặt sau cà vẹt xe.")]
        public IFormFile RegistrationImageBack { get; set; } = null!;
    }
}