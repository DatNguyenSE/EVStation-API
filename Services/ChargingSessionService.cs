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

namespace API.Services
{
    public class ChargingSessionService : IChargingSessionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IWalletService _walletService;
        private readonly IHubContext<ChargingHub> _hubContext;
        private readonly IChargingSimulationService _simulationService;

        public ChargingSessionService(
            IUnitOfWork uow,
            IWalletService walletService,
            IHubContext<ChargingHub> hubContext,
            IChargingSimulationService simulationService)
        {
            _uow = uow;
            _walletService = walletService;
            _hubContext = hubContext;
            _simulationService = simulationService;
        }

        // Tạo session mới
        public async Task<ChargingSessionDto> CreateSessionAsync(CreateChargingSessionDto dto)
        {
            var chargingPost = await _uow.ChargingPosts.GetByIdAsync(dto.PostId);
            if (chargingPost == null) throw new Exception("Không tìm thấy trụ sạc");

            var random = new Random();
            var startBatteryPercentage = random.Next(10, 36);

            if (!chargingPost.IsWalkIn)
            {
                if (dto.VehicleId == null) throw new Exception("Cần VehicleId cho trụ này");

                var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(dto.VehicleId.Value);
                if (vehicle == null) throw new Exception("Không tìm thấy xe");

                bool isFreeCharging = false;
                var ownerId = vehicle.OwnerId;
                var walletDto = await _walletService.GetWalletForUserAsync(ownerId);

                if (ownerId != null)
                {
                    var activeSubscription = await _uow.DriverPackages.GetActiveSubscriptionForUserAsync(ownerId, vehicle.Type);
                    if (activeSubscription != null)
                    {
                        isFreeCharging = true;
                    }

                    if (!isFreeCharging)
                    {
                        if (walletDto == null || walletDto.Balance < 50000)
                        {
                            throw new Exception("Số dư ví phải trên 50k để bắt đầu sạc");
                        }
                    }
                }

                var session = new ChargingSession
                {
                    VehicleId = dto.VehicleId,
                    VehiclePlate = vehicle.Plate,
                    PostId = dto.PostId,
                    ReservationId = dto.ReservationId,
                    StartTime = DateTime.UtcNow,
                    Status = SessionStatus.Charging,
                    EnergyConsumed = 0,
                    Cost = 0,
                    StartBatteryPercentage = startBatteryPercentage
                };

                session = await _uow.ChargingSessions.CreateAsync(session);
                await _uow.ChargingPosts.UpdateStatusAsync(chargingPost.Id, PostStatus.Occupied);
                await _uow.Complete();

                await _simulationService.StartSimulationAsync(session.Id, vehicle.BatteryCapacityKWh, isFreeCharging, guestMode: false,
                                                                initialPercentage: session.StartBatteryPercentage, initialEnergy: 0, initialCost: 0,
                                                                ownerId: ownerId, walletBalance: walletDto?.Balance ?? 0);

                return session.MapToDto();
            }
            else
            {
                var session = new ChargingSession
                {
                    VehicleId = null,
                    VehiclePlate = "",
                    PostId = dto.PostId,
                    StartTime = DateTime.UtcNow,
                    Status = SessionStatus.Charging,
                    EnergyConsumed = 0,
                    Cost = 0,
                    StartBatteryPercentage = startBatteryPercentage
                };

                session = await _uow.ChargingSessions.CreateAsync(session);
                await _uow.ChargingPosts.UpdateStatusAsync(chargingPost.Id, PostStatus.Occupied);
                await _uow.Complete();

                await _simulationService.StartSimulationAsync(session.Id, batteryCapacity: 0, isFreeCharging: false, guestMode: true,
                                                                initialPercentage: session.StartBatteryPercentage, initialEnergy: 0, initialCost: 0,
                                                                ownerId: null, walletBalance: 0);

                return session.MapToDto();
            }
        }

        public async Task<ChargingSessionDto> EndSessionAsync(int sessionId)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Không tìm thấy session");

            if (session.Status != SessionStatus.Charging && session.Status != SessionStatus.Full)
            {
                throw new Exception("Không thể kết thúc ở trạng thái hiện tại");
            }

