using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
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
                var receipt = await _service.CompleteSessionAsync(sessionId, true);
                return Ok(receipt);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPost("{sessionId}/update-plate")]
        [Authorize(Roles = AppConstant.Roles.Operator)]
        public async Task<IActionResult> UpdatePlate(int sessionId, [FromBody] UpdatePlateRequest vehiclePlate)
        {
            try
            {
                var s = await _service.UpdatePlateAsync(sessionId, vehiclePlate.Plate);
                return Ok(s);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("{sessionId}/reconnect")]
        public async Task<IActionResult> ReconnectSession(int sessionId)
        {
            try
            {
                var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);

                if (session == null)
                    return NotFound("Không tìm thấy phiên sạc");

                // Chỉ cho phép reconnect nếu session đang Charging hoặc Idle
                if (session.Status != SessionStatus.Charging && session.Status != SessionStatus.Idle)
                    return BadRequest("Phiên sạc đã kết thúc");

                var post = await _uow.ChargingPosts.GetByIdAsync(session.ChargingPostId);
                var station = await _uow.Stations.GetByIdAsync(post!.StationId);

                var response = new ReconnectSessionDto
                {
                    SessionId = session.Id,
                    PostId = session.ChargingPostId,
                    StationId = post.StationId,
                    StationName = station!.Name,
                    StationAddress = station.Address,
                    PostInfo = new PostDto
                    {
                        Id = post.Id,
                        Type = post.Type.ToString(),
                        PowerKW = post.PowerKW,
                        ConnectorType = post.ConnectorType.ToString(),
                        Status = post.Status.ToString()
                    },
                    VehicleInfo = session.VehicleId.HasValue ? new VehicleInfoDto
                    {
                        Plate = session.VehiclePlate,
                        Model = session.Vehicle?.Model ?? string.Empty,
                        BatteryCapacityKWh = session.Vehicle?.BatteryCapacityKWh ?? 0
                    } : null,
                    CurrentState = new SessionStateDto
                    {
                        BatteryPercent = (double)(session.EndBatteryPercentage ?? session.StartBatteryPercentage),
                        ChargedKwh = session.EnergyConsumed,
                        TotalPrice = session.Cost,
                        Status = session.Status.ToString()
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("history")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        public async Task<IActionResult> GetHistorySessions([FromQuery] PagingParams pagingParams)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var pagedSessions = await _uow.ChargingSessions.GetSessionsByDriverAsync(userId, pagingParams);
            var pagination = new
            {
                currentPage = pagingParams.PageNumber,
                pageSize = pagingParams.PageSize,
                totalCount = pagedSessions.TotalItemCount,
                totalPages = pagedSessions.PageCount
            };
            return Ok(new { sessions = pagedSessions.ToList(), pagination });
        }

        [HttpGet("{sessionId}/detail")]
        [Authorize(Roles = $"{AppConstant.Roles.Driver}, {AppConstant.Roles.Manager}, {AppConstant.Roles.Operator}, {AppConstant.Roles.Admin}")]
        public async Task<IActionResult> GetDetailHistorySession(int sessionId)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null)
            {
                return NotFound("Không tìm thấy phiên sạc này");
            }

            return Ok(session.MapToDetailDto());
        }

        [HttpGet("all")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> GetAllSession()
        {
            var sessions = await _uow.ChargingSessions.GetAllAsync();
            if (sessions == null)
            {
                return NotFound("Không tìm thấy phiên sạc này");
            }

            List<ChargingSessionDto> listSession = new();
            foreach (var session in sessions)
            {
                listSession.Add(session.MapToDto());
            }

            return Ok(listSession);
        }

        [HttpGet("by-station/{stationId}")]
        [Authorize(Roles = $"{AppConstant.Roles.Manager}, {AppConstant.Roles.Operator}, {AppConstant.Roles.Admin}")]
        public async Task<IActionResult> GetSessionsByStation(int stationId)
        {
            var staffId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(staffId)) return Unauthorized();

            bool isAdmin = User.IsInRole(AppConstant.Roles.Admin);

            int assignedStationId = 0;

            if (!isAdmin)
            {
                var assignment = await _uow.Assignments.GetCurrentAssignmentAsync(staffId);

                if (assignment == null)
                {
                    return NotFound("Bạn không có phân công đang hoạt động.");
                }

                assignedStationId = assignment.StationId;

                if (assignedStationId != stationId)
                {
                    return StatusCode(403, new
                    {
                        error = "Forbidden",
                        message = $"Bạn chỉ được phép xem dữ liệu của trạm ID: {assignedStationId}.",
                        requiredStationId = assignedStationId
                    });
                }
            }

            var sessions = await _uow.ChargingSessions.GetSessionsByStationAsync(stationId);

            if (sessions == null || sessions.Count == 0)
            {
                return Ok(new List<ChargingSessionHistoryDto>());
            }

            return Ok(sessions);
        }
    }
}