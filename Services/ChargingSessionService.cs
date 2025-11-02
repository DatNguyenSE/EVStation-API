using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using API.Entities;
using API.Helpers.Enums;
using API.Hubs;
using API.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MimeKit.Text;
using Microsoft.EntityFrameworkCore;
using API.DTOs.Receipt;
using API.DTOs.Pricing;
using API.Helpers;

namespace API.Services
{
    public class ChargingSessionService : IChargingSessionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IWalletService _walletService;
        private readonly IHubContext<ChargingHub> _hubContext;
        private readonly IChargingSimulationService _simulationService;
        private readonly IEmailService _emailService;
        private readonly IServiceScopeFactory _scopeFactory;

        public ChargingSessionService(
            IUnitOfWork uow,
            IWalletService walletService,
            IHubContext<ChargingHub> hubContext,
            IChargingSimulationService simulationService,
            IEmailService emailService,
            IServiceScopeFactory scopeFactory)
        {
            _uow = uow;
            _walletService = walletService;
            _hubContext = hubContext;
            _simulationService = simulationService;
            _emailService = emailService;
            _scopeFactory = scopeFactory;
        }

        // CREATE / START session
        public async Task<ChargingSessionDto> CreateSessionAsync(CreateChargingSessionDto dto)
        {
            var chargingPost = await _uow.ChargingPosts.GetByIdAsync(dto.PostId);
            if (chargingPost == null) throw new Exception("Không tìm thấy trụ sạc");
            var powerKW = (double)chargingPost.PowerKW;
            var connectorType = chargingPost.ConnectorType;

            // snapshot isWalkIn
            bool isWalkIn = chargingPost.IsWalkIn;

            // choose start battery
            decimal startBatteryPercentage = new Random().Next(10, 36);

            // If there is an existing Idle session for same user + same post
            ChargingSession? existingIdle = null;
            if (!string.IsNullOrEmpty(dto.VehiclePlate))
            {
                // Có biển số → tìm đúng Idle theo biển số
                existingIdle = await _uow.ChargingSessions.FindIdleSessionForUserAtPost(dto.VehiclePlate, dto.PostId);
            }
            else
            {
                // Không có biển số → tìm Idle gần nhất ở trụ đó
                existingIdle = await _uow.ChargingSessions.FindLatestIdleSessionAtPostAsync(dto.PostId);
            }

            if (chargingPost.Status != PostStatus.Available &&
                existingIdle == null) throw new Exception("Trụ không sẵn sàng");

            if (!isWalkIn && existingIdle != null)
            {
                var end = existingIdle.EndTime ?? DateTime.UtcNow.AddHours(7);
                var minutes = (DateTime.UtcNow.AddHours(7) - end).TotalMinutes;
                if (minutes <= AppConstant.ChargingRules.IDLE_GRACE_MINUTES)
                {
                    // Complete old session (create receipt, payment) BEFORE creating new session
                    await CompleteSessionAsync(existingIdle.Id, false);
                    // Copy battery value
                    if (existingIdle.EndBatteryPercentage.HasValue)
                        startBatteryPercentage = existingIdle.EndBatteryPercentage.Value;
                }
                else
                {
                    // too late -> must rebook, block
                    throw new Exception("Phiên trước đã quá 15 phút ân hạn. Vui lòng đặt chỗ mới.");
                }
            }

            // For walk-in: if existing idle present, we do NOT auto-complete; but we will use EndBatteryPercentage as start if available
            if (isWalkIn && existingIdle != null)
            {
                existingIdle.Status = SessionStatus.Completed;
                existingIdle.CompletedTime = DateTime.UtcNow.AddHours(7);
                if (existingIdle.EndBatteryPercentage.HasValue)
                    startBatteryPercentage = existingIdle.EndBatteryPercentage.Value;
                if (existingIdle.VehicleId.HasValue)
                    dto.VehicleId = existingIdle.VehicleId.Value;
                if (!string.IsNullOrEmpty(existingIdle.VehiclePlate))
                    dto.VehiclePlate = existingIdle.VehiclePlate;
            }

            if (dto.VehicleId != null)
            {
                var vehicleModel = await _uow.Vehicles.GetVehicleByIdAsync((int)dto.VehicleId);
                dto.VehiclePlate = vehicleModel?.Plate ?? string.Empty;
            }

            // Build session
            var session = new ChargingSession
            {
                VehicleId = dto.VehicleId,
                VehiclePlate = dto.VehiclePlate,
                ChargingPostId = dto.PostId,
                ReservationId = dto.ReservationId,
                StartTime = DateTime.UtcNow.AddHours(7),
                Status = SessionStatus.Charging,
                EnergyConsumed = 0,
                Cost = 0,
                StartBatteryPercentage = startBatteryPercentage,
                IsWalkInSession = isWalkIn,
                IsPaid = false
            };

            // check wallet/subscription etc
            bool isFreeCharging = false;
            decimal walletBalance = 0;
            string? ownerId = null;
            double batteryCapacity = 0;

            if (isWalkIn && dto.VehicleId.HasValue)
            {
                var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(dto.VehicleId.Value);
                if (vehicle != null)
                {
                    batteryCapacity = vehicle.BatteryCapacityKWh;
                    ownerId = vehicle.OwnerId ?? null;
                }
            }

            if (!isWalkIn)
            {
                // require vehicle
                var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(dto.VehicleId!.Value);
                if (vehicle == null) throw new Exception("Không tìm thấy xe");
                batteryCapacity = vehicle.BatteryCapacityKWh;
                ownerId = vehicle.OwnerId;
                var walletDto = await _walletService.GetWalletForUserAsync(ownerId!);
                walletBalance = walletDto?.Balance ?? 0;

                if (ownerId != null)
                {
                    var activeSubscription = await _uow.DriverPackages.GetActiveSubscriptionForUserAsync(ownerId, vehicle.Type);
                    if (activeSubscription != null) isFreeCharging = true;

                    if (!isFreeCharging)
                    {
                        if (walletDto == null || walletDto.Balance < 50000)
                        {
                            throw new Exception("Số dư ví phải trên 50k để bắt đầu sạc");
                        }
                    }
                }
            }

            // persist
            session = await _uow.ChargingSessions.CreateAsync(session);
            await _uow.ChargingPosts.UpdateStatusAsync(chargingPost.Id, PostStatus.Occupied);
            await _uow.Complete();

            // start simulation
            await _simulationService.StartSimulationAsync(
                session.Id,
                batteryCapacity,
                isFreeCharging,
                guestMode: isWalkIn,
                chargerPowerKW: powerKW,
                connectorType: connectorType,
                initialPercentage: (double)session.StartBatteryPercentage,
                initialEnergy: 0,
                initialCost: 0,
                ownerId: ownerId,
                walletBalance: walletBalance
            );

            try
            {
                await _hubContext.Clients.Group($"session-{session.Id}")
                    .SendAsync("ReceiveSessionResumed", session.MapToDto());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ SignalR send failed: {ex.Message}");
            }

            return session.MapToDto();
        }

