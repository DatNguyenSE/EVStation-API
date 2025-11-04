using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Data;
using API.DTOs.Reservation;
using API.Entities;
using API.Interfaces;
using API.Mappers;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly AppDbContext _context;
        public ReservationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            await _context.Reservations.AddAsync(reservation);
        }

        public async Task<bool> CheckOverlapAsync(int postId, DateTime start, DateTime end)
        {
            // Logic: Hai khoảng thời gian (start, end) và (r.TimeSlotStart, r.TimeSlotEnd) trùng lặp
            // nếu start < r.TimeSlotEnd VÀ r.TimeSlotStart < end
            return await _context.Reservations.AnyAsync(r =>
                r.ChargingPostId == postId &&
                r.Status == ReservationStatus.Confirmed &&
                (
                    start < r.TimeSlotEnd && r.TimeSlotStart < end
                ));
        }

        public async Task<int> CountByDriverInDateAsync(string driverId, DateTime date)
        {
            return await _context.Reservations
                .CountAsync(r => r.DriverId == driverId &&
                                r.CreatedAt.Date == date &&
                                r.Status != ReservationStatus.Cancelled);
        }

        public async Task<Reservation?> GetActiveByPostIdAsync(int postId)
        {
            return await _context.Reservations
                .Where(r => r.ChargingPostId == postId &&
                            (r.Status == ReservationStatus.Confirmed))
                .FirstOrDefaultAsync();
        }

        public async Task<Reservation?> GetReservationByIdAsync(int id)
        {
            return await _context.Reservations.FindAsync(id);
        }

        public async Task<bool> IsTimeSlotConflictedAsync(int postId, DateTime start, DateTime end)
        {
            // Chỉ kiểm tra các đặt chỗ đã được xác nhận (Confirmed) đang chiếm slot
            return await _context.Reservations
                .AnyAsync(r => r.ChargingPostId == postId &&
                       r.Status == ReservationStatus.Confirmed &&
                       start < r.TimeSlotEnd &&
                       end > r.TimeSlotStart);
        }

        public async Task<List<Reservation>> GetReservationsForPostOnDateAsync(int postId, DateTime date)
        {
            var startOfDay = date.Date; // Lấy phần ngày, bỏ qua giờ (00:00:00)
            var endOfDay = startOfDay.AddDays(1); // Ngày hôm sau lúc 00:00:00

            return await _context.Reservations
                .Where(r => r.ChargingPostId == postId &&
                            r.Status == ReservationStatus.Confirmed &&
                            r.TimeSlotStart >= startOfDay &&
                            r.TimeSlotStart < endOfDay)
                .ToListAsync();
        }

        public async Task<Reservation?> GetFirstOrDefaultAsync(Expression<Func<Reservation, bool>> predicate)
        {
            return await _context.Reservations.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Reservation>> GetOverdueReservationsAsync(int gracePeriodMinutes)
        {
            var now = DateTime.UtcNow.AddHours(7);
            var cutoffTime = now.AddMinutes(-gracePeriodMinutes);

            return await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Confirmed && r.TimeSlotStart < cutoffTime)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetUpcomingReservationsByDriverAsync(string driverId)
        {
            DateTime now = DateTime.UtcNow;
            return await _context.Reservations
                .Where(r => r.DriverId == driverId &&
                            r.Status == ReservationStatus.Confirmed &&
                            r.TimeSlotStart > now)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationHistoryByDriverAsync(string driverId)
        {
            DateTime now = DateTime.UtcNow;
            return await _context.Reservations
                .Where(r => r.DriverId == driverId &&
                            r.Status != ReservationStatus.Confirmed) // Lọc trạng thái không phải là Confirmed 
                .OrderByDescending(r => r.TimeSlotEnd) // Sắp xếp theo thời gian kết thúc mới nhất
                .ToListAsync();
        }

        public async Task<ReservationDetailDto> GetReservationDetailsAsync(int reservationId)
        {
            // Sử dụng Join (hoặc Include) và Select để chiếu dữ liệu trực tiếp sang DTO
            var result = await _context.Reservations
                .Where(r => r.Id == reservationId)
                // Join với ChargingPosts
                .Join(
                    _context.ChargingPosts,
                    reservation => reservation.ChargingPostId,
                    post => post.Id,
                    (reservation, post) => new { reservation, post }
                )
                // Join với Stations
                .Join(
                    _context.Stations,
                    t => t.post.StationId,
                    station => station.Id,
                    (t, station) => new { t.reservation, t.post, station }
                )
                // Chiếu kết quả sang ReservationDetailDto
                .Select(x => new ReservationDetailDto
                {
                    // Reservation
                    Id = x.reservation.Id,
                    DriverId = x.reservation.DriverId,
                    TimeSlotStart = x.reservation.TimeSlotStart,
                    TimeSlotEnd = x.reservation.TimeSlotEnd,
                    Status = x.reservation.Status.ToString(), // Chuyển Enum sang String

                    // Post
                    PostId = x.post.Id,
                    ConnectorType = x.post.ConnectorType.ToString(),
                    PowerKW = x.post.PowerKW,

                    // Station
                    StationId = x.station.Id,
                    StationName = x.station.Name,
                    StationAddress = x.station.Address
                })
                .FirstOrDefaultAsync(); // Chỉ lấy một kết quả

            if (result == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy lịch đặt với ID: {reservationId}");
            }

            return result;
        }

        public async Task<List<Reservation>> GetAllAsync()
        {
            return await _context.Reservations.ToListAsync();
        }

        public void Update(Reservation reservation)
        {
            _context.Reservations.Update(reservation);
        }

        public async Task<List<Reservation>> GetUpcomingReservationsForPostAsync(int postId)
        {
            return await _context.Reservations
                .Where(r => r.ChargingPostId == postId &&
                            r.Status == ReservationStatus.Confirmed &&
                            r.TimeSlotStart > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetConflictingReservationsAsync(int postId, DateTime maintenanceStart, DateTime maintenanceEnd)
        {
            return await _context.Reservations
                .Include(r => r.Vehicle) // Cần để lấy OwnerId
                .Where(r => r.ChargingPostId == postId &&
                            r.Status == ReservationStatus.Confirmed &&
                            // Logic kiểm tra sự trùng lặp thời gian:
                            // (StartA < EndB) and (EndA > StartB)
                            r.TimeSlotStart < maintenanceEnd &&
                            r.TimeSlotEnd > maintenanceStart)
                .ToListAsync();
        }

        public async Task<IEnumerable<Reservation>> FindAllAsync(Expression<Func<Reservation, bool>> predicate, bool asNoTracking = false)
        {
            IQueryable<Reservation> query = _context.Reservations.Where(predicate);

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }
    }
}