using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly AppDbContext _context;
        public VehicleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicleModel)
        {
            await _context.Vehicles.AddAsync(vehicleModel);
            return vehicleModel;
        }

        public async Task DeactivateVehicleAsync(Vehicle vehicle)
        {
            vehicle.IsActive = false;
            _context.Vehicles.Update(vehicle);
        }

        public async Task<Vehicle?> GetByPlateAsync(string plate)
        {
            return await _context.Vehicles.FirstOrDefaultAsync(v => v.Plate == plate);
        }

        public async Task<Vehicle?> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByUserAsync(string userId)
        {
            return await _context.Vehicles
                        .Where(v => v.OwnerId == userId && v.IsActive)
                        .ToListAsync();
        }

        public async Task<bool> PlateExistsAsync(string plate, int? excludeVehicleId = null)
        {
            return await _context.Vehicles
                .AnyAsync(v => v.Plate == plate && (excludeVehicleId == null || v.Id != excludeVehicleId));
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
        }
    } 
}