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
        public async Task<(bool CanStart, string Message)> ValidateScanAsync(int postId, string driverId)
        {
            // tìm trụ sạc trong db
            var post = await _uow.ChargingPosts.GetByIdAsync(postId);
            if (post == null)
            {
                return (false, "Mã QR không hợp lệ hoặc không tìm thấy trụ sạc.");
            }

            // Phân luồng logic dựa vào thuộc tính IsWalkIn
            if (post.IsWalkIn)
            {
                if (post.Status == Helpers.Enums.PostStatus.Available)
                {
                    return (true, "Trụ vãng lai sẵn sàng. Bạn có thể bắt đầu sạc.");
                }
                else
                {
                    return (false, $"Trụ đang ở trạng thái {post.Status}. Vui lòng thử lại sau.");
                }
            }
            else
            {
                var now = DateTime.UtcNow;

                // Tìm đơn đặt chỗ hợp lệ: đúng người, đúng trụ, đúng trạng thái và TRONG KHUNG GIỜ
                // Cho phép tài xế check-in sớm 15 phút
                var reservation = await _uow.Reservations.GetFirstOrDefaultAsync(r =>
                    r.DriverId == driverId &&
                    r.ChargingPostId == postId &&
                    r.Status == Entities.ReservationStatus.Confirmed &&
                    now >= r.TimeSlotStart.AddMinutes(-15) &&
                    now <= r.TimeSlotEnd);

                // Kiểm tra kết quả
                if (reservation == null)
                {
                    return (false, "Không tìm thấy đặt chỗ hợp lệ cho trụ này. Vui lòng kiểm tra lại thời gian hoặc mã QR.");
                }

                // Người dùng đã có lịch hợp lệ, BÂY GIỜ mới kiểm tra xem trụ có sẵn sàng không.
                switch (post.Status)
                {
                    case PostStatus.Available:
                        // Hợp lệ, cho phép sạc
                        return (true, "Xác thực đặt chỗ thành công. Bạn có thể bắt đầu sạc.");

                    case PostStatus.Occupied: // Đang sạc hoặc đã sạc xong nhưng chưa rời đi
                        return (false, "Lịch đặt của bạn hợp lệ, nhưng trụ đang được sử dụng bởi phiên sạc trước. Vui lòng đợi.");

                    case PostStatus.Maintenance:
                    case PostStatus.Offline:
                        return (false, "Lịch đặt của bạn hợp lệ, nhưng trụ đang bảo trì hoặc bị lỗi. Vui lòng liên hệ hỗ trợ.");

                    default:
                        return (false, $"Trụ đang ở trạng thái không xác định ({post.Status}).");
                }
                
            }
        }
    }
}