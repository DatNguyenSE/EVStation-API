using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Helpers;
using API.Helpers.Enums;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/charging-sessions")]
    public class ChargingSessionsController : ControllerBase
    {
        private readonly IChargingSessionService _service;

        public ChargingSessionsController(IChargingSessionService service)
        {
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
    }
}