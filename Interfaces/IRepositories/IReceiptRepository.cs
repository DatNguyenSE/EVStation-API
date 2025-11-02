using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Enums;

namespace API.Interfaces
{
    public interface IReceiptRepository
    {
        Task AddAsync(Receipt receipt);
        Task<Receipt?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, ReceiptStatus receiptStatus);
        void Update(Receipt receipt);
        /// <summary>
        /// Lấy hóa đơn KÈM THEO danh sách ChargingSessions.
        /// Cần thiết cho việc cập nhật trạng thái các phiên sạc khi thanh toán.
        /// </summary>
        Task<Receipt?> GetReceiptWithChargingSessionsAsync(int id);

        /// <summary>
        /// Lấy hóa đơn KÈM THEO TẤT CẢ chi tiết (User, Staff, Sessions, Transactions).
        /// Dùng cho các trang Chi tiết Hóa đơn.
        /// </summary>
        Task<Receipt?> GetReceiptWithDetailsAsync(int id);

        /// <summary>
        /// Trả về một IQueryable (chưa thực thi) để xây dựng truy vấn động.
        /// Đã bao gồm các Include() cơ bản cần thiết cho SummaryDto (ví dụ: AppUser).
        /// </summary>
        IQueryable<Receipt> GetReceiptsQuery();

        Task<IEnumerable<Receipt>> GetPendingReceiptForOperator();
    }
}