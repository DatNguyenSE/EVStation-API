
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IReservationRepository
    {
        // Thêm một đặt chỗ mới
        Task AddReservationAsync(Reservation reservation);
        // Lấy đặt chỗ theo ID
        Task<Reservation?> GetReservationByIdAsync(int id);
        // Kiểm tra xem có đặt chỗ nào bị trùng lặp trong khoảng thời gian không
        Task<bool> IsTimeSlotConflictedAsync(int postId, DateTime start, DateTime end);
        Task<int> CountByDriverInDateAsync(string driverId, DateTime date);
        // Kiểm tra xem có đặt chỗ nào bị trùng lặp trong khoảng thời gian không
        Task<bool> CheckOverlapAsync(int postId, DateTime start, DateTime end);
        Task<Reservation?> GetActiveByPostIdAsync(int postId);
        // Lấy danh sách đặt chỗ trong ngày của trụ
        Task<List<Reservation>> GetReservationsForPostOnDateAsync(int postId, DateTime date);

        Task<Reservation?> GetFirstOrDefaultAsync(Expression<Func<Reservation, bool>> predicate);
    }
}