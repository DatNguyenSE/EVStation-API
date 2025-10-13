using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Station;
using API.Entities;
using API.Helpers;
using API.Helpers.Enums;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class StationRepository : IStationRepository
    {
        private readonly AppDbContext _context;
        private readonly IChargingPostRepository _postRepo;
        public StationRepository(AppDbContext context, IChargingPostRepository postRepo)
        {
            _context = context;
            _postRepo = postRepo;
        }

        public async Task<Station> CreateAsync(Station stationModel)
        {
            // Tách danh sách post ra tạm, tránh EF tracking post khi lưu station
            var posts = stationModel.Posts.ToList();
            stationModel.Posts.Clear();

            // lưu station để có id
            await _context.Stations.AddAsync(stationModel);
            await _context.SaveChangesAsync();
            // generate code cho station
            stationModel.Code = StationCodeHelper.GenerateStationCode(stationModel.Address, stationModel.Id);
            _context.Stations.Update(stationModel); // đánh dấu lại là Modified cho EF Core
            await _context.SaveChangesAsync();

            // Tạo từng post qua repo (chỉ chạy logic create 1 lần)
            foreach (var post in posts)
            {
                await _postRepo.CreateAsync(stationModel.Id, post);
            }

            return stationModel;
        }

        public async Task<Station?> DeleteAsync(int id)
        {
            var stationModel = await _context.Stations.FindAsync(id);
            if (stationModel == null)
            {
                return null;
            }
            _context.Stations.Remove(stationModel);

            return stationModel;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Stations.AnyAsync(s => s.Id == id);
        }

        public async Task<List<Station>> GetAllAsync()
        {
            return await _context.Stations.ToListAsync();
        }

        public async Task<Station?> GetByIdAsync(int id)
        {
            return await _context.Stations.FindAsync(id);
        }

        public async Task<List<Station>> GetNearbyAsync(double latitude, double longitude, double radiusKm)
        {
            var stations = await _context.Stations.ToListAsync();

            return stations
                .Where(s => StationCodeHelper.GetDistanceKm(latitude, longitude, s.Latitude, s.Longitude) <= radiusKm)
                .ToList();
        }

        public async Task<List<Station>> SearchByAddressAsync(string address)
        {
            // Kiểm tra input rỗng
            if (string.IsNullOrEmpty(address))
            {
                return new List<Station>();
            }

            var search = address.Trim().ToLower();

            // Tìm kiếm trong database
            var stations = await _context.Stations
                .Where(s => s.Address.ToLower().Contains(search)).ToListAsync();

            return stations;
        }

        public async Task<Station?> UpdateAsync(int id, UpdateStationDto stationDto)
        {
            var stationModel = await _context.Stations.FindAsync(id);
            if (stationModel == null)
            {
                return null;
            }
            if (stationDto.Name != null)
                stationModel.Name = stationDto.Name;
            if (stationDto.Description != null)
                stationModel.Description = stationDto.Description;
            if (stationDto.OpenTime.HasValue)
                stationModel.OpenTime = stationDto.OpenTime.Value;
            if (stationDto.CloseTime.HasValue)
                stationModel.CloseTime = stationDto.CloseTime.Value;
            if (stationDto.Status.HasValue)
                stationModel.Status = stationDto.Status.Value;
            return stationModel;
        }

        public async Task<Station?> UpdateStatusAsync(int id, StationStatus status)
        {
            var station = await _context.Stations
                .Include(s => s.Posts) // load luôn các posts
                .FirstOrDefaultAsync(s => s.Id == id);

            if (station == null)
            {
                return null;
            }

            station.Status = status;

            // đổi trạng thái trạm thì đổi trụ luôn
            foreach (var post in station.Posts)
            {
                if (status == StationStatus.Maintenance)
                {
                    post.Status = Helpers.Enums.PostStatus.Maintenance;
                }
                else if (status == StationStatus.Inactive)
                {
                    post.Status = Helpers.Enums.PostStatus.Offline;
                }
                else
                {
                    post.Status = Helpers.Enums.PostStatus.Available;
                }
            }

            return station;
        }
    }
}