        // Stop charging (user pressed Stop) — sets Idle
        public async Task StopChargingAsync(int sessionId, StopReason stopReason = StopReason.ManualStop)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Không tìm thấy session");
            if (session.Status != SessionStatus.Charging) throw new Exception("Chỉ dừng khi đang sạc");

            if (_simulationService.IsRunning(sessionId))
            {
                await _simulationService.StopSimulationAsync(sessionId, setCompleted: true);
            }

            _uow.DetachAllEntities(); // thêm dòng này ngay trước khi reload
            session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Không tìm thấy session");
            session.Status = SessionStatus.Idle;
            session.EndTime = DateTime.UtcNow.AddHours(7);
            session.StopReason = stopReason;

            if (stopReason == StopReason.ReservationCompleted)
            {
                session.IdleFeeStartTime = session.EndTime;
            }
            else
            {
                session.IdleFeeStartTime = session.EndTime.Value.AddMinutes(AppConstant.ChargingRules.IDLE_GRACE_MINUTES);
            }

            _uow.ChargingSessions.Update(session);
            await _uow.Complete();

            // notify
            await _hubContext.Clients.Group($"session-{sessionId}")
                .SendAsync("ReceiveSessionStopped", sessionId, session.Status);
        }

        // Called by simulation when full (or you may call this from simulation flush logic)
        public async Task HandleSessionFullAsync(int sessionId)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) return;
            if (session.Status != SessionStatus.Charging) return;

            session.Status = SessionStatus.Idle;
            session.EndTime = DateTime.UtcNow.AddHours(7);
            session.StopReason = StopReason.BatteryFull;

            _uow.ChargingSessions.Update(session);
            await _uow.Complete();

            await _hubContext.Clients.Group($"session-{sessionId}")
                .SendAsync("ReceiveSessionFull", sessionId);
        }

        // Complete session: user pressed "Complete" (rời trụ) -> create receipt, payment, release post if allowed
        public async Task<ReceiptDto> CompleteSessionAsync(int sessionId, bool endReservation)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Không tìm thấy phiên sạc");
            if (session.Status == SessionStatus.Completed) throw new Exception("Phiên sạc đã hoàn tất");

            if (_simulationService.IsRunning(sessionId))
            {
                await _simulationService.StopSimulationAsync(sessionId, setCompleted: true);
            }

            // reload
            session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Không tìm thấy phiên sạc");
            int total = 0;

            // For walk-in
            Receipt receipt;
            if (session.IsWalkInSession)
            {
                // find unpaid sessions with same plate & same post (you can refine time window if needed)
                var allSessions = await _uow.ChargingSessions.GetAllAsync();
                var unpaid = (allSessions.Where(s =>
                    s.IsWalkInSession &&
                    !s.IsPaid &&
                    s.VehiclePlate == session.VehiclePlate &&
                    s.ChargingPostId == session.ChargingPostId)).ToList();

                // mark Completed for these sessions that are currently Idle/Charging
                foreach (var s in unpaid)
                {
                    if (s.Status != SessionStatus.Completed)
                    {
                        s.Status = SessionStatus.Completed;
                        s.CompletedTime = DateTime.UtcNow.AddHours(7);
                        _uow.ChargingSessions.Update(s);
                    }
                }

                await _uow.Complete();

                // compute totals
                decimal totalEnergyConsumed = (decimal)unpaid.Sum(x => x.EnergyConsumed);
                int totalCost = unpaid.Sum(x => x.Cost);
                int totalIdle = unpaid.Sum(x => x.IdleFee);
                int totalOverstay = unpaid.Sum(x => x.OverstayFee ?? 0);
                total = totalCost + totalIdle + totalOverstay;

                bool isAC = session.ChargingPost.ConnectorType == ConnectorType.Type2 || session.ChargingPost.ConnectorType == ConnectorType.VinEScooter;
                bool isDC = !isAC;

                var priceType = (session.IsWalkInSession, isAC) switch
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
                    Console.WriteLine($"⚠️ Pricing lookup failed for session {sessionId}: {px}");
                }

                receipt = new Receipt
                {
                    AppUserId = session.Vehicle?.OwnerId ?? null,
                    EnergyConsumed = totalEnergyConsumed,
                    EnergyCost = totalCost,
                    IdleStartTime = totalIdle > 0 ? session.IdleFeeStartTime : null,
                    IdleEndTime = totalIdle > 0 ? DateTime.UtcNow.AddHours(7) : null,
                    IdleFee = totalIdle,
                    OverstayFee = totalOverstay,
                    TotalCost = total,
                    PricingName = pricing?.Name ?? string.Empty,
                    PricePerKwhSnapshot = pricing!.PricePerKwh,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    Status = ReceiptStatus.Pending,
                    AppUser = session.Vehicle?.Owner,
                    PaymentMethod = session.Vehicle?.OwnerId != null ? "Ví tiền" : null
                };
                foreach (var s in unpaid)
                {
                    receipt.ChargingSessions.Add(s);
                }

                await _uow.ChargingPosts.UpdateStatusAsync(session.ChargingPostId, PostStatus.Available);
            }
            else // reservation session
            {
                decimal energyConsumed = (decimal)session.EnergyConsumed;
                int energyCost = session.Cost;
                int idle = session.IdleFee;
                int over = session.OverstayFee ?? 0;
                total = energyCost + idle + over;
                var discountAmount = 0;

                var driverPackage = await _uow.DriverPackages.GetActiveSubscriptionForUserAsync(session.Vehicle!.OwnerId!, session.Vehicle!.Type);
                if(driverPackage != null)
                {
                    discountAmount = total;
                    total = 0;
                }

                session.Status = SessionStatus.Completed;
                session.CompletedTime = DateTime.UtcNow.AddHours(7);
                session.IsPaid = true;

                _uow.ChargingSessions.Update(session);

                bool isAC = session.ChargingPost.ConnectorType == ConnectorType.Type2 || session.ChargingPost.ConnectorType == ConnectorType.VinEScooter;
                bool isDC = !isAC;

                var priceType = (session.IsWalkInSession, isAC) switch
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

                receipt = new Receipt
                {
                    AppUserId = session.Vehicle?.OwnerId ?? string.Empty,
                    EnergyConsumed = energyConsumed,
                    EnergyCost = energyCost,
                    IdleStartTime = idle > 0 ? session.IdleFeeStartTime : null,
                    IdleEndTime = idle > 0 ? DateTime.UtcNow.AddHours(7) : null,
                    IdleFee = idle,
                    OverstayFee = over,
                    TotalCost = total,
                    PricingName = pricing?.Name ?? string.Empty,
                    PricePerKwhSnapshot = pricing!.PricePerKwh,
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    Status = total == 0 ? ReceiptStatus.Paid : ReceiptStatus.Pending,
                    PackageId = driverPackage != null ? driverPackage.Package.Id : null,
                    DiscountAmount = discountAmount,
                    PaymentMethod = "Ví tiền"
                };
                receipt.ChargingSessions.Add(session);

                // release post only if reservation completed or null
                var post = await _uow.ChargingPosts.GetByIdAsync(session.ChargingPostId);
                if (post != null)
                {
                    await _uow.ChargingPosts.UpdateStatusAsync(post.Id, PostStatus.Available);
                }

                if(endReservation == true)
                {
                    if(session.Reservation != null)
                    {
                        var reservation = session.Reservation.Status = ReservationStatus.Completed;
                    }
                }
            }
            await _uow.Receipts.AddAsync(receipt);
            await _uow.Complete();

            // Wallet charge for reservation members
            if (session.Vehicle?.OwnerId != null && total > 0)
            {
                var payResult = await _walletService.PayingChargeWalletAsync(receipt.Id, session.Vehicle.OwnerId, total, TransactionType.PayCharging);
            }

            // send email if available
            if (session.Vehicle?.Owner != null && !string.IsNullOrEmpty(session.Vehicle.Owner.Email))
            {
                var dto = receipt.MapToDto();
                await _emailService.SendChargingReceiptAsync(session.Vehicle.Owner.Email, dto);
            }

            // signal
            await _hubContext.Clients.Group($"session-{sessionId}")
                .SendAsync("ReceiveSessionCompleted", sessionId, receipt.MapToDto());

            var receiptDto = receipt.MapToDto();
            if (session.Vehicle!.OwnerId != null)
            {
                receiptDto.ShouldSuggestRegistration = false;
            }
            else
            {
                receiptDto.ShouldSuggestRegistration = true;
            }

            return receiptDto;
        }

        // Keep UpdatePlate logic (from your original code) — unchanged except ensure StopReason/IsWalkIn interactions later
        public async Task<ChargingSession> UpdatePlateAsync(int sessionId, string plate)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Không tìm thấy session");
            if (session.VehicleId != null) throw new Exception("Chỉ dành cho vãng lai (VehicleId = null)");
            if (session.Status != SessionStatus.Charging) throw new Exception("Chỉ update khi đang sạc");

            var chargingPost = await _uow.ChargingPosts.GetByIdAsync(session.ChargingPostId);
            if (chargingPost == null) throw new Exception("Không tìm thấy trụ sạc");

            var powerKW = (double)chargingPost.PowerKW;
            var connectorType = chargingPost.ConnectorType;

            session.VehiclePlate = plate;

            double batteryCapacity = 1.0;
            bool isFreeCharging = false;
            bool guestMode = true;

            var vehicle = await _uow.Vehicles.GetByPlateAsync(plate);
            if (vehicle != null)
            {
                session.VehicleId = vehicle.Id;
                session.Vehicle = vehicle;
                batteryCapacity = vehicle.BatteryCapacityKWh;
            }
            else
            {
                var compatibleModels = await _uow.VehicleModels.GetCompatibleModelsAsync(connectorType, (decimal) powerKW);
                if (compatibleModels?.Any() != true) throw new Exception("Không tìm thấy mẫu xe tương thích");

                var random = new Random();
                var compatibleModelsList = compatibleModels.ToList();
                var randomModel = compatibleModelsList[random.Next(compatibleModelsList.Count)];
                batteryCapacity = randomModel.BatteryCapacityKWh;

                var newVehicle = new Vehicle
                {
                    Model = randomModel.Model,
                    Type = randomModel.Type,
                    BatteryCapacityKWh = randomModel.BatteryCapacityKWh,
                    MaxChargingPowerKW = (double)(randomModel.Type == VehicleType.Car ? randomModel.MaxChargingPowerDC_KW : randomModel.MaxChargingPowerKW),
                    ConnectorType = randomModel.ConnectorType,
                    Plate = plate,
                    OwnerId = null
                };
                var vehicleModel = await _uow.Vehicles.AddVehicleAsync(newVehicle);
                await _uow.Complete();
                session.VehicleId = vehicleModel.Id;
                session.Vehicle = await _uow.Vehicles.GetVehicleByIdAsync((int)session.VehicleId);
            }
            await _uow.Complete();

            if (_simulationService.IsRunning(sessionId))
            {
                bool setCompleted = false;
                await _simulationService.StopSimulationAsync(sessionId, setCompleted);

                await Task.Delay(1000);

                // DÙNG SCOPE MỚI ĐỂ LOAD FRESH DATA
                using var loadScope = _scopeFactory.CreateScope();
                var loadUow = loadScope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var freshSession = await loadUow.ChargingSessions.GetByIdAsync(sessionId);

                if (freshSession == null) throw new Exception("Session không tồn tại");

                // DÙNG freshSession, KHÔNG DÙNG session cũ (có cache)
                double currentPercentage = (double)(freshSession.EndBatteryPercentage ?? freshSession.StartBatteryPercentage);

                double chargedPercentage = (double)(currentPercentage - (double)session.StartBatteryPercentage);
                if (chargedPercentage < 0) chargedPercentage = 0;

                double energyConsumed = (batteryCapacity > 0) ? ((chargedPercentage / 100.0) * batteryCapacity) : 0.0;
                energyConsumed = Math.Round(energyConsumed, 3, MidpointRounding.AwayFromZero);

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
                int pricePerKWh = (int)(pricing?.PricePerKwh ?? 0);
                int cost = (int)Math.Round(energyConsumed * pricePerKWh, MidpointRounding.AwayFromZero);

                freshSession.EnergyConsumed = energyConsumed;
                freshSession.Cost = cost;
                await loadUow.Complete();

                Console.WriteLine($"[FRESH LOAD] EndBattery: {freshSession.EndBatteryPercentage}, Energy: {energyConsumed}, Cost: {cost}");

                double percentageStep = powerKW switch
                {
                    <= 1.2 => 0.1,   // sạc chậm Type2
                    <= 11 => 0.2,   // sạc chậm Type2
                    <= 60 => 0.5,   // sạc nhanh 60kW
                    <= 150 => 1.0,  // siêu nhanh 150kW
                    <= 250 => 1.5,  // ultra fast 250kW
                    _ => 0.2
                };

                await _simulationService.StartSimulationAsync(
                    sessionId,
                    batteryCapacity,
                    isFreeCharging,
                    guestMode: guestMode,
                    chargerPowerKW: powerKW,
                    connectorType: connectorType,
                    initialPercentage: currentPercentage,
                    initialEnergy: energyConsumed,
                    initialCost: cost,
                    ownerId: null,
                    walletBalance: 0
                );

                var update = new EnergyUpdateDto
                {
                    SessionId = sessionId,
                    BatteryPercentage = Math.Round(currentPercentage, 1),
                    EnergyConsumed = energyConsumed,
                    Cost = cost,
                    TimeRemain = currentPercentage >= 100 ? 0 : (int)Math.Ceiling((100 - currentPercentage) / percentageStep / 60.0),
                    IsTempMode = false
                };

                await _hubContext.Clients.Group($"session-{sessionId}")
                    .SendAsync("ReceiveSessionUpdate", update);
            }

            return session;
        }
    }
}