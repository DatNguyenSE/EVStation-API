using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Helpers.Enums;
using API.Hubs;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class ReservationMonitorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private readonly IHubContext<ChargingHub> _hubContext;

        public ReservationMonitorService(IServiceScopeFactory scopeFactory, IHubContext<ChargingHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var sessionService = scope.ServiceProvider.GetRequiredService<IChargingSessionService>();

                var now = DateTime.UtcNow;

                var allReservations = await uow.Reservations.GetAllAsync();
                var expiredReservations = allReservations.Where(r =>
                                r.Status == ReservationStatus.Confirmed && r.TimeSlotEnd <= now);

                foreach (var res in expiredReservations)
                {
                    // mark reservation completed
                    res.Status = ReservationStatus.Completed;
                    uow.Reservations.Update(res);

                    // find sessions linked to this reservation that are still active
                    var allSessions = await uow.ChargingSessions.GetAllAsync();
                    var sessions = allSessions.Where(s =>
                        s.ReservationId == res.Id &&
                        (s.Status == SessionStatus.Charging || s.Status == SessionStatus.Idle));

                    foreach (var s in sessions)
                    {
                        // If charging: stop simulation -> StopChargingAsync will set Idle and EndTime
                        if (s.Status == SessionStatus.Charging)
                        {
                            // call StopChargingAsync with ReservationCompleted reason
                            try
                            {
                                await sessionService.StopChargingAsync(s.Id, StopReason.ReservationCompleted);
                            }
                            catch
                            {
                                // swallow per-session errors but continue
                            }
                        }
                        else
                        {
                            // already Idle -> set EndTime if missing and immediate IdleFee start
                            if (!s.EndTime.HasValue)
                                s.EndTime = DateTime.UtcNow;
                            s.StopReason = StopReason.ReservationCompleted;
                            s.IdleFeeStartTime = s.EndTime;
                            uow.ChargingSessions.Update(s);

                            await _hubContext.Clients.Group($"session-{s.Id}")
                                .SendAsync("ReceiveSessionStopped", s.Id, s.Status);
                        }
                    }
                }

                await uow.Complete();
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}