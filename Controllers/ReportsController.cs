using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs.Report;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IUnitOfWork _uow;
        public ReportsController(IReportService reportService, IUnitOfWork unitOfWork)
        {
            _reportService = reportService;
            _uow = unitOfWork;
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // --- CÁC HÀNH ĐỘNG (POST) THEO LUỒNG NGHIỆP VỤ ---

        // 1. (Staff) Tạo báo cáo

        // Angular sẽ gọi API này
        [HttpPost]
        [Authorize(Roles = AppConstant.Roles.Operator)]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportDto dto)
        {
            var staffId = GetCurrentUserId();
            // Khi hàm service này được gọi...
            var report = await _reportService.CreateReportAsync(dto, staffId!);
            // ...thì SignalR sẽ được kích hoạt TỪ BÊN TRONG service
            return Ok(report);
        }

        // 2. (Admin) Đánh giá báo cáo (Critical / No)
        [HttpPost("{id}/evaluate")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> EvaluateReport(int id, [FromBody] EvaluateReportDto dto)
        {
            var result = await _reportService.EvaluateReportAsync(id, dto);
            if (!result) return BadRequest("Không thể đánh giá báo cáo. (Có thể sai trạng thái hoặc ID)");
            return Ok(new { message = "Đánh giá báo cáo thành công." });
        }

        // 3. (Admin) Gán việc cho Kỹ thuật viên
        [HttpPost("{id}/assign")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> AssignTechnician(int id, [FromBody] AssignTechnicianDto dto)
        {
            var result = await _reportService.AssignTechnicianAsync(id, dto);
            if (!result) return BadRequest("Không thể gán việc. (Kiểm tra ID KTV hoặc trạng thái báo cáo)");
            return Ok(new { message = "Gán việc cho KTV thành công." });
        }

        // 4. (Technician) Báo cáo đã sửa xong
        [HttpPost("{id}/complete")]
        [Authorize(Roles = AppConstant.Roles.Technician)]
        public async Task<IActionResult> CompleteFix(int id, [FromBody] CompleteFixDto dto)
        {
            var technicianId = GetCurrentUserId(); // Chỉ KTV đang đăng nhập mới được báo cáo
            var result = await _reportService.CompleteFixAsync(id, dto, technicianId!);
            if (!result) return BadRequest("Không thể hoàn tất. (Bạn không được gán hoặc báo cáo sai trạng thái)");
            return Ok(new { message = "Báo cáo hoàn tất sửa chữa thành công." });
        }

        // 5. (Admin) Xác nhận và đóng báo cáo
        [HttpPost("{id}/close")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> CloseReport(int id)
        {
            var result = await _reportService.CloseReportAsync(id);
            if (!result) return BadRequest("Không thể đóng báo cáo. (Báo cáo chưa được KTV xử lý)");
            return Ok(new { message = "Báo cáo đã được đóng và trụ sạc đã kích hoạt." });
        }

        // --- CÁC API LẤY DỮ LIỆU (GET) CHO UI ---

        // (Lấy chi tiết 1 report)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,Technician")] // Cả 3 vai trò đều có thể xem
        public async Task<IActionResult> GetReportById(int id)
        {
            var reportDetailDto = await _reportService.GetReportDetailsAsync(id);
            if (reportDetailDto == null) return NotFound(new { message = "Không tìm thấy báo cáo." });
            return Ok(reportDetailDto);
        }

        // (Admin) Lấy danh sách report mới
        [HttpGet("new")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> GetNewReports()
        {
            var reportsDto = await _reportService.GetNewReportsAsync();
            return Ok(reportsDto);
        }
        
        // (Technician) Lấy danh sách việc của tôi
        [HttpGet("mytasks")]
        [Authorize(Roles = AppConstant.Roles.Technician)]
        public async Task<IActionResult> GetMyTasks()
        {
            var technicianId = GetCurrentUserId();
            var reportsDto = await _reportService.GetMyTasksAsync(technicianId!);
            return Ok(reportsDto);
        }
    }
}