using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Helpers.Enums;
using API.Hubs;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.Services
{
    public class ChargingSimulationService : IChargingSimulationService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHubContext<ChargingHub> _hubContext;

        // Lưu trạng thái mô phỏng của từng session trong RAM
        private readonly ConcurrentDictionary<int, SimulationState> _sessionStates = new();
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _runningSessions = new();
        private readonly ConcurrentDictionary<int, bool> _stopRequested = new();

        // Interval flush dữ liệu xuống DB (mỗi 60s)
        private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(60);

        public ChargingSimulationService(IServiceScopeFactory scopeFactory, IHubContext<ChargingHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

        // Class mô tả state hiện tại của 1 phiên sạc (giữ trong RAM)
        private class SimulationState
        {
            public double CurrentPercentage { get; set; }
            public double EnergyConsumed { get; set; }
            public int Cost { get; set; }
            public double BatteryCapacity { get; set; }
            public double StartPercentage { get; set; }
            public bool IsFreeCharging { get; set; }
            public bool GuestMode { get; set; }
            public string? OwnerId { get; set; }                 
            public decimal WalletBalance { get; set; }  
            public DateTime LastFlushed { get; set; } = DateTime.UtcNow;
        }

        public bool IsRunning(int sessionId) => _runningSessions.ContainsKey(sessionId);

        public async Task StopSimulationAsync(int sessionId, bool setCompleted)
        {
            if (_runningSessions.TryGetValue(sessionId, out var cts))
            {
                _stopRequested[sessionId] = setCompleted;
                try { cts.Cancel(); } catch { /* ignore */ }

                Console.WriteLine(setCompleted
                    ? $"🟨 Graceful stop requested for session {sessionId}"
                    : $"🟦 Temporary stop requested for session {sessionId}");
            }

            // Chờ quá trình dừng
            var start = DateTime.UtcNow;
            while (_runningSessions.ContainsKey(sessionId) && (DateTime.UtcNow - start).TotalSeconds < 5)
                await Task.Delay(100);

            // Flush state xuống DB (chốt dữ liệu)
            if (_sessionStates.TryRemove(sessionId, out var state))
            {
                using var scope = _scopeFactory.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var session = await uow.ChargingSessions.GetByIdAsync(sessionId);
                if (session != null)
                {
                    session.EndBatteryPercentage = (float)Math.Round(state.CurrentPercentage, 1);
                    session.EnergyConsumed = state.EnergyConsumed;
                    session.Cost = state.Cost;
                    session.EndTime = DateTime.UtcNow;
                    session.Status = setCompleted ? SessionStatus.Completed : SessionStatus.Charging;

                    uow.ChargingSessions.Update(session);
                    await uow.Complete();

                    Console.WriteLine($"💾 Session {sessionId} flushed to DB on stop");
                }
            }

            _runningSessions.TryRemove(sessionId, out _);
            _stopRequested.TryRemove(sessionId, out _);
        }

        // Bắt đầu mô phỏng
        public Task StartSimulationAsync(
            int sessionId,
            double batteryCapacity,
            bool isFreeCharging,
            bool guestMode,
            double initialPercentage,
            double initialEnergy,
            int initialCost,
            string? ownerId,           
            decimal walletBalance = 0)
        {
            if (_runningSessions.ContainsKey(sessionId))
            {
                Console.WriteLine($"⚠️ Simulation already running for session {sessionId}");
                return Task.CompletedTask;
            }

            var cts = new CancellationTokenSource();
            _runningSessions[sessionId] = cts;
            var token = cts.Token;

            // Tạo state ban đầu
            _sessionStates[sessionId] = new SimulationState
            {
                BatteryCapacity = batteryCapacity,
                StartPercentage = initialPercentage,
                CurrentPercentage = initialPercentage,
                EnergyConsumed = initialEnergy,
                Cost = initialCost,
                GuestMode = guestMode,
                IsFreeCharging = isFreeCharging,
                OwnerId = ownerId,
                WalletBalance = walletBalance,
                LastFlushed = DateTime.UtcNow
            };

            _ = Task.Run(async () =>
            {
                try
                {
                    double percentageStep = 0.5;
                    int pricePerKWh = guestMode ? 5000 : 4000;

                    Console.WriteLine($"▶️ Start sim session {sessionId} | initialEnergy={initialEnergy} | initialCost={initialCost} | battery={batteryCapacity} | guest={guestMode}");

                    const int intervalMs = 1000;

                    while (true)
                    {
                        if (token.IsCancellationRequested)
                            break;

                        await Task.Delay(intervalMs, token);

                        // 🔹 Lấy state hiện tại
                        if (!_sessionStates.TryGetValue(sessionId, out var state))
                            break;

                        var previousPercentage = state.CurrentPercentage;
                        state.CurrentPercentage = Math.Min(state.CurrentPercentage + percentageStep, 100);
                        state.CurrentPercentage = Math.Round(state.CurrentPercentage * 2) / 2.0;

                        // Tính energy + cost (chỉ trong RAM)
                        double addedEnergy = ((state.CurrentPercentage - previousPercentage) / 100.0) * state.BatteryCapacity;
                        state.EnergyConsumed = Math.Round(state.EnergyConsumed + addedEnergy, 3, MidpointRounding.AwayFromZero);

                        state.Cost = (int)Math.Round(state.EnergyConsumed * pricePerKWh, MidpointRounding.AwayFromZero);

                        if (!state.GuestMode && !state.IsFreeCharging && state.Cost > state.WalletBalance)
                        {
                            Console.WriteLine($"💸 Session {sessionId} stopped due to insufficient funds (cost={state.Cost}, balance={state.WalletBalance})");

                            await FlushToDatabase(sessionId, state); // Ghi lại thông tin đến thời điểm này

                            using var scope = _scopeFactory.CreateScope();
                            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                            var session = await uow.ChargingSessions.GetByIdAsync(sessionId);
                            if (session != null)
                            {
                                session.Status = SessionStatus.StoppedDueToInsufficientFunds;
                                session.EndBatteryPercentage = (float)Math.Round(state.CurrentPercentage, 1);
                                uow.ChargingSessions.Update(session);
                                await uow.Complete();

                                await _hubContext.Clients.Group($"session-{sessionId}")
                                    .SendAsync("ReceiveSessionEnded", sessionId, session.Status);
                            }

                            // Dừng vòng lặp
                            break;
                        }

                        // Gửi realtime SignalR
                        await _hubContext.Clients.Group($"session-{sessionId}")
                            .SendAsync("ReceiveEnergyUpdate", new
                            {
                                SessionId = sessionId,
                                BatteryPercentage = Math.Round(state.CurrentPercentage, 1),
                                EnergyConsumed = state.EnergyConsumed,
                                Cost = state.Cost
                            });

                        Console.WriteLine($"⚡ Session {sessionId}: {state.CurrentPercentage}% - {state.EnergyConsumed}kWh - {state.Cost}đ");

                        // Flush định kỳ mỗi 60s 
                        if (DateTime.UtcNow - state.LastFlushed >= _flushInterval)
                        {
                            state.LastFlushed = DateTime.UtcNow;
                            await FlushToDatabase(sessionId, state);
                        }

                        // Dừng khi pin đầy
                        if (state.CurrentPercentage >= 100)
                        {
                            await FlushToDatabase(sessionId, state, true);
                            Console.WriteLine($"🟩 Session {sessionId} fully charged");
                            break;
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine($"🟦 Simulation canceled for session {sessionId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"🟥 Simulation error for session {sessionId}: {ex.Message}");
                }
                finally
                {
                    _runningSessions.TryRemove(sessionId, out _);
                }
            }, token);

            return Task.CompletedTask;
        }

        // Flush xuống DB (được gọi khi đủ 60s hoặc khi kết thúc)
        private async Task FlushToDatabase(int sessionId, SimulationState state, bool finalFlush = false)
        {
            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var session = await uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) return;

            session.EndBatteryPercentage = (float)Math.Round(state.CurrentPercentage, 1);
            session.EnergyConsumed = state.EnergyConsumed;
            session.Cost = state.Cost;

            if (finalFlush)
            {
                session.Status = SessionStatus.Full;
                session.EndTime = DateTime.UtcNow;
            }

            uow.ChargingSessions.Update(session);
            await uow.Complete();

            Console.WriteLine($"💾 Flushed session {sessionId} to DB {(finalFlush ? "(final)" : "")}");
        }
    }
}