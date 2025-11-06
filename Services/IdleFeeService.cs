using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers;
using API.Helpers.Enums;
using API.Hubs;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Org.BouncyCastle.Asn1.Cms;

namespace API.Services
{
    public class IdleFeeService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ChargingHub> _hubContext;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

        public IdleFeeService(IServiceScopeFactory scopeFactory, IHubContext<ChargingHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var _uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var _pricingService = scope.ServiceProvider.GetRequiredService<IPricingService>();

                var occupancyPricing = await _pricingService.GetCurrentActivePriceByTypeAsync(PriceType.OccupancyFee);
                var overstayPricing = await _pricingService.GetCurrentActivePriceByTypeAsync(PriceType.OverstayFee); // Cần PriceType.OverstayFee
                
                var IDLE_FEE_PER_MINUTE = (int)(occupancyPricing?.PricePerMinute ?? 1000); 
                var OVERSTAY_FEE_PER_MINUTE = (int)(overstayPricing?.PricePerMinute ?? 2000);

                // Get Idle sessions that are not completed
                var allSession = await _uow.ChargingSessions.GetAllAsync();
                var idleSessions = (allSession.Where(s =>
                    s.Status == SessionStatus.Idle &&
                    !s.CompletedTime.HasValue &&
                    s.EndTime.HasValue)).ToList();

                foreach (var s in idleSessions)
                {
                    // Xác định thời điểm bắt đầu tính phí Occupancy/Overstay
                    bool noGrace = false;

                    // Nếu StopReason là ReservationCompleted HOẶC có ReservationId (đã được cập nhật bởi ReservationMonitorService)
                    bool isReservationSession = !s.IsWalkInSession && s.ReservationId.HasValue;
                    
                    if (isReservationSession && s.StopReason == StopReason.ReservationCompleted)
                    {
                        noGrace = true;
                    }
                    else if (isReservationSession) // Dành cho các phiên Idle của khách đặt chỗ nhưng chưa hết giờ đặt chỗ
                    {
                        // Kiểm tra nếu phiên Idle bắt đầu sau khi hết giờ đặt chỗ (chỉ để đảm bảo)
                        if (s.EndTime.HasValue && s.Reservation?.TimeSlotEnd != null && s.EndTime.Value >= s.Reservation.TimeSlotEnd.AddMinutes(AppConstant.ChargingRules.IDLE_GRACE_MINUTES))
                        {
                            noGrace = true;
                        }
                    }

                    DateTime endTime = DateTime.UtcNow.AddHours(7);
                    if (s.EndTime!.HasValue)
                    {
                        endTime = s.EndTime!.Value;
                    }
                    
                    DateTime feeStart = noGrace ? endTime : endTime.AddMinutes(AppConstant.ChargingRules.IDLE_GRACE_MINUTES);
                    s.IdleFeeStartTime ??= feeStart; // set if not set

                    if (DateTime.UtcNow.AddHours(7) < feeStart)
                    {
                        // still in grace
                        if (s.IdleFee != 0 || (s.OverstayFee ?? 0) != 0)
                        {
                            s.IdleFee = 0;
                            s.OverstayFee = 0; 
                            _uow.ChargingSessions.Update(s);
                        }
                        continue;
                    }
                    
                    // --- BẮT ĐẦU LOGIC TÍNH HAI LOẠI PHÍ PHÂN TÁCH ---
                    
                    int currentIdleFee = 0;
                    int currentOverstayFee = 0;
                    
                    // Lấy thời điểm kết thúc đặt chỗ (chỉ có cho Member Session và có Reservation)
                    DateTime? timeSlotEnd = (s.ReservationId.HasValue && s.Reservation?.TimeSlotEnd != null) 
                                            ? s.Reservation.TimeSlotEnd // Đổi sang giờ local
                                            : null;

                    // Thời điểm hiện tại (Local)
                    DateTime nowLocal = DateTime.UtcNow.AddHours(7);

                    // 1. TÍNH OCCUPANCY FEE (Phí Chiếm Dụng)
                    if (timeSlotEnd.HasValue)
                    {
                        // Tính phút chiếm dụng TRƯỚC khi hết giờ đặt chỗ (hoặc đến hiện tại nếu TimeSlotEnd chưa tới)

                        // Thời điểm kết thúc tính phí chiếm dụng thường (là TimeSlotEnd, hoặc Now nếu TimeSlotEnd chưa tới)
                        DateTime occupancyFeeEnd = nowLocal < timeSlotEnd.Value ? nowLocal : timeSlotEnd.Value;

                        // Chỉ tính nếu feeStart nhỏ hơn thời điểm kết thúc tính phí chiếm dụng (occupancyFeeEnd)
                        if (feeStart < occupancyFeeEnd)
                        {
                            var minutesOccupancy = (int)Math.Floor((occupancyFeeEnd - feeStart).TotalMinutes);
                            if (minutesOccupancy > 0)
                            {
                                currentIdleFee = minutesOccupancy * IDLE_FEE_PER_MINUTE;
                            }
                        }
                    }
                    else // Áp dụng cho Walk-in hoặc Reservation không có TimeSlotEnd: chỉ tính Occupancy Fee
                    {
                        var minutes = (int)Math.Floor((nowLocal - feeStart).TotalMinutes);
                        if (minutes > 0)
                        {
                            currentIdleFee = minutes * IDLE_FEE_PER_MINUTE;
                        }
                    }

                    System.Console.WriteLine($"========== {timeSlotEnd?.ToString()}");
                    System.Console.WriteLine($"========== {nowLocal.ToString()}");

                    // 2. TÍNH OVERSTAY FEE (Phí Quá Giờ)
                    if (timeSlotEnd.HasValue && nowLocal > timeSlotEnd.Value)
                    {
                        s.IsOverstay = true;
                        // Tính phút quá giờ: từ TimeSlotEnd đến hiện tại
                        var minutesOverstay = (int)Math.Floor((nowLocal - timeSlotEnd.Value).TotalMinutes);
                        if (minutesOverstay > 0)
                        {
                            currentOverstayFee = minutesOverstay * OVERSTAY_FEE_PER_MINUTE;
                        }
                    }
                    
                    // 3. CẬP NHẬT VÀ GỬI SIGNALR
                    if (s.IdleFee != currentIdleFee || (s.OverstayFee ?? 0) != currentOverstayFee)
                    {
                        s.IdleFee = currentIdleFee;
                        s.OverstayFee = currentOverstayFee;
                        _uow.ChargingSessions.Update(s);

                        // SignalR notify clients
                        await _hubContext.Clients.Group($"session-{s.Id}")
                            .SendAsync("ReceiveIdleFeeUpdated", new
                            {
                                SessionId = s.Id,
                                IdleFee = s.IdleFee,
                                OverstayFee = s.OverstayFee
                            });
                    }

                }

                await _uow.Complete();

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}