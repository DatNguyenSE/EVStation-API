using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IPackageService
    {
        Task<(bool Success, string Message)> PurchasePackageAsync(string userId, int packageId);
        // Bạn có thể thêm các phương thức khác ở đây trong tương lai
        // ví dụ: Task<IEnumerable<ChargingPackageDto>> GetAvailablePackagesAsync();
    }
}