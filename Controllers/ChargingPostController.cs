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
using API.Helpers;

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
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GetQRCode(int id)
        {
            var post = await _uow.ChargingPosts.GetByIdAsync(id);
            if (post == null || post.QRCode == null)
                return NotFound("Không có QR code cho trụ này.");

            return File(post.QRCode, "image/png");
        }

        [HttpPost("{stationId}/post")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
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
        [Authorize(Roles = AppConstant.Roles.Admin)]
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
        [Authorize(Roles = AppConstant.Roles.Admin)]
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
        [Authorize(Roles = AppConstant.Roles.Admin)]
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
        [Authorize]
        public async Task<IActionResult> GetAvailableSlots([FromRoute] int postId)
        {
            var post = await _uow.ChargingPosts.GetByIdAsync(postId);
            if (post == null) return NotFound("Không tìm thấy trụ sạc");

            // lấy thông tin trạm (để biết giờ mở/đóng)
            var station = await _uow.Stations.GetByIdAsync(post.StationId);
            if (station == null) return NotFound("Không tìm thấy trạm");

            // --- Bước 2: Tính toán slot cho cả hai ngày ---
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);
            // Gọi hàm helper để lấy slot cho từng ngày
            var todaySlots = await GetAvailableSlotsForDateAsync(postId, station, today);
            var tomorrowSlots = await GetAvailableSlotsForDateAsync(postId, station, tomorrow);

            // --- Bước 3: Tạo đối tượng kết quả và trả về ---

            // Sử dụng Dictionary để cấu trúc kết quả cho dễ dùng ở phía client
            var result = new Dictionary<string, List<AvailableSlotDto>>
            {
                { today.ToString("yyyy-MM-dd"), todaySlots },
                { tomorrow.ToString("yyyy-MM-dd"), tomorrowSlots }
            };

            return Ok(result);
        }

        /// <summary>
        /// (Đã sửa lỗi) Lấy tất cả các slot 1 giờ đã bị chiếm dụng
        /// </summary>
        private async Task<HashSet<DateTime>> GetBookedSlotsAsync(int postId, DateTime date)
        {
            var reservationsOnDate = await _uow.Reservations.GetReservationsForPostOnDateAsync(postId, date);

            var bookedSlots = new HashSet<DateTime>();
            foreach (var res in reservationsOnDate)
            {
                // Vòng lặp này sẽ thêm 14:00, 15:00, 16:00 nếu đặt từ 14   :00-17:00
                var slotIterator = res.TimeSlotStart;
                while (slotIterator < res.TimeSlotEnd)
                {
                    // Chỉ thêm các slot thuộc ngày đang xét
                    if(slotIterator.Date == date.Date)
                    {
                        bookedSlots.Add(slotIterator);
                    }
                    slotIterator = slotIterator.AddMinutes(AppConstant.ReservationRules.slotDurationMinutes);
                }
            }
            return bookedSlots;
        }

        /// <summary>
        /// Phương thức helper để tính toán các slot còn trống cho một trụ và một ngày cụ thể.
        /// </summary>
        /// <param name="postId">ID của trụ sạc</param>
        /// <param name="station">Đối tượng trạm chứa thông tin giờ mở/đóng cửa</param>
        /// <param name="date">Ngày cần tính toán</param>
        /// <returns>Danh sách các DateTime là thời gian bắt đầu của các slot còn trống</returns>
        private async Task<List<AvailableSlotDto>> GetAvailableSlotsForDateAsync(int postId, Station station, DateTime date)
        {
            // Lấy danh sách các giờ đã bị đặt (đã sửa lỗi)
            var bookedStartTimes = await GetBookedSlotsAsync(postId, date);

            var availableSlotGroups = new List<AvailableSlotDto>();

            var slotIterator = date.Date + station.OpenTime;
            var closingTime = date.Date + station.CloseTime;

            // Xử lý trường hợp trạm mở qua đêm
            if (closingTime <= slotIterator)
            {
                closingTime = closingTime.AddDays(1);
            }
            var now = DateTime.Now;
            while (slotIterator < closingTime)
            {
                // Kiểm tra xem slot này có trống VÀ có nằm trong tương lai không
                if (!bookedStartTimes.Contains(slotIterator) && slotIterator >= now)
                {
                    // Đã tìm thấy một slot trống. Giờ hãy đếm xem có bao nhiêu slot liên tục...
                    var consecutiveCount = 0;
                    var lookAheadIterator = slotIterator;

                    // Vòng lặp "nhìn về phía trước"
                    while (consecutiveCount < AppConstant.ReservationRules.MaxSlotCount && // Dừng đếm nếu đã đủ 4
                           !bookedStartTimes.Contains(lookAheadIterator) && // Dừng nếu slot tiếp theo đã bị đặt
                           lookAheadIterator < closingTime) // Dừng nếu hết giờ làm việc
                    {
                        consecutiveCount++;
                        lookAheadIterator = lookAheadIterator.AddMinutes(AppConstant.ReservationRules.slotDurationMinutes);
                    }

                    // Thêm kết quả vào danh sách
                    availableSlotGroups.Add(new AvailableSlotDto
                    {
                        StartTime = slotIterator,
                        MaxConsecutiveSlots = consecutiveCount
                    });

                    // Quan trọng: KHÔNG nhảy cóc,
                    //    chỉ đi đến slot tiếp theo (cách 1 tiếng)
                    slotIterator = slotIterator.AddMinutes(AppConstant.ReservationRules.slotDurationMinutes);
                }
                else
                {
                    // Slot này đã bị đặt hoặc đã trôi qua, chuyển sang slot tiếp theo
                    slotIterator = slotIterator.AddMinutes(AppConstant.ReservationRules.slotDurationMinutes);
                }
            }

            return availableSlotGroups;
        }
    }
}