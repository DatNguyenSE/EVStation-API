using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs.ChargingPackage;
using API.Helpers;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;



namespace API.Controllers
{
    [Route("api/charging-package")]
    [ApiController]
    public class ChargingPackageController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IPackageService _packageService;

        public ChargingPackageController(IUnitOfWork uow, IPackageService packageService)
        {
            _uow = uow;
            _packageService = packageService;
        }

        [HttpGet]
        [Authorize(Roles = $"{AppConstant.Roles.Operator}, {AppConstant.Roles.Manager}, {AppConstant.Roles.Technician}, {AppConstant.Roles.Admin}")]
        public async Task<IActionResult> GetAll()
        {
            var packages = await _uow.ChargingPackages.GetAllAsync();
            var packageDtos = packages.Select(p => p.ToPackageDto()).ToList();
            return Ok(packageDtos);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = $"{AppConstant.Roles.Operator}, {AppConstant.Roles.Manager}, {AppConstant.Roles.Technician}, {AppConstant.Roles.Admin}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var packageModel = await _uow.ChargingPackages.GetByIdAsync(id);
            if (packageModel == null)
            {
                return NotFound();
            }
            return Ok(packageModel.ToPackageDto());
        }

        [HttpPost]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateChargingPackageDto packageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var packageModel = packageDto.ToPackageFromCreateDto();

                // Sử dụng Repository để tạo (đã bao gồm validation)
                await _uow.ChargingPackages.CreateAsync(packageModel);

                if (!await _uow.Complete())
                {
                    // Lỗi xảy ra khi lưu vào database (DB Error)
                    return StatusCode(500, "Lỗi Server: Không thể lưu gói sạc vào cơ sở dữ liệu.");
                }

                // Trả về 201 Created
                return CreatedAtAction(nameof(GetById), new { id = packageModel.Id }, packageModel.ToPackageDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateChargingPackageDto packageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var packageModel = await _uow.ChargingPackages.UpdateAsync(id, packageDto);

                if (packageModel == null)
                {
                    return NotFound($"Không tìm thấy gói sạc với ID: {id}");
                }

                if (!await _uow.Complete())
                {
                    return StatusCode(500, "Lỗi Server: Không thể cập nhật gói sạc trong cơ sở dữ liệu.");
                }

                return Ok(packageModel.ToPackageDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> UpdateStatus([FromRoute] int id, [FromBody] UpdateChargingPackageStatusDto packageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var packageModel = await _uow.ChargingPackages.UpdateStatusAsync(id, packageDto);
            if (packageModel == null)
            {
                return NotFound();
            }
            if (!await _uow.Complete())
                return StatusCode(500, "Không thể thay đổi trạng thái gói sạc.");

            return Ok(packageModel.ToPackageDto());
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var packageModel = await _uow.ChargingPackages.DeleteAsync(id);
            if (packageModel == null)
            {
                return NotFound();
            }
            if (!await _uow.Complete())
                return StatusCode(500, "Không thể xóa gói sạc.");

            return NoContent();
        }

        [HttpPost("purchase/{packageId}")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        public async Task<IActionResult> PurchasePackage(int packageId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var (Success, Message) = await _packageService.PurchasePackageAsync(userId, packageId);

            if (!Success)
            {
                return BadRequest(new { message = Message });
            }

            return Ok(new { message = Message });
        }

        [HttpGet("available")]
        [Authorize]
        public async Task<IActionResult> GetAvailable()
        {
            // Gọi thẳng đến service để lấy và xử lý dữ liệu
            var packageDtos = await _packageService.GetAvailablePackagesAsync();

            // Trả về 200 OK
            return Ok(packageDtos);
        }
    }
}