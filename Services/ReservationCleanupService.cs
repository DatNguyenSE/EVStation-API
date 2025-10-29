using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Services
{
    public class ReservationCleanupService : BackgroundService
    {
        private readonly ILogger<ReservationCleanupService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        public ReservationCleanupService(ILogger<ReservationCleanupService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory; // Tiêm factory để tạo scope
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Service is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckForOverdueReservations();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while checking for overdue reservations.");
                }

                // Chờ 1 phút trước khi chạy lại
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckForOverdueReservations()
        {
            _logger.LogInformation("Checking for overdue reservations at {time}", DateTime.UtcNow.AddHours(7));

            // BƯỚC 1: TẠO SCOPE MỚI ĐỂ SỬ DỤNG UNIT OF WORK
            using (var scope = _scopeFactory.CreateScope())
            {
                // BƯỚC 2: LẤY MỘT INSTANCE UNIT OF WORK MỚI TỪ SCOPE
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                
                var gracePeriod = AppConstant.ReservationRules.NoShowGracePeriodMinutes;
                
                // BƯỚC 3: SỬ DỤNG REPOSITORY THÔNG QUA UNIT OF WORK ĐỂ LẤY DỮ LIỆU
                var overdueReservations = await unitOfWork.Reservations.GetOverdueReservationsAsync(gracePeriod);

                if (!overdueReservations.Any())
                {
                    return; // Không có gì để làm
                }

                // BƯỚC 4: THAY ĐỔI DỮ LIỆU TRÊN CÁC ĐỐI TƯỢNG
                // EF Core Change Tracker sẽ tự động theo dõi các thay đổi này
                foreach (var reservation in overdueReservations)
                {
                    _logger.LogWarning($"Reservation ID {reservation.Id} is overdue. Setting status to NoShow.");
                    reservation.Status = ReservationStatus.Expired;
                }
                
                // BƯỚC 5: GỌI COMPLETE TRÊN UNIT OF WORK ĐỂ LƯU TẤT CẢ THAY ĐỔI
                // Đây là lúc SaveChangesAsync() được gọi, tất cả các cập nhật sẽ được lưu trong một transaction.
                if (await unitOfWork.Complete())
                {
                    _logger.LogInformation($"Successfully updated {overdueReservations.Count()} reservations to NoShow status.");
                }
                else
                {
                    _logger.LogError("Failed to save changes for overdue reservations via UnitOfWork.");
                }
            } // Scope và UnitOfWork sẽ tự động được giải phóng ở đây
        }
    }
}