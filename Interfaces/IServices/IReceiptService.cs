using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Receipt;
using API.Helpers;
using X.PagedList;

namespace API.Interfaces.IServices
{
    // Dùng cho Commands (Create, Update, Delete)
    public record ServiceResult(bool IsSuccess, string? ErrorMessage = null);

    // Dùng cho Queries (Get)
    public record ServiceResult<T>(bool IsSuccess, T? Data, string? ErrorMessage = null)
    {
        // Factory methods cho tiện
        public static ServiceResult<T> Success(T data) => new(true, data, null);
        public static ServiceResult<T> Fail(string message) => new(false, default, message);
    }

    public interface IReceiptService
    {
        Task<ServiceResult> ConfirmWalkInPaymentAsync(int receiptId, string staffId, string paymentMethod);
        // === User Queries ===
        Task<ServiceResult<ReceiptDetailsDto>> GetReceiptDetailsAsync(int receiptId, string appUserId);
        Task<ServiceResult<IPagedList<ReceiptSummaryDto>>> GetUserReceiptsAsync(string appUserId, PagingParams pagingParams);

        // === Admin/Staff Queries ===
        Task<ServiceResult<IPagedList<ReceiptSummaryDto>>> GetAllReceiptsForAdminAsync(ReceiptFilterParams filterParams, PagingParams pagingParams);
        Task<ServiceResult<ReceiptDetailsDto>> GetReceiptByIdForAdminAsync(int receiptId);

        // === Financial Ops ===
        Task<ServiceResult> IssueRefundForReceiptAsync(RefundRequestDto refundRequest, string adminId);
        Task<ServiceResult> CancelReceiptAsync(int receiptId, string reason, string adminId);
        Task<IEnumerable<ReceiptSummaryDto>> GetPendingReceiptForOperator();
    }
}