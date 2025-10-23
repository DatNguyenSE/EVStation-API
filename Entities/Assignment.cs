using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Assignment
    {
        public int Id { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeSpan ShiftStart { get; set; }
        public TimeSpan ShiftEnd { get; set; }

        // FK trỏ đến AppUser (nhân viên)
        public string StaffId { get; set; } = string.Empty;
        [ForeignKey("StaffId")]
        public AppUser Staff { get; set; } = null!;

        // FK trỏ đến Station
        public int StationId { get; set; }
        public Station Station { get; set; } = null!;
    }
}