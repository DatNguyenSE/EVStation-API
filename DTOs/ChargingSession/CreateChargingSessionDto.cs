using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.ChargingSession
{
    public class CreateChargingSessionDto
    {
        public int? VehicleId { get; set; }
        public string? VehiclePlate { get; set; }
        public int PostId { get; set; }
        public int? ReservationId { get; set; }
    }
}