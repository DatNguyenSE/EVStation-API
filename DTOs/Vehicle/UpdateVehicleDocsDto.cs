using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Vehicle
{
    public class UpdateVehicleDocsDto
    {
        [Required]
        public int VehicleId { get; set; }

        [Required]
        public IFormFile RegistrationImageFront { get; set; } = null!;

        [Required]
        public IFormFile RegistrationImageBack { get; set; } = null!;
    }
}