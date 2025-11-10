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

                var now = DateTime.UtcNow.AddHours(7);

                var allReservations = await uow.Reservations.GetAllAsync();
                var expiredReservations = allReservations.Where(r =>
                                r.Status == ReservationStatus.InProgress && r.TimeSlotEnd <= now);

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
                        // Trường hợp 3
                        if (s.Status == SessionStatus.Charging)
                        {
                            // call StopChargingAsync with ReservationCompleted reason
                            try
                            {
                                await sessionService.StopChargingAsync(s.Id, StopReason.ReservationCompleted);
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                        // Trường hợp 4 & Trường hợp 1: Đã Idle
                    }
                }

                await uow.Complete();
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}