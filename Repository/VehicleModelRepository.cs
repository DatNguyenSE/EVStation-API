using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Helpers.Enums;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Engines;

namespace API.Repository
{
    public class VehicleModelRepository : IVehicleModelRepository
    {
        private readonly AppDbContext _context;

        public VehicleModelRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<VehicleModel?> GetByModelNameAsync(string vehicleModel)
        {
            return await _context.VehicleModels
                .FirstOrDefaultAsync(vm => vm.Model == vehicleModel);
        }

        public Task<List<VehicleModel>> GetByTypeAsync(VehicleType vehicleType)
        {
            return _context.VehicleModels.Where(vm => vm.Type == vehicleType).ToListAsync();
        }

        public async Task<List<VehicleModel>> GetCompatibleModelsAsync(ConnectorType connectorType, decimal postPowerKW)
        {
            var query = _context.VehicleModels.AsQueryable();

            if (connectorType == ConnectorType.CCS2 || connectorType == ConnectorType.Type2)
            {
                // Nếu là trụ CCS2 hoặc Type2 → chỉ lấy xe hơi có công suất sạc <= công suất trụ
                query = query.Where(vm =>
                    vm.Type == VehicleType.Car &&
                    (
                        (vm.MaxChargingPowerDC_KW != null && vm.MaxChargingPowerDC_KW <= (double)postPowerKW) ||
                        (vm.MaxChargingPowerAC_KW != null && vm.MaxChargingPowerAC_KW <= (double)postPowerKW)
                    )
                );
            }
            else if (connectorType == ConnectorType.VinEScooter)
            {
                // Nếu là trụ VinEScooter → chỉ lấy xe máy đúng loại cổng
                query = query.Where(vm =>
                    vm.Type == VehicleType.Motorbike &&
                    vm.ConnectorType == ConnectorType.VinEScooter &&
                    vm.MaxChargingPowerKW <= (double)postPowerKW
                );
            }
            return await query.ToListAsync();
        }
    }
}