using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/vehiclemodels")]
    [ApiController]
    public class VehicleModelController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public VehicleModelController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetVehicleModelDetails([FromQuery] string modelName)
        {
            var model = await _uow.VehicleModels.GetByModelNameAsync(modelName);
            if (model == null) return NotFound("Model không tồn tại");

            double maxPower;

            if (model.Type == VehicleType.Car)
            {
                // Nếu là xe hơi → dùng công suất DC
                maxPower = model.MaxChargingPowerDC_KW ?? 0;
            }
            else
            {
                // Nếu là xe máy → dùng công suất thường (AC)
                maxPower = model.MaxChargingPowerKW ?? 0;
            }
            
            return Ok(new
            {
                model.Model,
                model.Type,
                model.BatteryCapacityKWh,
                MaxChargingPowerKW = maxPower,
                model.ConnectorType
            });
        }
    }
}