using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Helpers;
using API.Helpers.Enums;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/charging-sessions")]
    public class ChargingSessionsController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IChargingSessionService _service;

        public ChargingSessionsController(IUnitOfWork uow, IChargingSessionService service)
        {
            _uow = uow;
            _service = service;
        }

        // API tạo session: POST api/chargingsessions
        [HttpPost("start")]
        public async Task<ActionResult<ChargingSessionDto>> CreateSession(CreateChargingSessionDto dto)
        {
            try
            {
                var session = await _service.CreateSessionAsync(dto);
                return Ok(session);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{sessionId}/stop")]
        public async Task<IActionResult> Stop(int sessionId)
        {
            try
            {
                await _service.StopChargingAsync(sessionId, StopReason.ManualStop);
                return Ok();
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{sessionId}/complete")]
        public async Task<IActionResult> Complete(int sessionId)
        {
            try
            {
                var receipt = await _service.CompleteSessionAsync(sessionId);
                return Ok(receipt);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{sessionId}/update-plate")]
        public async Task<IActionResult> UpdatePlate(int sessionId, [FromBody] UpdatePlateRequest vehiclePlate)
        {
            try
            {
                var s = await _service.UpdatePlateAsync(sessionId, vehiclePlate.Plate);
                return Ok(s);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistorySession()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var historySessions = await _uow.ChargingSessions.GetSessionsByDriverAsync(userId);
            return Ok(historySessions);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> GetDetailHistorySession(int sessionId)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                return NotFound("Không tìm thấy phiên sạc này");
            }

            return Ok(session.MapToDetailDto());
        }
    }
}