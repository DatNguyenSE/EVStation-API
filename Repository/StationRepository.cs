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

            Console.WriteLine($"Before Save: OpenTime={stationModel.OpenTime}, CloseTime={stationModel.CloseTime}");

            // Convert 24:00:00 -> 00:00:00 (SQL không nhận 24h)
            if (stationModel.CloseTime.TotalHours >= 24)
                stationModel.CloseTime = TimeSpan.Zero;
            if (stationModel.OpenTime.TotalHours >= 24)
                stationModel.OpenTime = TimeSpan.Zero;

            _context.Entry(stationModel).State = EntityState.Detached;

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
            return await _context.Stations.Include(s => s.Posts).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Station?> GetNearestAsync(double latitude, double longitude)
        {
            var stations = await _context.Stations.ToListAsync();

            // Xử lý trường hợp không có trạm nào trong cơ sở dữ liệu
            if (!stations.Any())
            {
                return null;
            }


            // Sắp xếp các trạm theo khoảng cách tăng dần và chọn cái đầu tiên
            var nearestStation = stations
                .OrderBy(s => StationCodeHelper.GetDistanceKm(latitude, longitude, s.Latitude, s.Longitude))
                .FirstOrDefault();

            return nearestStation;
        }

        public async Task<List<Station>> SearchAsync(string keyword)
        {
            // Nên dùng IsNullOrWhiteSpace để kiểm tra cả trường hợp chuỗi chỉ có khoảng trắng
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return new List<Station>();
            }

            var search = keyword.Trim().ToLower();

            // Tìm kiếm trong database
            var stations = await _context.Stations
                .Where(s => s.Address.ToLower().Contains(search) ||
                            s.Name.ToLower().Contains(search))
                .ToListAsync();

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
            // Convert 24:00:00 -> 00:00:00 (SQL không nhận 24h)
            if (stationDto.OpenTime.HasValue)
            {
                if (stationDto.OpenTime.Value.TotalHours == 24)
                {
                    stationModel.OpenTime = TimeSpan.Zero;
                }
                else
                {
                    stationModel.OpenTime = stationDto.OpenTime.Value;
                }
            }   
            if (stationDto.CloseTime.HasValue)
            {
                if (stationDto.CloseTime.Value.TotalHours == 24)
                {
                    stationModel.CloseTime = TimeSpan.Zero;
                }
                else
                {
                    stationModel.CloseTime = stationDto.CloseTime.Value;
                }
            }                
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