using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Hubs;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class ChargingSimulationService : BackgroundService, IChargingSimulationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _runningSessions = new();

        public ChargingSimulationService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public bool IsRunning(int sessionId) => _runningSessions.ContainsKey(sessionId);

        public async Task StartSimulationAsync(int sessionId, double batteryCapacity)
        {
            if (_runningSessions.ContainsKey(sessionId))
                return; // đã chạy rồi thì thôi

            var cts = new CancellationTokenSource();
            _runningSessions[sessionId] = cts;

            _ = Task.Run(() => SimulateChargingAsync(sessionId, batteryCapacity, cts.Token));
        }

        public async Task StopSimulationAsync(int sessionId)
        {
            if (_runningSessions.TryRemove(sessionId, out var cts))
            {
                cts.Cancel();
                Console.WriteLine($"🟥 Stop simulation for session {sessionId}");
            }

            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var session = await uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session != null)
            {
                session.Status = API.Helpers.Enums.SessionStatus.Completed;
                uow.ChargingSessions.Update(session);
                await uow.Complete();
            }

            var hub = scope.ServiceProvider.GetRequiredService<IHubContext<ChargingHub>>();
            await hub.Clients.Group($"session_{sessionId}")
                .SendAsync("ReceiveSessionEnded", sessionId, "Stopped by user");
        }

        private async Task SimulateChargingAsync(int sessionId, double batteryCapacity, CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var hub = scope.ServiceProvider.GetRequiredService<IHubContext<ChargingHub>>();

            try
            {
                Console.WriteLine($"🔋 Start simulate charging for session {sessionId}, battery {batteryCapacity}");

                var session = await uow.ChargingSessions.GetByIdAsync(sessionId);
                if (session == null) return;

                double pricePerKWh = 5;
                double currentPercentage = session.StartBatteryPercentage ?? 10;
                double energyConsumed = 0;
                int cost = 0;
                const double ratePerInterval = 0.5; // tăng 0.5%/s
                const int intervalMs = 1000;

                while (currentPercentage < 100 && !token.IsCancellationRequested)
                {
                    await Task.Delay(intervalMs, token);

                    currentPercentage += ratePerInterval;
                    if (currentPercentage > 100) currentPercentage = 100;

                    double addedEnergy = (ratePerInterval / 100) * batteryCapacity;
                    energyConsumed += addedEnergy;
                    cost = (int)(energyConsumed * pricePerKWh);

                    int timeRemain = currentPercentage >= 100 ? 0 : (int)((100 - currentPercentage) / ratePerInterval);

                    var update = new EnergyUpdateDto
                    {
                        SessionId = sessionId,
                        EnergyConsumed = energyConsumed,
                        BatteryPercentage = currentPercentage,
                        TimeRemain = timeRemain,
                        Cost = cost
                    };

                    // update DB
                    var s = await uow.ChargingSessions.GetByIdAsync(sessionId);
                    if (s == null) break;
                    s.EnergyConsumed = update.EnergyConsumed;
                    s.EndBatteryPercentage = (float)update.BatteryPercentage;
                    s.Cost = (int) update.Cost;

                    if (update.BatteryPercentage >= 100)
                        s.Status = API.Helpers.Enums.SessionStatus.Full;

                    uow.ChargingSessions.Update(s);
                    await uow.Complete();

                    await hub.Clients.Group($"session_{sessionId}")
                        .SendAsync("ReceiveSessionUpdate", update);

                    Console.WriteLine($"⚡ Session {sessionId}: {update.BatteryPercentage:0.0}% - {update.EnergyConsumed:0.0}kWh");
                }

                Console.WriteLine($"🟩 Simulation finished for session {sessionId}");
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"🟥 Simulation for session {sessionId} was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR simulate session {sessionId}: {ex.Message}");
            }
            finally
            {
                _runningSessions.TryRemove(sessionId, out _);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // BackgroundService yêu cầu override, nhưng mình không cần loop riêng
            return Task.CompletedTask;
        }
    }
}