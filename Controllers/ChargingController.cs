using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/charging")]
    [Authorize(Roles = AppConstant.Roles.Driver)]
    public class ChargingController : ControllerBase
    {
        private readonly IChargingService _chargingService;
        public ChargingController(IChargingService chargingService)
        {
            _chargingService = chargingService;
        }

        [HttpPost("validate-scan")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateScan([FromQuery] int postId)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var (canStart, message, reservationId, vehicleId) = await _chargingService.ValidateScanAsync(postId, driverId);

            if (!canStart)
            {
                // Dùng Conflict (409) cho các lỗi chung như trụ bận, sai giờ...
                return Conflict(new { Message = message });
            }

            return Ok(new 
            { 
                Message = message, 
                PostId = postId,
                ReservationId = reservationId, 
                VehicleId = vehicleId
            });
        }
    }
}