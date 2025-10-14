using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Enums
{
    public enum SessionStatus
    {
        // [Description("Chờ bắt đầu")] // lúc cắm sạc vô xe, chờ xét điều kiện tiền trong ví
        // Pending,
        // [Description("Đang sạc")] // đủ tiền, hệ thống bắt đầu sạc
        // Charging,
        // [Description("Đã đầy pin")] 
        // Full,
        // [Description("Hoàn tất")] // dừng sạc hoặc đầy pin và dừng sạc
        // Completed,
        // [Description("Đã hủy")] // cancel sạc khi ví thiếu tiền
        // Cancelled,
        // [Description("Lỗi")] // lỗi trụ/trạm, manager cancel trụ/trạm
        // Error
    }
}