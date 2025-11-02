using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Assignment
{
    public class AssignmentCreateDto
    {
        [Required]
        public string StaffId { get; set; } = string.Empty;

        [Required]
        public int StationId { get; set; }

        [Required]
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }
    }
}