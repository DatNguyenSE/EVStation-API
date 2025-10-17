using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using API.DTOs.Reservation;
using API.Entities;
using API.Helpers;
using API.Helpers.Enums;
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

        public async Task<ReservationResponseDto> CancelReservationAsync(int reservationId, string driverId)
        {
            // Bắt đầu transaction để đảm bảo tất cả thay đổi đều thành công hoặc không gì cả
            await using var transaction = await _uow.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                // kiểm tra có lịch đặt ko
                var reservation = await _uow.Reservations.GetReservationByIdAsync(reservationId);
                if (reservation == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy lịch đặt chỗ.");
                }

                // Kiểm tra quyền sở hữu
                if (reservation.DriverId != driverId)
                {
                    throw new UnauthorizedAccessException("Bạn không có quyền hủy lịch đặt của người khác.");
                }

                // 3. Kiểm tra trạng thái hợp lệ để hủy
                if (reservation.Status != ReservationStatus.Confirmed)
                    throw new InvalidOperationException($"Không thể hủy lịch đặt ở trạng thái '{reservation.Status}'.");

                // 4. (Rule nghiệp vụ) Kiểm tra thời gian cho phép hủy
                // Không cho phép hủy nếu lịch đặt sắp bắt đầu trong vòng 20 phút
                var cutoffTime = reservation.TimeSlotStart.AddMinutes(-AppConstant.ReservationRules.CancellationCutoffMinutes);
                if (DateTime.UtcNow >= cutoffTime)
                    throw new InvalidOperationException($"Không thể hủy khi còn ít hơn {AppConstant.ReservationRules.CancellationCutoffMinutes} giờ là đến giờ hẹn.");

                // === Bắt đầu thay đổi trạng thái ===
                //Cập nhật trạng thái của lịch đặt thành Canceled
                reservation.Status = ReservationStatus.Cancelled;
                
                // Lưu tất cả thay đổi vào database
                if (!await _uow.Complete())
                {
                    throw new Exception("Lỗi hệ thống: Không thể lưu các thay đổi khi hủy đặt chỗ.");
                }

                // Commit transaction nếu mọi thứ thành công
                await transaction.CommitAsync();

                return reservation.ToReservationResponseDto();
                
            } catch (Exception)
            {
                await transaction.RollbackAsync();
                // Ném lại lỗi để Controller xử lý
                throw;
            }
        }

        public async Task<ReservationResponseDto> CreateReservationAsync(CreateReservationDto dto, string driverId)
        {
            // var now = DateTime.UtcNow.AddHours(AppConstant.ReservationRules.TimezoneOffsetHours);
            var now = DateTime.UtcNow;

            // Ép kiểu DateTime nhận được thành UTC để đảm bảo tính nhất quán
            var timeSlotStartUtc = DateTime.SpecifyKind(dto.TimeSlotStart, DateTimeKind.Utc);

            // Kiểm tra giờ chẵn (phút, giây, mili giây phải = 0)
            if (timeSlotStartUtc.Minute != 0 || timeSlotStartUtc.Second != 0 || timeSlotStartUtc.Millisecond != 0)
                throw new Exception("Thời gian bắt đầu phải là giờ chẵn (ví dụ: 08:00, 09:00, 10:00).");

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
            if (vehicle.ConnectorType == ConnectorType.CCS2)
            {
                if (post.ConnectorType != ConnectorType.CCS2 && post.ConnectorType != ConnectorType.Type2)
                {
                    throw new Exception($"Xe sạc CCS2 không tương thích. Trụ này là loại '{post.ConnectorType}', chỉ chấp nhận trụ CCS2 hoặc Type2.");
                }
            }
            else 
            {
                // Đối với các loại xe khác, cổng sạc của xe và trụ phải khớp chính xác.
                if (vehicle.ConnectorType != post.ConnectorType)
                {
                    throw new Exception($"Loại xe không tương thích. Trụ này yêu cầu loại sạc '{post.ConnectorType}'.");
                }
            }

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