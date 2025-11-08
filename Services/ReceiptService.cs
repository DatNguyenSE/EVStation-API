using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Receipt;
using API.Helpers;
using API.Helpers.Enums;
using API.Interfaces;
using API.Interfaces.IServices;
using API.Mappers;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.EF;

namespace API.Services
{
    public class ReceiptService : IReceiptService
    {
        private readonly IUnitOfWork _uow;
        private readonly IWalletService _walletService;
        private readonly IEmailService _emailService;
        public ReceiptService(IUnitOfWork uow, IWalletService walletService, IEmailService emailService)
        {
            _uow = uow;
            _walletService = walletService;
            _emailService = emailService;
        }

        public async Task<ServiceResult> CancelReceiptAsync(int receiptId, string reason, string managerId)
        {
            var receipt = await _uow.Receipts.GetByIdAsync(receiptId);

            if (receipt == null)
                return new ServiceResult(false, "Không tìm thấy hóa đơn.");

            if (receipt.Status != ReceiptStatus.Pending)
                return new ServiceResult(false, "Chỉ có thể hủy hóa đơn đang ở trạng thái 'Pending'.");

            receipt.Status = ReceiptStatus.Cancelled;
            receipt.ConfirmedByStaffId = managerId; // Dùng trường này để ghi nhận ai đã hủy
            receipt.ConfirmedAt = DateTime.UtcNow.AddHours(7);
            // GHI CHÚ: Bạn nên thêm một trường 'Notes' vào Receipt để lưu 'reason'

            _uow.Receipts.Update(receipt);

            if (await _uow.Complete())
            {
                return new ServiceResult(true);
            }

            return new ServiceResult(false, "Lỗi khi lưu vào cơ sở dữ liệu.");
        }

        public async Task<ServiceResult> ConfirmWalkInPaymentAsync(int receiptId, string staffId, PaymentMethod paymentMethod)
        {
            // 1. Lấy dữ liệu từ repository
            var receipt = await _uow.Receipts.GetReceiptWithChargingSessionsAsync(receiptId);

            if (receipt == null)
            {
                return new ServiceResult(false, "Không tìm thấy hóa đơn.");
            }

            if (receipt.Status != ReceiptStatus.Pending)
            {
                return new ServiceResult(false, $"Không thể xác nhận hóa đơn ở trạng thái '{receipt.Status}'.");
            }

            // 3. Cập nhật đối tượng
            receipt.Status = ReceiptStatus.Paid;
            receipt.ConfirmedByStaffId = staffId;
            receipt.ConfirmedAt = DateTime.UtcNow.AddHours(7);
            receipt.PaymentMethod = paymentMethod.ToString();
            _uow.Receipts.Update(receipt);

            await _uow.ChargingSessions.UpdatePayingStatusAsync(receipt.ChargingSessions.Select(cs => cs.Id).ToList());

            // 5. Lưu thay đổi
            var success = await _uow.Complete();

            if (!success)
            {
                return new ServiceResult(false, "Lỗi khi lưu vào cơ sở dữ liệu.");
            }

            if(!string.IsNullOrEmpty(receipt.AppUserId) && receipt.AppUser != null)
            {
                await _emailService.SendChargingReceiptAsync(receipt.AppUser.Email!, receipt);
            }

            return new ServiceResult(true);
        }

        // === Admin/Staff Queries ===

        /// <summary>
        /// (Admin) Lấy tất cả hóa đơn, có bộ lọc và phân trang.
        /// </summary>
        public async Task<ServiceResult<IPagedList<ReceiptSummaryDto>>> GetAllReceiptsForAdminAsync(ReceiptFilterParams filterParams, PagingParams pagingParams)
        {
            var query = _uow.Receipts.GetReceiptsQuery(); // Lấy IQueryable
            // Lọc
            if (filterParams.Status.HasValue)
                query = query.Where(r => r.Status == filterParams.Status.Value);
            if (filterParams.StartDate.HasValue)
                query = query.Where(r => r.CreateAt.Date >= filterParams.StartDate.Value.Date);
            if (filterParams.EndDate.HasValue)
                query = query.Where(r => r.CreateAt.Date <= filterParams.EndDate.Value.Date);
            if (filterParams.IsWalkInOnly == true)
                query = query.Where(r => r.AppUserId == null);
            if (!string.IsNullOrEmpty(filterParams.AppUserName))
            {
                query = query.Where(r => r.AppUser != null &&
                                         EF.Functions.Like(r.AppUser.FullName, $"%{filterParams.AppUserName}%"));
            }


            // Sắp xếp
            query = query.OrderByDescending(r => r.CreateAt);

            // THAY ĐỔI: Thực thi truy vấn và Phân trang thủ công
            // 1. Phân trang trên IQueryable<Receipt> trước
            var pagedEntities = await query.ToPagedListAsync(pagingParams.PageNumber, pagingParams.PageSize);

            // 2. Map danh sách đã phân trang sang DTO
            var dtoList = pagedEntities.Select(r => r.ToReceiptSummaryDto()).ToList();

            // 3. Tạo PagedList tĩnh (StaticPagedList) từ DTO list
            var pagedResult = new StaticPagedList<ReceiptSummaryDto>(dtoList, pagedEntities);

            return ServiceResult<IPagedList<ReceiptSummaryDto>>.Success(pagedResult);
        }

