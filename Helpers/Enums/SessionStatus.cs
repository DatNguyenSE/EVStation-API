using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Enums
{
    public enum SessionStatus
    {
        [Description("Đang sạc")] // đủ tiền, hệ thống bắt đầu sạc
        Charging,
        [Description("Đã đầy pin")]
        Full,
        [Description("Hoàn tất")] // dừng sạc hoặc đầy pin và dừng sạc
        Completed,
        [Description("Lỗi")] // lỗi trụ/trạm, manager cancel trụ/trạm
        Error,
        [Description("Dừng do không đủ tiền trong ví")]
        StoppedDueToInsufficientFunds,
        Idle
    }
}