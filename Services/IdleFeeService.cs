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

                var pricing = await _pricingService.GetCurrentActivePriceByTypeAsync(PriceType.OccupancyFee);
                var IDLE_FEE_PER_MINUTE = (int) (pricing.PricePerMinute ?? 1000);

                // Get Idle sessions that are not completed
                var allSession = await _uow.ChargingSessions.GetAllAsync();
                var idleSessions = (allSession.Where(s =>
                    s.Status == SessionStatus.Idle &&
                    !s.CompletedTime.HasValue &&
                    s.EndTime.HasValue)).ToList();

                foreach (var s in idleSessions)
                {
                    // determine whether to apply grace
                    bool noGrace = false;

                    // If reservation ended (StopReason == ReservationCompleted) AND this is a reservation post -> immediate fee
                    if (!s.IsWalkInSession && s.StopReason == StopReason.ReservationCompleted)
                    {
                        noGrace = true;
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
                        if (s.IdleFee != 0)
                        {
                            s.IdleFee = 0;
                            _uow.ChargingSessions.Update(s);
                        }
                        continue;
                    }

                    // minutes since feeStart
                    var minutes = (int)Math.Floor((DateTime.UtcNow.AddHours(7) - feeStart).TotalMinutes);
                    if (minutes < 0) minutes = 0;
                    var newFee = minutes * IDLE_FEE_PER_MINUTE;

                    if (s.IdleFee != newFee)
                    {
                        s.IdleFee = newFee;
                        _uow.ChargingSessions.Update(s);

                        // SignalR notify clients in session group
                        await _hubContext.Clients.Group($"session-{s.Id}")
                            .SendAsync("ReceiveIdleFeeUpdated", new
                            {
                                SessionId = s.Id,
                                IdleFee = s.IdleFee
                            });
                    }
                }

                await _uow.Complete();

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}