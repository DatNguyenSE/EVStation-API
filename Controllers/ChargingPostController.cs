using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingPost;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class ChargingPostController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public ChargingPostController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // Lấy danh sách trụ
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var posts = await _uow.ChargingPosts.GetAllAsync();
            var postDtos = posts.Select(s => s.ToPostDto()).ToList();
            return Ok(postDtos);
        }

        // Lấy chi tiết trụ theo id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var post = await _uow.ChargingPosts.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound("Không tìm thấy trụ sạc.");
            }
            return Ok(post.ToPostDto());
        }

        // lấy qrcode trụ
        [HttpGet("{id:int}/qrcode")]
        public async Task<IActionResult> GetQRCode(int id)
        {
            var post = await _uow.ChargingPosts.GetByIdAsync(id);
            if (post == null || post.QRCode == null)
                return NotFound("Không có QR code cho trụ này.");

            return File(post.QRCode, "image/png");
        }

        /*
        [HttpGet("{postId}/check-reservation")]
        public async Task<IActionResult> CheckReservation(int postId)
        {
            var driverId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var post = await _uow.ChargingPosts.GetByIdAsync(postId);
            if (post == null)
                return NotFound(new { message = "Không tìm thấy trụ sạc" });

            var reservation = await _uow.Reservations
                .GetActiveByPostIdAsync(postId);

            if (reservation == null)
            {
                return Ok(new
                {
                    canStart = false,
                    status = post.Status.ToString(),
                    message = "Trụ hiện chưa được đặt"
                });
            }

            if (reservation.DriverId == driverId)
            {
                return Ok(new
                {
                    canStart = true,
                    status = post.Status.ToString(),
                    message = "Bạn đã đặt trụ này, có thể bắt đầu sạc"
                });
            }

            return Ok(new
            {
                canStart = false,
                status = post.Status.ToString(),
                message = "Trụ đã được đặt bởi người khác"
            });
        }
        */

        [HttpPost("{stationId}/post")]
        public async Task<IActionResult> Create([FromRoute] int stationId, [FromBody] CreateChargingPostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postModel = postDto.ToChargingPostFromCreateDto();
            try
            {
                await _uow.ChargingPosts.CreateAsync(stationId, postModel);
                await _uow.Complete(); // Lưu thay đổi
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return CreatedAtAction(nameof(GetById), new { id = postModel.Id }, postModel.ToPostDto());
        }

        // Cập nhật trụ sạc
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateChargingPostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postModel = await _uow.ChargingPosts.UpdateAsync(id, postDto);
            if (postModel == null)
            {
                return NotFound("Không tìm thấy trụ để cập nhật.");
            }
            await _uow.Complete();
            return Ok(postModel.ToPostDto());
        }

        // Cập nhật trạng thái trụ sạc
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] PostStatus status)
        {
            var postModel = await _uow.ChargingPosts.UpdateStatusAsync(id, status);
            if (postModel == null)
            {
                return NotFound("Không tìm thấy trụ để cập nhật trạng thái.");
            }
            await _uow.Complete();
            return Ok(postModel.ToPostDto());
        }

        // Xóa trụ sạc
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var postModel = await _uow.ChargingPosts.DeleteAsync(id);
            if (postModel == null)
            {
                return NotFound("Không tìm thấy trụ để xóa.");
            }
            await _uow.Complete();
            return NoContent();
        }

        // Lấy các khung giờ còn trống của một trụ sạc
        [HttpGet("{postId:int}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots([FromRoute] int postId)
        {
            // --- Bước 1: Xác thực dữ liệu đầu vào ---
            // Kiểm tra trụ tồn tại
            var post = await _uow.ChargingPosts.GetByIdAsync(postId);
            if (post == null)
            {
                return NotFound("Không tìm thấy trụ sạc");
            }

            // lấy thông tin trạm (để biết giờ mở/đóng)
            var station = await _uow.Stations.GetByIdAsync(post.StationId);
            if (station == null)
            {
                return NotFound("Không tìm thấy trạm");
            }

            // --- Bước 2: Tính toán slot cho cả hai ngày ---

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            // Gọi hàm helper để lấy slot cho từng ngày
            var todaySlots = await GetAvailableSlotsForDateAsync(postId, station, today);
            var tomorrowSlots = await GetAvailableSlotsForDateAsync(postId, station, tomorrow);

            // --- Bước 3: Tạo đối tượng kết quả và trả về ---

            // Sử dụng Dictionary để cấu trúc kết quả cho dễ dùng ở phía client
            var result = new Dictionary<string, List<DateTime>>
            {
                { today.ToString("yyyy-MM-dd"), todaySlots },
                { tomorrow.ToString("yyyy-MM-dd"), tomorrowSlots }
            };

            return Ok(result);
        }

        /// <summary>
        /// Phương thức helper để tính toán các slot còn trống cho một trụ và một ngày cụ thể.
        /// </summary>
        /// <param name="postId">ID của trụ sạc</param>
        /// <param name="station">Đối tượng trạm chứa thông tin giờ mở/đóng cửa</param>
        /// <param name="date">Ngày cần tính toán</param>
        /// <returns>Danh sách các DateTime là thời gian bắt đầu của các slot còn trống</returns>
        private async Task<List<DateTime>> GetAvailableSlotsForDateAsync(int postId, Station station, DateTime date)
        {
            // Lấy tất cả các Reservation của trụ này trong ngày được chọn
            var reservationsOnDate = await _uow.Reservations.GetReservationsForPostOnDateAsync(postId, date);
            var bookedStartTimes = reservationsOnDate.Select(r => r.TimeSlotStart).ToHashSet();

            var availableSlots = new List<DateTime>();
            const int slotDurationMinutes = 60;

            var slotIterator = date.Date + station.OpenTime;
            var closingTime = date.Date + station.CloseTime;

            if (closingTime <= slotIterator)
            {
                closingTime = closingTime.AddDays(1);
            }

            while (slotIterator < closingTime)
            {
                // Chỉ thêm vào nếu slot chưa được đặt và chưa trôi qua
                if (!bookedStartTimes.Contains(slotIterator) && slotIterator >= DateTime.Now)
                {
                    availableSlots.Add(slotIterator);
                }

                slotIterator = slotIterator.AddMinutes(slotDurationMinutes);
            }

            return availableSlots;
        }
    }
}