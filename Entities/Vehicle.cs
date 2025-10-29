using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public VehicleType Type { get; set; }       // "Car" hoặc "Bike"

        public string Model { get; set; } = string.Empty;      // VD: VF e34, Klara S
        
        public double BatteryCapacityKWh { get; set; }         // dung lượng pin

        public double MaxChargingPowerKW { get; set; }         // công suất sạc tối đa xe hỗ trợ

        [Column(TypeName = "nvarchar(20)")]   // set type cho column chứ không nó để thành int
        public ConnectorType ConnectorType { get; set; }
        
        public string Plate { get; set; } = string.Empty;

        // Quan hệ với AppUser
        public string OwnerId { get; set; } = string.Empty;                   // FK tới AppUser.Id
        public AppUser Owner { get; set; } = null!;            // navigation property

        // Để đánh dấu trạng thái thay vì xóa
        public bool IsActive { get; set; } = true;
    }
}