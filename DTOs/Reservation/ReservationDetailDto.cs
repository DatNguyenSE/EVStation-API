using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.Reservation
{
    public class ReservationDetailDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string? DriverId { get; set; }
        public DateTime TimeSlotStart { get; set; }
        public DateTime TimeSlotEnd { get; set; }
        public string Status { get; set; } = string.Empty; // Trạng thái đặt chỗ

        // Thông tin Trụ Sạc (Post)
        public int PostId { get; set; }
        public string PostCode { get; set; } = string.Empty;
        public string ConnectorType { get; set; } = string.Empty;
        public decimal PowerKW { get; set; }
        
        // Thông tin Trạm Sạc (Station)
        public int StationId { get; set; }
        public string StationName { get; set; } = string.Empty;
        public string StationAddress { get; set; } = string.Empty;
    }
}