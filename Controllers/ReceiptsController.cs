using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs.Receipt;
using API.Helpers;
using API.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/receipts")]
    [Authorize]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;
        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        /// <summary>
        /// [STAFF] Xác nhận thanh toán (tiền mặt/thẻ) cho hóa đơn của khách vãng lai.
        /// </summary>
        [HttpPost("{id}/confirm-payment-by-staff")]
        [Authorize(Roles = AppConstant.Roles.Operator)]
        public async Task<IActionResult> ConfirmPaymentByStaff(int id, [FromBody] ConfirmPaymentRequestDto dto)
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId))
            {
                return Unauthorized("Không thể xác định nhân viên.");
            }

            if (string.IsNullOrWhiteSpace(dto.PaymentMethod))
            {
                return BadRequest("Phương thức thanh toán không được để trống.");
            }

            var result = await _receiptService.ConfirmWalkInPaymentAsync(id, staffId, dto.PaymentMethod);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage?.Contains("Không tìm thấy") == true)
                {
                    return NotFound(result.ErrorMessage);
                }

                return BadRequest(result.ErrorMessage);
            }

            return NoContent(); // HTTP 204
        }

        /// <summary>
        /// [ADMIN/STAFF] Hủy một hóa đơn đang ở trạng thái Pending.
        /// </summary>
        [HttpPost("{id}/cancel")]
        [Authorize(Roles = $"{AppConstant.Roles.Operator},{AppConstant.Roles.Admin}")]
        public async Task<IActionResult> CancelReceipt(int id, [FromBody] CancelRequestDto dto)
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId))
            {
                return Unauthorized("Không thể xác định nhân viên.");
            }

            var result = await _receiptService.CancelReceiptAsync(id, dto.Reason, staffId);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage?.Contains("Không tìm thấy") == true)
                {
                    return NotFound(result.ErrorMessage);
                }
                return BadRequest(result.ErrorMessage);
            }

            return NoContent();
        }

        /// <summary>
        /// [ADMIN] Thực hiện hoàn tiền cho một hóa đơn đã thanh toán.
        /// </summary>
        [HttpPost("refund")]
        [Authorize(Roles = AppConstant.Roles.Admin)] // Chỉ Admin mới được hoàn tiền
        public async Task<IActionResult> IssueRefund([FromBody] RefundRequestDto refundRequest)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(adminId))
            {
                return Unauthorized("Không thể xác định quản trị viên.");
            }

            var result = await _receiptService.IssueRefundForReceiptAsync(refundRequest, adminId);

            if (!result.IsSuccess)
            {
                if (result.ErrorMessage?.Contains("Không tìm thấy") == true)
                {
                    return NotFound(result.ErrorMessage);
                }
                return BadRequest(result.ErrorMessage);
            }

            return Ok("Hoàn tiền thành công.");
        }

        // === USER ENDPOINTS ===

        /// <summary>
        /// [USER] Lấy lịch sử hóa đơn (đã phân trang) của người dùng hiện tại.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserReceipts([FromQuery] PagingParams pagingParams)
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            var result = await _receiptService.GetUserReceiptsAsync(appUserId, pagingParams);
            var paged = result.Data;

            // Hàm này luôn trả về Success (có thể là danh sách rỗng)
            return Ok(
                new
                {
                    items = paged.ToList(),
                    pageNumber = paged.PageNumber,
                    pageSize = paged.PageSize,
                    totalItemCount = paged.TotalItemCount,
                    pageCount = paged.PageCount
                }
            );
        }

        /// <summary>
        /// [USER] Lấy chi tiết một hóa đơn CỦA CHÍNH người dùng hiện tại.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReceiptDetails(int id)
        {
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId))
            {
                return Unauthorized("Không thể xác định người dùng.");
            }

            var result = await _receiptService.GetReceiptDetailsAsync(id, appUserId);

            if (!result.IsSuccess)
            {
                // Service layer sẽ kiểm tra xem receipt.AppUserId == appUserId
                // Nếu không tìm thấy hoặc không có quyền, đều trả về lỗi
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        // === ADMIN/STAFF ENDPOINTS ===

        /// <summary>
        /// [ADMIN/STAFF] Lấy TẤT CẢ hóa đơn trong hệ thống (có lọc, phân trang).
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Roles = $"{AppConstant.Roles.Operator},{AppConstant.Roles.Admin}")]
        public async Task<IActionResult> GetAllReceiptsForAdmin(
            [FromQuery] ReceiptFilterParams filterParams,
            [FromQuery] PagingParams pagingParams)
        {
            var result = await _receiptService.GetAllReceiptsForAdminAsync(filterParams, pagingParams);
            var paged = result.Data;
            return Ok(new
            {
                items = paged.ToList(),
                pageNumber = paged.PageNumber,
                pageSize = paged.PageSize,
                totalItemCount = paged.TotalItemCount,
                pageCount = paged.PageCount
            });
        }

        /// <summary>
        /// [ADMIN/STAFF] Lấy chi tiết một hóa đơn BẤT KỲ theo ID.
        /// </summary>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = $"{AppConstant.Roles.Operator},{AppConstant.Roles.Admin}")]
        public async Task<IActionResult> GetReceiptByIdForAdmin(int id)
        {
            var result = await _receiptService.GetReceiptByIdForAdminAsync(id);

            if (!result.IsSuccess)
            {
                return NotFound(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }
}