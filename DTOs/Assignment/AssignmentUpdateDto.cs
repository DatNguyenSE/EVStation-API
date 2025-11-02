using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.Assignment
{
    public class AssignmentUpdateDto
    {
        public string StaffId { get; set; } = string.Empty;
        public int StationId { get; set; }
        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public bool? IsActive { get; set; }
    }
}