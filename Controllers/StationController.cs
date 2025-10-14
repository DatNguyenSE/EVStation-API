using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Station;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.Identity.Client;

namespace API.Controllers
{
    [Route("api/station")]
    [ApiController]
    public class StationController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public StationController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll()
        {
            var stations = await _uow.Stations.GetAllAsync();
            var stationDtos = stations.Select(s => s.ToStationDto()).ToList();
            return Ok(stationDtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var station = await _uow.Stations.GetByIdAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            return Ok(station.ToStationDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStationDto stationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stationModel = stationDto.ToStationFromCreateDto();
            await _uow.Stations.CreateAsync(stationModel);
            // var result = await _uow.Complete();
            // if (!result) return BadRequest("Tạo trạm thất bại");
            return CreatedAtAction(nameof(GetById), new { id = stationModel.Id }, stationModel.ToStationDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStationDto stationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stationModel = await _uow.Stations.UpdateAsync(id, stationDto);
            if (stationModel == null)
            {
                return NotFound();
            }
            var result = await _uow.Complete();
            if (!result) return BadRequest("Cập nhật trạm thất bại");
            return Ok(stationModel.ToStationDto());
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] StationStatus status)
        {
            var stationModel = await _uow.Stations.UpdateStatusAsync(id, status);
            if (stationModel == null)
            {
                return NotFound();
            }
            var result = await _uow.Complete();
            if (!result) return BadRequest("Cập nhật trạm thất bại");
            return Ok(stationModel.ToStationDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var stationModel = await _uow.Stations.DeleteAsync(id);
            if (stationModel == null)
            {
                return NotFound();
            }
            var result = await _uow.Complete();
            if (!result) return BadRequest("Xoá trạm thất bại");
            return NoContent();
        }

        // API gợi ý trạm gần nhất
        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearest([FromQuery] double lat, [FromQuery] double lon)
        {
            var station = await _uow.Stations.GetNearestAsync(lat, lon);

            // Nếu không tìm thấy trạm nào (CSDL rỗng), trả về lỗi 404 Not Found
            if (station == null)
            {
                return NotFound("Không tìm thấy trạm nào.");
            }
            var stationDto = station.ToStationDto();
            return Ok(stationDto);
        }

        // API tìm kiếm trạm
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string address)
        {
            var stations = await _uow.Stations.SearchAsync(address);
            return Ok(stations.Select(s => s.ToStationDto()));
        }

        // trả về danh sách các trụ sạc tương thích với xe trong trạm
        [HttpGet("{stationId:int}/compatible-posts/{vehicleId:int}")]
        public async Task<IActionResult> GetCompatiblePosts(int stationId, int vehicleId) 
        {
            var station = await _uow.Stations.GetByIdAsync(stationId);
            if (station == null)
            {
                return NotFound("Không tìm thấy trạm.");
            }

            var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return NotFound("Không tìm thấy xe.");
            }

            var posts = station.Posts;

            // Nếu là xe ô tô và loại sạc CCS2
            if (vehicle.Type == VehicleType.Car && vehicle.ConnectorType == ConnectorType.CCS2)
            {
                posts = posts
                    .Where(p => p.ConnectorType == ConnectorType.CCS2 || p.ConnectorType == ConnectorType.Type2)
                    .ToList();
            }
            else if (vehicle.Type == VehicleType.Motorbike && vehicle.ConnectorType == ConnectorType.VinEScooter)
            {
                posts = posts
                    .Where(p => p.ConnectorType == ConnectorType.VinEScooter)
                    .ToList();
            }
            else
            {
                posts = new List<ChargingPost>();
            }

            return Ok(posts.Select(p => p.ToPostDto()));
        }
    }   
}