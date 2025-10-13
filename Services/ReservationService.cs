using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.Reservation;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Mappers;

namespace API.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IVehicleRepository _vehicleRepo;

        public ReservationService(IUnitOfWork uow, IVehicleRepository vehicleRepository)
        {
            _uow = uow;
            _vehicleRepo = vehicleRepository;
        }

        public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto dto, string driverId)
        {
            // var now = DateTime.UtcNow.AddHours(AppConstant.ReservationRules.TimezoneOffsetHours);
            var now = DateTime.UtcNow;

            // Ép kiểu DateTime nhận được thành UTC để đảm bảo tính nhất quán
            var timeSlotStartUtc = DateTime.SpecifyKind(dto.TimeSlotStart, DateTimeKind.Utc);

            // Kiểm tra không đặt trong quá khứ
            if (timeSlotStartUtc < now)
                throw new Exception("Không thể đặt chỗ trong quá khứ.");

            // Kiểm tra xe có tồn tại không
            var vehicle = await _vehicleRepo.GetVehicleByIdAsync(dto.VehicleId);
            if (vehicle == null)
                throw new Exception("Xe không tồn tại.");
                
            // Kiểm tra quyền sở hữu
            if (vehicle.OwnerId != driverId)
               throw new Exception("Bạn không có quyền đặt chỗ cho xe này.");

            // Kiểm tra trụ sạc có tồn tại không
            var post = await _uow.ChargingPosts.GetByIdAsync(dto.ChargingPostId);
            if (post == null)
                throw new Exception("Trụ sạc không tồn tại.");

            // Kiểm tra trạng thái trụ
            if (post.Status != Helpers.Enums.PostStatus.Available)
                throw new Exception($"Trụ sạc hiện đang ở trạng thái {post.Status}, không thể đặt chỗ.");

            // So sánh trực tiếp loại cổng sạc của xe và của trụ.
            if (vehicle.ConnectorType != post.ConnectorType)
                throw new Exception($"Loại xe không tương thích. Trụ này yêu cầu loại sạc '{post.ConnectorType}'.");

            // Kiểm tra số slot hợp lệ (1–4)
            if (dto.SlotCount < AppConstant.ReservationRules.MinSlotCount || dto.SlotCount > AppConstant.ReservationRules.MaxSlotCount)
                throw new Exception($"Số slot phải từ {AppConstant.ReservationRules.MinSlotCount} đến {AppConstant.ReservationRules.MaxSlotCount}.");

            var start = dto.TimeSlotStart;
            var end = start.AddHours(dto.SlotCount);

            // Kiểm tra driver đã đặt bao nhiêu lần trong ngày
            var today = now.Date;
            var countToday = await _uow.Reservations.CountByDriverInDateAsync(driverId, today);
            if (countToday > AppConstant.ReservationRules.MaxReservationsPerDay)
                throw new Exception($"Mỗi tài xế chỉ được đặt tối đa {AppConstant.ReservationRules.MaxReservationsPerDay} lần mỗi ngày.");

            // Bắt đầu Transaction (mức cô lập cao để tránh 2 người cùng đặt)
            await using var transaction = await _uow.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                // Kiểm tra trùng giờ — kiểm tra trong cùng transaction để tránh race condition
                bool overlap = await _uow.Reservations.CheckOverlapAsync(dto.ChargingPostId, start, end);
                if (overlap)
                    throw new Exception("Trụ sạc đã có người đặt trong khung giờ này.");

                // Tạo đối tượng Reservation mới 
                var reservation = new Reservation
                {
                    VehicleId = dto.VehicleId,
                    ChargingPostId = dto.ChargingPostId,
                    DriverId = driverId,
                    TimeSlotStart = start,
                    TimeSlotEnd = end,
                    Status = ReservationStatus.Confirmed,
                    CreatedAt = DateTime.UtcNow
                };
                // Thêm Reservation vào Context (chưa lưu)
                await _uow.Reservations.AddReservationAsync(reservation);
                // Cập nhật trạng thái trụ
                post.Status = Helpers.Enums.PostStatus.Reserved;
                await _uow.ChargingPosts.UpdateStatusAsync(post.Id, Helpers.Enums.PostStatus.Reserved);

                // Lưu và commit
                if (!await _uow.Complete())
                {
                    throw new Exception("Lỗi hệ thống: Không thể lưu các thay đổi.");
                }

                await transaction.CommitAsync();

                return reservation.ToReservationResponseDto();
            }
            catch(Exception e)
            {
                e.ToString();
                // Nếu có bất kỳ lỗi nào xảy ra trong khối try, rollback transaction
                // Mọi thay đổi (thêm reservation, cập nhật status) sẽ bị hủy bỏ
                await transaction.RollbackAsync();
                // Ném lại exception để các lớp cao hơn (Controller, Middleware) xử lý
                throw;
            }
        }
    }
}