using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Enums;

namespace API.Interfaces
{
    public interface IVehicleModelRepository
    {
        Task<List<VehicleModel>> GetByTypeAsync(VehicleType vehicleType);
        Task<VehicleModel?> GetByModelNameAsync(string vehicleModel);
        Task<IEnumerable<VehicleModel>> GetCompatibleModelsAsync(ConnectorType connectorType);
    }
}