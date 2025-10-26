using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Enums
{
    public enum ReportSeverity
    {
        Normal,
        Critical
    }

    public enum ReportStatus
    {
        New,        // Staff vừa tạo
        Pending,    // Admin đã duyệt, chờ gán Technician
        InProgress, // Đã gán Technician, đang sửa
        Resolved,   // Technician báo đã sửa xong, chờ Admin xác nhận
        Closed,     // Admin đã xác nhận, trụ hoạt động lại
        Cancelled   // Admin hủy bỏ báo cáo
    }
}