using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingPost;
using API.Entities;

namespace API.Interfaces
{
    public interface IChargingPostRepository
    {
        Task<List<ChargingPost>> GetAllAsync();
        Task<ChargingPost?> GetByIdAsync(int id);
        Task<ChargingPost> CreateAsync(int stationId, ChargingPost postModel);
        Task<ChargingPost?> UpdateAsync(int id, UpdateChargingPostDto postDto);
        Task<ChargingPost?> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<ChargingPost?> UpdateStatusAsync(int id, Helpers.Enums.PostStatus status);
    }
}