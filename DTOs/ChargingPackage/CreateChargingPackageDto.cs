using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Enums;

namespace API.DTOs.ChargingPackage
{
    public class CreateChargingPackageDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public VehicleType VehicleType { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; } = 30;
    }
}