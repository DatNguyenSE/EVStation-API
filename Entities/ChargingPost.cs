using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.Entities
{
    public class ChargingPost
    {
        public int Id { get; set; }
        public int StationId { get; set; }
        public string Code { get; set; } = string.Empty;
        [Column(TypeName = "nvarchar(20)")]   // set type cho column chứ không nó để thành int
        public PostType Type { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PowerKW { get; set; }
        [Column(TypeName = "nvarchar(20)")]   // set type cho column chứ không nó để thành int
        public ConnectorType ConnectorType { get; set; }
        [Column(TypeName = "nvarchar(20)")]   // set type cho column chứ không nó để thành int
        public PostStatus Status { get; set; }
        public bool IsWalkIn { get; set; } = false; // có phải trụ vãng lai không??
        public byte[]? QRCode { get; set; }
    }
}