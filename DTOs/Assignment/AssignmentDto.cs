using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Account;
using API.DTOs.Station;

namespace API.DTOs.Assignment
{
    public class AssignmentDto
    {
        public int Id { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }

        public StaffDto Staff { get; set; } = null!;

        public StationDto Station { get; set; } = null!;
    }
}