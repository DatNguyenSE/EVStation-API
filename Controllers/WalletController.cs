using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs.Vnpay;
using API.DTOs.Wallet;
using API.Entities;
using API.Entities.Wallet;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace API.Controllers
{
    // [Authorize(Roles = AppConstant.Roles.Driver)]
    [ApiController]
    [Route("api/wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService; // SỬ DỤNG SERVICE
        private readonly UserManager<AppUser> _userManager;

        public WalletController(IWalletService walletService, UserManager<AppUser> userManager)
        {
            _walletService = walletService;
            _userManager = userManager;
        }

        // Lấy ví của user
        [HttpGet("me")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetMyWallet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Gọi Service Layer
            var walletDto = await _walletService.GetWalletForUserAsync(userId);

            if (walletDto == null)
            {
                // Nếu Service không thể tạo/tìm ví (lỗi DB hoặc user không tồn tại)
                return StatusCode(500, "Không thể khởi tạo hoặc tìm ví.");
            }

            return Ok(walletDto);
        }

        // Lấy lịch sử giao dịch
        [HttpGet("transactions")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTransaction([FromQuery] PagingParams paging)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _walletService.GetUserTransactionsAsync(userId, paging);

            if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

            // Trả thêm metadata phân trang
            var meta = result.Data.ToPaginationMeta();

            return Ok(new
            {
                items = result.Data,
                pagination = meta
            });
        }

        [HttpPost("top-up")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentInformationModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.GetUsername();
            var paymentUrl = await _walletService.CreatePaymentAsync(model, username, HttpContext);
            return Ok(new { paymentUrl });
        }

        [AllowAnonymous]
        [HttpGet("vnpay-return")]
        public async Task<IActionResult> PaymentCallback()
        {
            try
            {
                var response = await _walletService.HandleVnpayCallbackAsync(Request.Query);

                // Lấy lại query string do VNPAY gửi về
                var query = Request.QueryString.Value;

                // Redirect về frontend Angular
                return Redirect($"http://localhost:4200/thanh-toan{query}");
            }
            catch (Exception ex)
            {
                return Redirect($"http://localhost:4200/thanh-toan?error={ex.Message}");
            }
        }

        [Authorize(Roles = $"{AppConstant.Roles.Manager}, ${AppConstant.Roles.Admin}")] // Chỉ cho phép Manager hoặc Admin
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("manual-topup")]
        public async Task<IActionResult> ManualTopUp([FromBody] ManualTopUpDto model)
        {
            // Lấy tên Manager từ Token
            var managerName = User.GetUsername();

            var result = await _walletService.ManualTopUpByManagerAsync(model.DriverUserName, model.Amount, managerName);

            if (result.Success)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }
    }
}