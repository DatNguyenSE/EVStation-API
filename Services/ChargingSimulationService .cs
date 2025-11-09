using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.DTOs.Pricing;
using API.Entities;
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

        // RAM state
        private readonly ConcurrentDictionary<int, SimulationState> _sessionStates = new();
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _runningSessions = new();
        private readonly ConcurrentDictionary<int, bool> _stopRequested = new();
        private readonly ConcurrentDictionary<int, double> _batteryState = new();

        private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(60);

        public ChargingSimulationService(IServiceScopeFactory scopeFactory, IHubContext<ChargingHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _hubContext = hubContext;
        }

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
            public double ChargerPowerKW { get; set; }
            public ConnectorType ConnectorType { get; set; }
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

            // wait for loop to exit
            var start = DateTime.UtcNow;
            while (_runningSessions.ContainsKey(sessionId) && (DateTime.UtcNow - start).TotalSeconds < 5)
                await Task.Delay(100);

            // flush state to DB
            if (_sessionStates.TryGetValue(sessionId, out var state)) // Giữ TryGetValue, không Remove ngay
            {
                using var scope = _scopeFactory.CreateScope();
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // Force flush trước để sync state vào DB
                await FlushToDatabase(sessionId, state, fullFlush: setCompleted);

                // Reload session sau flush để lấy data mới nhất
                var session = await uow.ChargingSessions.GetByIdAsync(sessionId);
                if (session != null)
                {
                    Console.WriteLine($"🟨 CurrentPercentage: {state.CurrentPercentage}");

                    if (setCompleted)
                    {
                        session.Status = SessionStatus.Idle; // Set Idle nếu graceful stop
                        session.StopReason = StopReason.ManualStop; // Hoặc tùy theo context
                    }

                    uow.ChargingSessions.Update(session);
                    await uow.Complete();

                    // SignalR: inform clients
                    await _hubContext.Clients.Group($"session-{sessionId}")
                        .SendAsync("ReceiveSessionStopped", sessionId, session.Status);

                    Console.WriteLine($"💾 Session {sessionId} flushed to DB on stop (setCompleted={setCompleted})");
                }

                // Remove sau khi done
                _sessionStates.TryRemove(sessionId, out _);
            }

            _runningSessions.TryRemove(sessionId, out _);
            _stopRequested.TryRemove(sessionId, out _);
        }

        public async Task StartSimulationAsync(
    int sessionId,
    double batteryCapacity,
    bool isFreeCharging,
    bool guestMode,
    double chargerPowerKW,
    ConnectorType connectorType,
    double initialPercentage,
    double initialEnergy,
    int initialCost,
    string? ownerId,
    decimal walletBalance = 0)
        {
            if (_runningSessions.ContainsKey(sessionId))
            {
                Console.WriteLine($"⚠️ Simulation already running for session {sessionId}");
                return;
            }

            var cts = new CancellationTokenSource();
            _runningSessions[sessionId] = cts;
            var token = cts.Token;

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
                ChargerPowerKW = chargerPowerKW,
                ConnectorType = connectorType,
                LastFlushed = DateTime.UtcNow
            };

            double percentageStep = chargerPowerKW switch
            {
                <= 1.2 => 0.1,
                <= 11 => 0.2,
                <= 60 => 0.5,
                <= 150 => 1.0,
                <= 250 => 1.5,
                _ => 0.2
            };

            bool isAC = connectorType == ConnectorType.Type2 || connectorType == ConnectorType.VinEScooter;
            bool isDC = !isAC;

            var priceType = (guestMode, isAC) switch
            {
                (false, true) => PriceType.Member_AC,
                (false, false) => PriceType.Member_DC,
                (true, true) => PriceType.Guest_AC,
                (true, false) => PriceType.Guest_DC,
            };

            PricingDto? pricing = null;
            try
            {
                using var pricingScope = _scopeFactory.CreateScope();
                var pricingService = pricingScope.ServiceProvider.GetRequiredService<IPricingService>();
                pricing = await pricingService.GetCurrentActivePriceByTypeAsync(priceType);
            }
            catch (Exception px)
            {
                // If pricing lookup fails, log and continue using last-known/default values (pricePerKWh = 0)
                Console.WriteLine($"⚠️ Pricing lookup failed for session {sessionId}: {px}");
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine($"▶️ Start sim session {sessionId} | initialEnergy={initialEnergy} | initialCost={initialCost} | battery={batteryCapacity} | guest={guestMode}");

                    const int intervalMs = 1000;

                    while (true)
                    {
                        if (token.IsCancellationRequested)
                            break;

                        await Task.Delay(intervalMs, token);

                        if (!_sessionStates.TryGetValue(sessionId, out var state))
                            break;

                        int pricePerKWh = (int)(pricing?.PricePerKwh ?? 0);

                        var previousPercentage = state.CurrentPercentage;
                        state.CurrentPercentage = Math.Min(state.CurrentPercentage + percentageStep, 100);
                        state.CurrentPercentage = Math.Round(state.CurrentPercentage, 1);

                        if (state.BatteryCapacity > 0)
                        {
                            double addedEnergy = ((state.CurrentPercentage - previousPercentage) / 100.0) * state.BatteryCapacity;
                            state.EnergyConsumed = Math.Round(state.EnergyConsumed + addedEnergy, 3, MidpointRounding.AwayFromZero);

                            _batteryState[sessionId] = state.CurrentPercentage;

                            state.Cost = (int)Math.Round(state.EnergyConsumed * pricePerKWh, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            // ⚠️ Nếu chưa biết pin (xe chưa xác định) → chỉ update % pin để hiển thị
                            _batteryState[sessionId] = state.CurrentPercentage;
                        }

                        // insufficient funds -> stop but mark Idle & StopReason handled by service layer
                        if (!state.GuestMode && !state.IsFreeCharging && state.Cost > state.WalletBalance)
                        {
                            Console.WriteLine($"💸 Session {sessionId} stopped due to insufficient funds (cost={state.Cost}, balance={state.WalletBalance})");

                            await FlushToDatabase(sessionId, state); // flush current values

                            using var scope = _scopeFactory.CreateScope();
                            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                            var session = await uow.ChargingSessions.GetByIdAsync(sessionId);
                            if (session != null)
                            {
                                session.Status = SessionStatus.Idle;
                                session.EndBatteryPercentage = (decimal)Math.Round(state.CurrentPercentage, 1);
                                session.EndTime = DateTime.UtcNow.AddHours(7);
                                session.StopReason = StopReason.InsufficientFunds;
                                uow.ChargingSessions.Update(session);
                                await uow.Complete();


                                double energyNeededKWh = (100 - state.CurrentPercentage) / 100.0 * state.BatteryCapacity;
                                double hoursRemaining = energyNeededKWh / state.ChargerPowerKW;

                                // Tính tổng giây (dạng double)
                                double totalSecondsDouble = hoursRemaining * 3600;

                                // Làm tròn lên đến giây gần nhất (ceiling)
                                int totalSeconds = (int)Math.Ceiling(totalSecondsDouble);

                                // Tạo TimeSpan từ giây đã làm tròn
                                TimeSpan TimeRemain = TimeSpan.FromSeconds(totalSeconds);

                                await _hubContext.Clients.Group($"session-{sessionId}")
                                .SendAsync("ReceiveEnergyUpdate", new
                                {
                                    SessionId = sessionId,
                                    BatteryPercentage = Math.Round(state.CurrentPercentage, 1),
                                    EnergyConsumed = state.EnergyConsumed,
                                    Cost = state.Cost,
                                    TimeRemainTotalSeconds = (int)TimeRemain.TotalSeconds
                                });

                                // SignalR: notify client
                                await _hubContext.Clients.Group($"session-{sessionId}")
                                    .SendAsync("ReceiveSessionStopped_InsufficientFunds", sessionId, session.Status);
                            }

                            break;
                        }

                        TimeSpan timeRemain = TimeSpan.Zero;

                        if (state.CurrentPercentage < 100 && state.BatteryCapacity > 0 && state.ChargerPowerKW > 0)
                        {
                            double energyNeededKWh = (100 - state.CurrentPercentage) / 100.0 * state.BatteryCapacity;
                            double hoursRemaining = energyNeededKWh / state.ChargerPowerKW;

                            // Tính tổng giây (dạng double)
                            double totalSecondsDouble = hoursRemaining * 3600;

                            // Làm tròn lên đến giây gần nhất (ceiling)
                            int totalSeconds = (int)Math.Ceiling(totalSecondsDouble);

                            // Tạo TimeSpan từ giây đã làm tròn
                            timeRemain = TimeSpan.FromSeconds(totalSeconds);
                        }

                        // realtime update
                        await _hubContext.Clients.Group($"session-{sessionId}")
                            .SendAsync("ReceiveEnergyUpdate", new
                            {
                                SessionId = sessionId,
                                BatteryPercentage = Math.Round(state.CurrentPercentage, 1),
                                EnergyConsumed = state.EnergyConsumed,
                                Cost = state.Cost,
                                // TimeRemainHours = (int)timeRemain.TotalHours,
                                // TimeRemainMinutes = timeRemain.Minutes,
                                // TimeRemainSeconds = timeRemain.Seconds,
                                // Hoặc gửi total seconds nếu FE muốn tự format
                                TimeRemainTotalSeconds = (int)timeRemain.TotalSeconds
                            });

                        Console.WriteLine($"⚡ Session {sessionId}: {state.CurrentPercentage}% - {state.EnergyConsumed}kWh - {state.Cost}đ");

                        if (DateTime.UtcNow - state.LastFlushed >= _flushInterval)
                        {
                            state.LastFlushed = DateTime.UtcNow;
                            await FlushToDatabase(sessionId, state);
                        }

                        if (state.CurrentPercentage >= 100)
                        {
                            await FlushToDatabase(sessionId, state, true);

                            await _hubContext.Clients.Group($"session-{sessionId}")
                                .SendAsync("ReceiveSessionFull", sessionId);

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
                    Console.WriteLine($"🟥 Simulation error for session {sessionId}: {ex.ToString()}");
                }
                finally
                {
                    _runningSessions.TryRemove(sessionId, out _);
                }
            }, token);

            return;
        }

        private async Task FlushToDatabase(int sessionId, SimulationState state, bool fullFlush = false)
        {
            using var scope = _scopeFactory.CreateScope();
            var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var session = await uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) return;

            session.EndBatteryPercentage = (decimal)Math.Round(state.CurrentPercentage, 1);
            session.EnergyConsumed = state.EnergyConsumed;
            session.Cost = state.Cost;

            if (fullFlush)
            {
                session.Status = SessionStatus.Idle;
                session.EndTime = DateTime.UtcNow.AddHours(7);
                session.StopReason = StopReason.BatteryFull;
            }

            await uow.Complete();

            Console.WriteLine($"💾 Setting EndBatteryPercentage to {Math.Round(state.CurrentPercentage, 1)} in flush");
            Console.WriteLine($"💾 Setting EnergyConsumed to {Math.Round(state.EnergyConsumed, 1)} in flush");
            Console.WriteLine($"💾 Setting Cost to {state.Cost} in flush");
            Console.WriteLine($"💾 Flushed session {sessionId} to DB {(fullFlush ? "(final -> Idle)" : "")}");
        }
    }
}