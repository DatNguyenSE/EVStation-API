using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;

namespace API.DTOs.ChargingSession
{
    public class ChargingSessionDetailDto : ChargingSessionHistoryDto
    {
        // Thông tin thêm về thời gian
        public DateTime? EndTime { get; set; }
        public DateTime? CompletedTime { get; set; }

        // Thông tin pin
        public double StartBatteryPercentage { get; set; }
        public double? EndBatteryPercentage { get; set; }

        // Chi tiết chi phí
        public decimal ChargingCost { get; set; }
        public decimal? IdleFee { get; set; }
        public decimal? OverstayFee { get; set; }

        // Trạng thái và loại phiên
        public StopReason? StopReason { get; set; }
        public bool IsWalkInSession { get; set; }
        public bool IsPaid { get; set; }

        // Liên kết với các đối tượng khác (nếu cần)
        public int? ReservationId { get; set; }
        public int? ReceiptId { get; set; }
    }
}