            session.Status = SessionStatus.Completed;
            session.EndTime = DateTime.UtcNow;

            _uow.ChargingSessions.Update(session);
            await _uow.Complete();

            await _simulationService.StopSimulationAsync(sessionId, setCompleted: true);

            var updatedSession = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            await _uow.ChargingPosts.UpdateStatusAsync(session.PostId, PostStatus.Available);

            await _hubContext.Clients.Group($"session-{sessionId}")
                .SendAsync("ReceiveSessionEnded", sessionId, updatedSession.Status);

            await _uow.Complete();

            return updatedSession.MapToDto();
        }

        public async Task<ChargingSession> UpdatePlateAsync(int sessionId, string plate)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Không tìm thấy session");
            if (session.VehicleId != null) throw new Exception("Chỉ dành cho vãng lai (VehicleId = null)");
            if (session.Status != SessionStatus.Charging) throw new Exception("Chỉ update khi đang sạc");

            session.VehiclePlate = plate;

            double batteryCapacity = 1.0;
            bool isFreeCharging = false;
            bool newGuestMode = true;

            var vehicle = await _uow.Vehicles.GetByPlateAsync(plate);
            if (vehicle != null)
            {
                session.VehicleId = vehicle.Id;
                batteryCapacity = vehicle.BatteryCapacityKWh;
            }
            else
            {
                var chargingPost = await _uow.ChargingPosts.GetByIdAsync(session.PostId);
                var compatibleModels = await _uow.VehicleModels.GetCompatibleModelsAsync(chargingPost.ConnectorType);
                if (compatibleModels?.Any() != true) throw new Exception("Không tìm thấy mẫu xe tương thích");

                var random = new Random();
                var compatibleModelsList = compatibleModels.ToList();
                var randomModel = compatibleModelsList[random.Next(compatibleModelsList.Count)];
                batteryCapacity = randomModel.BatteryCapacityKWh;
            }

            _uow.ChargingSessions.Update(session);
            await _uow.Complete();

            if (_simulationService.IsRunning(sessionId))
            {
                bool setCompleted = false;
                await _simulationService.StopSimulationAsync(sessionId, setCompleted);

                session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
                if (session == null) throw new Exception("Session biến mất sau khi dừng simulation");

                // compute elapsed seconds since StartTime
                double elapsedSeconds = (DateTime.UtcNow - session.StartTime).TotalSeconds;
                if (elapsedSeconds < 0) elapsedSeconds = 0;
                const double ratePerSecond = 0.5; // % per second

                double currentPercentage = session.StartBatteryPercentage + elapsedSeconds * ratePerSecond;
                if (currentPercentage > 100) currentPercentage = 100;

                // làm tròn 1 chữ số
                currentPercentage = Math.Round(currentPercentage * 2.0, MidpointRounding.AwayFromZero) / 2.0;
                currentPercentage = Math.Round(currentPercentage, 1, MidpointRounding.AwayFromZero);

                double chargedPercentage = currentPercentage - session.StartBatteryPercentage;
                if (chargedPercentage < 0) chargedPercentage = 0;

                double energyConsumed = (batteryCapacity > 0) ? ((chargedPercentage / 100.0) * batteryCapacity) : 0.0;
                energyConsumed = Math.Round(energyConsumed, 3, MidpointRounding.AwayFromZero);

                int pricePerKWh = 5000; // giá vãng lai
                int cost = (int)Math.Round(energyConsumed * pricePerKWh, MidpointRounding.AwayFromZero);

                session.EnergyConsumed = energyConsumed;
                session.Cost = cost;

                _uow.ChargingSessions.Update(session);
                await _uow.Complete();

                await _simulationService.StartSimulationAsync(
                    sessionId,
                    batteryCapacity,
                    isFreeCharging,
                    guestMode: newGuestMode,
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
                    TimeRemain = currentPercentage >= 100 ? 0 : (int)Math.Ceiling((100 - currentPercentage) / ratePerSecond / 60.0),
                    IsTempMode = false
                };

                await _hubContext.Clients.Group($"session-{sessionId}")
                    .SendAsync("ReceiveSessionUpdate", update);
            }

            return session;
        }
    }
}