using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.ChargingSession
{
    public class UpdatePlateRequest
    {
        public string Plate { get; set; } = string.Empty;
    }
}