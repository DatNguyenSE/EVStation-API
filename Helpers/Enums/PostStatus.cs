using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers.Enums
{
    public enum PostStatus
    {
        Available,   // sẵn sàng
        Occupied,    // đang có xe
        Maintenance, // bảo trì
        Offline,      // mất kết nối
    }
}