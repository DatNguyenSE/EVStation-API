using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Vehicle
{
    public class PendingVehicleDto
    {
        public int VehicleId { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Plate { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;

        // Thêm thông tin chủ xe
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;

        // URL đầy đủ của ảnh
        public string? RegistrationImageFrontUrl { get; set; }
        public string? RegistrationImageBackUrl { get; set; }
    }
}