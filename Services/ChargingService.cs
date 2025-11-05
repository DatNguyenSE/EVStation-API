using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers.Enums;
using API.Interfaces;

namespace API.Services
{
    public class ChargingService : IChargingService
    {
        private readonly IUnitOfWork _uow;
        public ChargingService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Task StartChargingAsync(string chargerId)
        {
            throw new NotImplementedException();
        }

        public Task StopChargingAsync(string chargerId)
        {
            throw new NotImplementedException();
        }


        // sau khi quét qr trên trụ sẽ gọi API để kiểm tra để có thể bắt đầu sạc
        public async Task<(bool CanStart, string Message, int? ReservationId, int? VehicleId)> ValidateScanAsync(int postId, string? driverId)
        {
            // tìm trụ sạc trong db
            var post = await _uow.ChargingPosts.GetByIdAsync(postId);
            if (post == null)
            {
                return (false, " Mã QR không hợp lệ hoặc không tìm thấy trụ sạc.", null, null);
            }

            // Phân luồng logic dựa vào thuộc tính IsWalkIn
            if (post.IsWalkIn)
            {
                if (post.Status == Helpers.Enums.PostStatus.Available)
                {
                    return (true, "Trụ vãng lai sẵn sàng. Bạn có thể bắt đầu sạc.", null, null);
                }
                else
                {
                    return (false, $"Trụ đang ở trạng thái {post.Status}. Vui lòng thử lại sau.", null, null);
                }
            }

            // Nếu là trụ đặt chỗ
            var now = DateTime.UtcNow.AddHours(7);
            var reservation = await _uow.Reservations.GetFirstOrDefaultAsync(r =>
                r.Vehicle.OwnerId == driverId &&
                r.ChargingPostId == postId &&
                (r.Status == Entities.ReservationStatus.Confirmed));

            // Nếu không có đơn hợp lệ
            if (reservation == null)
            {
                return (false, " Không tìm thấy đặt chỗ hợp lệ cho trụ này. Vui lòng kiểm tra lại thời gian hoặc mã QR.", null, null);
            }

            // Kiểm tra khung giờ (cho phép check-in sớm 15 phút)
            bool isEarly = now < reservation.TimeSlotStart.AddMinutes(-15);
            bool isLate = now > reservation.TimeSlotEnd;

            // if (isEarly)
            // {
            //     return (false,
            //         $" Chưa đến thời gian đặt chỗ.- Giờ hiện tại: {now:HH:mm}- Giờ đặt: {reservation.TimeSlotStart:HH:mm} - {reservation.TimeSlotEnd:HH:mm} (UTC).");
            // }

            // if (isLate)
            // {
            //     return (false,
            //         $" Đã quá thời gian đặt chỗ. - Giờ hiện tại: {now:HH:mm} - Giờ đặt: {reservation.TimeSlotStart:HH:mm} - {reservation.TimeSlotEnd:HH:mm} (UTC).");
            // }

            // Người dùng có đặt chỗ hợp lệ, kiểm tra trạng thái trụ
            switch (post.Status)
            {
                case PostStatus.Available:
                    reservation.Status = Entities.ReservationStatus.InProgress;
                    return (true,
                        $" Xác thực đặt chỗ thành công. - Giờ hiện tại: {now:HH:mm} - Khung giờ đặt: {reservation.TimeSlotStart:HH:mm} - {reservation.TimeSlotEnd:HH:mm} (UTC).", reservation.Id, reservation.VehicleId);

                case PostStatus.Occupied:
                    return (false, " Lịch đặt hợp lệ, nhưng trụ đang được sử dụng. Vui lòng đợi.", reservation.Id, reservation.VehicleId);

                case PostStatus.Maintenance:
                case PostStatus.Offline:
                    return (false, " Trụ đang bảo trì hoặc offline. Vui lòng liên hệ hỗ trợ.", null, null);

                default:
                    return (false, $" Trụ đang ở trạng thái không xác định ({post.Status}).", null, null);
            }
        }

    }
}