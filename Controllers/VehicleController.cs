using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Vehicle;
using API.Entities;
using API.Entities.Cloudinary;
using API.Extensions;
using API.Helpers;
using API.Helpers.Enums;
using API.Interfaces;
using API.Mappers;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace API.Controllers
{
    [Route("api/vehicle")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VehicleController(IUnitOfWork uow, UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _uow = uow;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("add")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        public async Task<IActionResult> AddVehicle([FromForm] AddVehicleRequestDto dto, [FromServices] IOptions<CloudinarySettings> cloudinaryConfig)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null)
            {
                return Unauthorized();
            }

            var existingVehicle = await _uow.Vehicles.GetByPlateAsync(dto.Plate);

            if (existingVehicle != null)
            {
                if (existingVehicle.RegistrationStatus == VehicleRegistrationStatus.Approved)
                {
                    return BadRequest("Xe đã tồn tại và đã được đăng ký sở hữu.");
                }

                string frontUrl = existingVehicle.VehicleRegistrationFrontUrl ?? "";
                string backUrl = existingVehicle.VehicleRegistrationBackUrl ?? "";

                if (string.IsNullOrEmpty(frontUrl) || string.IsNullOrEmpty(backUrl))
                {
                    try
                    {
                        var settings = cloudinaryConfig.Value;
                        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
                        var cloudinary = new Cloudinary(account);

                        // --- Upload mặt trước ---
                        using var streamFront = dto.RegistrationImageFront.OpenReadStream();
                        var uploadFront = await cloudinary.UploadAsync(new ImageUploadParams
                        {
                            File = new FileDescription(dto.RegistrationImageFront.FileName, streamFront),
                            Folder = "evms/vehicles"
                        });
                        frontUrl = uploadFront.SecureUrl.ToString();

                        // --- Upload mặt sau ---
                        using var streamBack = dto.RegistrationImageBack.OpenReadStream();
                        var uploadBack = await cloudinary.UploadAsync(new ImageUploadParams
                        {
                            File = new FileDescription(dto.RegistrationImageBack.FileName, streamBack),
                            Folder = "evms/vehicles"
                        });
                        backUrl = uploadBack.SecureUrl.ToString();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return StatusCode(500, "Lỗi xảy ra khi tải ảnh lên Cloudinary.");
                    }
                }

                existingVehicle.OwnerId = appUser.Id;
                existingVehicle.VehicleRegistrationFrontUrl = frontUrl; // Cập nhật (hoặc giữ nguyên nếu đã có)
                existingVehicle.VehicleRegistrationBackUrl = backUrl;   // Cập nhật (hoặc giữ nguyên nếu đã có)
                existingVehicle.RegistrationStatus = VehicleRegistrationStatus.Pending; // Đưa về trạng thái chờ duyệt

                var res = await _uow.Complete();
                if (!res) return BadRequest("Cập nhật thông tin xe thất bại");

                return Ok(new { message = "Đăng ký sở hữu xe thành công, vui lòng chờ duyệt.", vehicle = existingVehicle });
            }
            else
            {
                string frontUrl = "", backUrl = "";
                try
                {
                    var settings = cloudinaryConfig.Value;
                    var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
                    var cloudinary = new Cloudinary(account);

                    // --- Upload mặt trước ---
                    using var streamFront = dto.RegistrationImageFront.OpenReadStream();
                    var uploadFront = await cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(dto.RegistrationImageFront.FileName, streamFront),
                        Folder = "evms/vehicles"
                    });
                    frontUrl = uploadFront.SecureUrl.ToString();

                    // --- Upload mặt sau ---
                    using var streamBack = dto.RegistrationImageBack.OpenReadStream();
                    var uploadBack = await cloudinary.UploadAsync(new ImageUploadParams
                    {
                        File = new FileDescription(dto.RegistrationImageBack.FileName, streamBack),
                        Folder = "evms/vehicles"
                    });
                    backUrl = uploadBack.SecureUrl.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return StatusCode(500, "Lỗi xảy ra khi tải ảnh lên Cloudinary.");
                }

                var vehicle = new Vehicle
                {
                    Model = dto.Model,
                    Type = dto.Type,
                    BatteryCapacityKWh = dto.BatteryCapacityKWh,
                    MaxChargingPowerKW = dto.MaxChargingPowerKW,
                    ConnectorType = dto.ConnectorType,
                    Plate = dto.Plate,
                    OwnerId = appUser.Id,
                    VehicleRegistrationFrontUrl = frontUrl,
                    VehicleRegistrationBackUrl = backUrl,
                    RegistrationStatus = VehicleRegistrationStatus.Pending
                };

                var created = await _uow.Vehicles.AddVehicleAsync(vehicle);

                var result = await _uow.Complete();
                if (!result) return BadRequest("Thêm xe thất bại");

                return Ok(new { message = "Đăng ký xe thành công, vui lòng chờ duyệt.", vehicle });
            }
        }

        // lấy thông tin xe của User
        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> GetMyVehicles()
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            if (appUser == null)
            {
                return Unauthorized();
            }

            var vehicles = await _uow.Vehicles.GetVehiclesByUserAsync(appUser.Id);

            if (!vehicles.Any())
                return Ok(new List<VehicleResponseDto>()); // hoặc trả thông báo rỗng

            // Map sang DTO
            var result = vehicles.Select(v => v.ToVehicleResponseDto());

            return Ok(result);
        }

        [HttpPost("info/{id}")]
        [Authorize]
        public async Task<IActionResult> GetMyVehicleInfoById(int id)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            if (appUser == null)
            {
                return Unauthorized();
            }

            var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(id);

            if (vehicle == null)
                return NotFound(new { message = "Vehicle not found for this user." });
            return Ok(vehicle.ToVehicleResponseDto());
        }

        [HttpGet("my-approved")]
        [Authorize]
        public async Task<IActionResult> GetMyVehiclesApproved()
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);

            if (appUser == null)
            {
                return Unauthorized();
            }

            var vehicles = await _uow.Vehicles.GetVehiclesApprovedByUserAsync(appUser.Id);

            if (!vehicles.Any())
                return Ok(new List<VehicleResponseDto>()); // hoặc trả thông báo rỗng

            // Map sang DTO
            var result = vehicles.Select(v => v.ToVehicleResponseDto());

            return Ok(result);
        }

        [HttpGet("models")]
        [Authorize]
        public async Task<IActionResult> GetVehicleModels([FromQuery] VehicleType vehicleType)
        {
            var vehicleModelObjects = await _uow.VehicleModels.GetByTypeAsync(vehicleType);

            // Chỉ chọn ra thuộc tính 'Model'
            var modelNames = vehicleModelObjects.Select(vm => vm.Model);

            return Ok(modelNames);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        public async Task<IActionResult> UpdateVehicle([FromRoute] int id, [FromBody] VehicleUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized();

            var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(id);
            if (vehicle == null || vehicle.OwnerId != appUser.Id)
            {
                return NotFound("Không tìm thấy xe hoặc bạn không có quyền.");
            }

            // Kiểm tra trùng biển số
            if (await _uow.Vehicles.PlateExistsAsync(dto.Plate, id))
            {
                return BadRequest("Biển số xe đã tồn tại.");
            }

            // Cập nhật thông tin
            vehicle.Model = dto.Model;
            vehicle.Type = dto.Type;
            vehicle.BatteryCapacityKWh = dto.BatteryCapacityKWh;
            vehicle.MaxChargingPowerKW = dto.MaxChargingPowerKW;
            vehicle.ConnectorType = dto.ConnectorType;
            vehicle.Plate = dto.Plate;

            await _uow.Vehicles.UpdateVehicleAsync(vehicle);
            var result = await _uow.Complete();
            if (!result) return BadRequest("Cập nhật xe thất bại.");

            return Ok(new
            {
                message = "Cập nhật xe thành công",
                vehicle = vehicle.ToVehicleResponseDto()
            });
        }

        [HttpDelete("my/delete/{id}")]
        [Authorize(Roles = AppConstant.Roles.Driver)]
        public async Task<IActionResult> DeactivateVehicle([FromRoute] int id)
        {
            var username = User.GetUsername();
            var appUser = await _userManager.FindByNameAsync(username);
            if (appUser == null) return Unauthorized();

            var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(id);
            if (vehicle == null)
            {
                return NotFound("Không tìm thấy xe");
            }

            if (vehicle.OwnerId != appUser.Id)
            {
                return Forbid("Bạn không có quyền thao tác trên xe này.");
            }

            if (!vehicle.IsActive)
            {
                return BadRequest("Xe này đã bị vô hiệu hóa trước đó.");
            }

            await _uow.Vehicles.DeactivateVehicleAsync(vehicle);
            var result = await _uow.Complete();
            if (!result) return BadRequest("Vô hiệu hóa xe thất bại.");
            return Ok(new { message = "Xe đã được vô hiệu hóa (inactive)." });
        }

        /**
         * [ADMIN] Lấy danh sách tất cả xe đang chờ duyệt
         */
        [HttpGet("pending")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> GetPendingVehicles()
        {
            var pendingVehicles = await _uow.Vehicles.GetPendingVehiclesWithOwnersAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // Chuyển đổi sang DTO để trả về cho Admin
            var dtos = pendingVehicles.Select(v => new PendingVehicleDto
            {
                VehicleId = v.Id,
                Model = v.Model,
                Plate = v.Plate,
                VehicleType = v.Type.ToString(),
                OwnerName = v.Owner?.UserName ?? "N/A",
                OwnerEmail = v.Owner?.Email ?? "N/A",
                // Tạo URL tuyệt đối để Admin có thể xem ảnh
                RegistrationImageFrontUrl = string.IsNullOrEmpty(v.VehicleRegistrationFrontUrl)
                                        ? null
                                        : $"{v.VehicleRegistrationFrontUrl}",
                RegistrationImageBackUrl = string.IsNullOrEmpty(v.VehicleRegistrationBackUrl)
                                        ? null
                                        : $"{v.VehicleRegistrationBackUrl}",
            });

            return Ok(dtos);
        }

        /**
        * [ADMIN] Phê duyệt một xe
        */
        [HttpPost("{vehicleId:int}/approve")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> ApproveVehicle(int vehicleId)
        {
            var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return NotFound("Không tìm thấy xe.");
            }

            if (vehicle.RegistrationStatus != VehicleRegistrationStatus.Pending)
            {
                return BadRequest("Xe này không ở trạng thái chờ duyệt.");
            }

            // Cập nhật trạng thái
            vehicle.RegistrationStatus = VehicleRegistrationStatus.Approved;

            var receipts = await _uow.Receipts.GetReceiptsByPlateAsync(vehicle.Plate);
            if (receipts.Any())
            {
                foreach (var receipt in receipts)
                {
                    receipt.AppUserId = vehicle.OwnerId;
                    _uow.Receipts.Update(receipt);
                }
            }

            if (await _uow.Complete())
            {
                // (Nâng cao): Gửi thông báo SignalR cho chủ xe (vehicle.OwnerId)
                return Ok(new { message = "Đã duyệt xe thành công." });
            }

            return StatusCode(500, "Lỗi hệ thống khi cập nhật trạng thái.");
        }

        /**
         * [ADMIN] Từ chối một xe (không cần lý do)
         */
        [HttpPost("{vehicleId:int}/reject")]
        [Authorize(Roles = AppConstant.Roles.Admin)]
        public async Task<IActionResult> RejectVehicle(int vehicleId)
        {
            var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null)
            {
                return NotFound("Không tìm thấy xe.");
            }

            if (vehicle.RegistrationStatus != VehicleRegistrationStatus.Pending)
            {
                return BadRequest("Xe này không ở trạng thái chờ duyệt.");
            }

            // Cập nhật trạng thái
            vehicle.RegistrationStatus = VehicleRegistrationStatus.Rejected;

            if (await _uow.Complete())
            {
                // (Nâng cao): Gửi thông báo SignalR cho chủ xe (vehicle.OwnerId)
                return Ok(new { message = "Đã từ chối xe thành công." });
            }

            return StatusCode(500, "Lỗi hệ thống khi cập nhật trạng thái.");
        }
    }
}