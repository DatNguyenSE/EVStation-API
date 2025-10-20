using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs.Reservation;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/reservation")]
    [Authorize(Roles = AppConstant.Roles.Driver)]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        /// <summary>
        /// Đặt chỗ sạc xe (1–4 slot, mỗi slot = 1 tiếng)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto dto)
        {
            try
            {
                // Lấy driverId từ JWT 
                var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(driverId)) return Unauthorized();

                if (dto == null) return BadRequest("Dữ liệu đặt chỗ không hợp lệ.");

                var result = await _reservationService.CreateReservationAsync(dto, driverId);
                return Ok(new
                {
                    message = "Đặt chỗ thành công!",
                    data = result
                });

            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpPost("{reservationId:int}/cancel")]
        public async Task<IActionResult> CancelReservation([FromRoute] int reservationId)
        {
            try
            {
                var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy driverId từ JWT 
                if (string.IsNullOrEmpty(driverId)) return Unauthorized();

                var result = await _reservationService.CancelReservationAsync(reservationId, driverId);
                return Ok(new
                {
                    message = "Huỷ đặt chỗ thành công.",
                    data = result
                });

            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách các lịch đặt chỗ SẮP TỚI (Confirmed) của tài xế đang đăng nhập.
        /// </summary>
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcomingReservations()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(driverId)) return Unauthorized();
            try
            {
                var reservations = await _reservationService.GetUpcomingReservationsByDriverAsync(driverId);
                return Ok(reservations);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lấy LỊCH SỬ đặt chỗ (Completed, Cancelled, Expired...) của tài xế.
        /// </summary>
        [HttpGet("history")]
        public async Task<IActionResult> GetReservationHistory()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(driverId))
            {
                return Unauthorized();
            }

            try
            {
                var reservations = await _reservationService.GetReservationHistoryByDriverAsync(driverId);
                return Ok(reservations);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin CHI TIẾT của một lịch đặt cụ thể.
        /// </summary>
        [HttpGet("{reservationId:int}")]
        public async Task<IActionResult> GetReservationById([FromRoute] int reservationId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(driverId))
            {
                return Unauthorized();
            }

            try
            {
                var reservation = await _reservationService.GetReservationDetailsAsync(reservationId);

                // Bảo mật: Đảm bảo tài xế này sở hữu lịch đặt trước khi trả về
                if (reservation.DriverId != driverId)
                {
                    return Forbid(); // Trả về 403 Forbidden
                }

                return Ok(reservation);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(new { message = e.Message });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}