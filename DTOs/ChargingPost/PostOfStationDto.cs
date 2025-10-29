using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.ChargingPost
{
    public class PostOfStationDto
    {
        public string Code { get; set; } = string.Empty;
        public PostType Type { get; set; }
        public decimal PowerKW { get; set; }
        public ConnectorType ConnectorType { get; set; }
        public PostStatus Status { get; set; }
        public bool IsWalkIn { get; set; }
    }
}