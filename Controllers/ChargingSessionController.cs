using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  // Route: api/chargingsessions
    public class ChargingSessionsController : ControllerBase
    {
        private readonly IChargingSessionService _service;

        public ChargingSessionsController(IChargingSessionService service)
        {
            _service = service;
        }

        // API tạo session: POST api/chargingsessions
        [HttpPost]
        public async Task<ActionResult<ChargingSessionDto>> CreateSession(CreateChargingSessionDto dto)
        {
            try
            {
                var session = await _service.CreateSessionAsync(dto);
                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{sessionId}/update-plate")]
        public async Task<IActionResult> UpdatePlate (int sessionId, [FromBody] UpdatePlateRequest vehiclePlate)
        {
            try
            {
                var session = await _service.UpdatePlateAsync(sessionId, vehiclePlate.Plate);
                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // API kết thúc session: POST api/chargingsessions/{sessionId}/end
        [HttpPost("{sessionId}/end")]
        public async Task<ActionResult<ChargingSessionDto>> EndSession(int sessionId)
        {
            try
            {
                var session = await _service.EndSessionAsync(sessionId);
                return Ok(session);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}