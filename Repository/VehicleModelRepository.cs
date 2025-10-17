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

        public async Task<IEnumerable<VehicleModel>> GetCompatibleModelsAsync(ConnectorType connectorType)
        {
            return await _context.VehicleModels
                .Where(vm => vm.ConnectorType == connectorType)
                .ToListAsync();
        }
    }
}