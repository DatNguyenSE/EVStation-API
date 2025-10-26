using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities
{
    public class ChargingSession
    {
        [Key]
        public int Id { get; set; }

        public int? VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        public int ChargingPostId { get; set; }
        public ChargingPost ChargingPost { get; set; } = null!;

        public int? ReservationId { get; set; } // sạc vãng lai kh cần đặt trước    
        public Reservation? Reservation { get; set; }

        public string VehiclePlate { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }  // Thời điểm nhấn "Hoàn tất"
        public float StartBatteryPercentage { get; set; }
        public float? EndBatteryPercentage { get; set; }
        public double EnergyConsumed { get; set; }
        [Column(TypeName = "nvarchar(30)")]   // set type cho column chứ không nó để thành int
        public SessionStatus Status { get; set; }
        public int Cost { get; set; }
    }
}