        /// <summary>
        /// (Admin) Lấy chi tiết một hóa đơn bất kỳ mà không cần kiểm tra chủ sở hữu.
        /// </summary>
        public async Task<ServiceResult<ReceiptDetailsDto>> GetReceiptByIdForAdminAsync(int receiptId)
        {
            var receipt = await _uow.Receipts.GetReceiptWithDetailsAsync(receiptId);
            if (receipt == null)
            {
                return ServiceResult<ReceiptDetailsDto>.Fail("Không tìm thấy hóa đơn.");
            }

            // THAY ĐỔI: Dùng mapper thủ công
            var receiptDto = receipt.ToReceiptDetailsDto();
            return ServiceResult<ReceiptDetailsDto>.Success(receiptDto);
        }

        // === User Queries ===

        /// <summary>
        /// Lấy chi tiết hóa đơn cho chính người dùng đó.
        /// </summary>
        public async Task<ServiceResult<ReceiptDetailsDto>> GetReceiptDetailsAsync(int receiptId, string appUserId)
        {
            var receipt = await _uow.Receipts.GetReceiptWithDetailsAsync(receiptId);
            if (receipt == null)
            {
                return ServiceResult<ReceiptDetailsDto>.Fail("Không tìm thấy hóa đơn.");
            }
            if (receipt.AppUserId != appUserId)
            {
                return ServiceResult<ReceiptDetailsDto>.Fail("Không có quyền truy cập vào hóa đơn này.");
            }

            // THAY ĐỔI: Dùng mapper thủ công
            var receiptDto = receipt.ToReceiptDetailsDto();
            return ServiceResult<ReceiptDetailsDto>.Success(receiptDto);
        }

        /// <summary>
        /// Lấy lịch sử hóa đơn (phân trang) của chính người dùng đó.
        /// </summary>
        public async Task<ServiceResult<IPagedList<ReceiptSummaryDto>>> GetUserReceiptsAsync(string appUserId, PagingParams pagingParams)
        {
            var query = _uow.Receipts.GetReceiptsQuery();
            query = query.Where(r => r.AppUserId == appUserId);
            query = query.OrderByDescending(r => r.CreateAt);

            // THAY ĐỔI: Thực thi truy vấn và Phân trang thủ công (giống hệt hàm GetAll...)
            // 1. Phân trang trên IQueryable<Receipt>
            var pagedEntities = await query.ToPagedListAsync(pagingParams.PageNumber, pagingParams.PageSize);

            // 2. Map
            var dtoList = pagedEntities.Select(r => r.ToReceiptSummaryDto()).ToList();

            // 3. Tạo StaticPagedList
            var pagedResult = new StaticPagedList<ReceiptSummaryDto>(dtoList, pagedEntities);

            return ServiceResult<IPagedList<ReceiptSummaryDto>>.Success(pagedResult);
        }

        public async Task<ServiceResult> IssueRefundForReceiptAsync(RefundRequestDto refundRequest, string adminId)
        {
            await using var transaction = await _uow.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                var receipt = await _uow.Receipts.GetByIdAsync(refundRequest.ReceiptId);

                if (receipt == null)
                    return new ServiceResult(false, "Không tìm thấy hóa đơn.");
                if (receipt.Status != ReceiptStatus.Paid)
                    return new ServiceResult(false, "Chỉ có thể hoàn tiền cho hóa đơn đã thanh toán.");
                if (refundRequest.Amount <= 0 || refundRequest.Amount > receipt.TotalCost)
                    return new ServiceResult(false, "Số tiền hoàn không hợp lệ.");

                // 1. Cập nhật hóa đơn
                receipt.Status = ReceiptStatus.Refunded; // Thêm trạng thái Refunded vào Enum
                receipt.DiscountAmount = refundRequest.Amount; // Có thể dùng trường này
                // GHI CHÚ: Thêm trường 'Notes' để lưu lý do
                _uow.Receipts.Update(receipt);

                // 2. Hoàn tiền vào ví (nếu là user)
                if (receipt.AppUserId != null)
                {
                    var walletResult = await _walletService.CreateRefundTransactionAsync(
                        receipt.AppUserId,
                        refundRequest.Amount,
                        $"Hoàn tiền cho hóa đơn #{receipt.Id}. Lý do: {refundRequest.Reason}",
                        receipt.Id
                    );

                    if (!walletResult.Success)
                    {
                        await transaction.RollbackAsync();
                        return new ServiceResult(false, walletResult.Message);
                    }
                }

                // 3. Lưu
                if (!await _uow.Complete())
                {
                    await transaction.RollbackAsync();
                    return new ServiceResult(false, "Lỗi khi lưu vào cơ sở dữ liệu.");
                }

                await transaction.CommitAsync();
                return new ServiceResult(true);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                // TODO: Log ex
                return new ServiceResult(false, "Đã xảy ra lỗi hệ thống khi hoàn tiền.");
            }
        }

        public async Task<IEnumerable<ReceiptSummaryDto>> GetPendingReceiptForOperator(string staffId)
        {
            var assignment = await _uow.Assignments.GetCurrentAssignmentAsync(staffId);
            if (assignment != null)
            {
                var receipts = await _uow.Receipts.GetPendingReceiptForOperator(assignment.StationId);
                return receipts.Select(r => r.ToReceiptSummaryDto());
            }

            return Enumerable.Empty<ReceiptSummaryDto>();
        }
    }
}