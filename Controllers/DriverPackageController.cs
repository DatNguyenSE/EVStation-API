using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs.DriverPackage;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/userpackage")]
    [ApiController]
    public class DriverPackageController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public DriverPackageController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> GetAll()
        {
            var userPackageModels = await _uow.DriverPackages.GetAllAsync();
            var userPackageDtos = userPackageModels.Select(up => up.ToDriverPackageDto()).ToList();
            return Ok(userPackageDtos);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> GetById(int id)
        {
            var userPackageModel = await _uow.DriverPackages.GetByIdAsync(id);
            if (userPackageModel == null)
            {
                return NotFound("Không tìm thấy.");
            }
            return Ok(userPackageModel);
        }

        // driver tự xem gói của mình
        [HttpGet("my-packages")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        public async Task<IActionResult> GetByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var userPackageModels = await _uow.DriverPackages.GetByUserAsync(userId);
            var userPackageDtos = userPackageModels.Select(up => up.ToUserPackageViewDto()).ToList();
            return Ok(userPackageDtos);
        }

        // Admin xem gói của user khác
        [HttpGet("{userId}")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var userPackageModels = await _uow.DriverPackages.GetByUserAsync(userId);
            var userPackageDtos = userPackageModels.Select(up => up.ToUserPackageViewDto()).ToList();
            return Ok(userPackageDtos);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> Deactive(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userPackageModel = await _uow.DriverPackages.DeactiveAsync(id);
            if (userPackageModel == null)
            {
                return NotFound("Không tìm thấy gói.");
            }
            
            var result = await _uow.Complete();
            if (!result)
                return BadRequest("Huỷ gói lỗi.");

            return NoContent();
        }
    }
}