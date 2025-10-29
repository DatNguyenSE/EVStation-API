using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Enums
{
    public enum ReportSeverity
    {
        /// <summary>Vấn đề nhỏ, không ảnh hưởng lớn đến hoạt động.</summary>
        Minor,
        /// <summary>Vấn đề nghiêm trọng, gây gián đoạn dịch vụ quan trọng.</summary>
        Critical
    }

    public enum ReportStatus
    {
        /// <summary>Báo cáo mới được tạo.</summary>
        New,

        /// <summary>Đã được giao cho kỹ thuật viên.</summary>
        Assigned,

        /// <summary>Kỹ thuật viên đang xử lý.</summary>
        InProgress,

        /// <summary>Đã sửa xong và đang chờ xác nhận.</summary>
        Resolved,

        /// <summary>Đã hoàn tất và đóng.</summary>
        Closed,

        /// <summary>Không thể sửa được hoặc cần thêm thông tin.</summary>
        OnHold
    }
}