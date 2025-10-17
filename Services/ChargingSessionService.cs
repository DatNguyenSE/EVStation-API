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

// check người dùng có mua gói không => không tính phí


namespace API.Services
{
    public class ChargingSessionService : IChargingSessionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IWalletService _walletService; // Thêm để kiểm tra ví
        private readonly IHubContext<ChargingHub> _hubContext;
        private readonly IChargingSimulationService _simulationService;

        public ChargingSessionService(
            IUnitOfWork uow,
            IWalletService walletService, // Inject IWalletService
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
            if (dto.VehicleId == null && string.IsNullOrEmpty(dto.VehiclePlate))
            {
                throw new Exception("Cần VehicleId hoặc VehiclePlate");
            }

            var session = new ChargingSession
            {
                VehicleId = dto.VehicleId,
                VehiclePlate = dto.VehiclePlate ?? string.Empty,
                PostId = dto.PostId,
                ReservationId = dto.ReservationId,
                StartTime = DateTime.UtcNow,
                Status = SessionStatus.Charging,
                EnergyConsumed = 0,
                Cost = 0
            };

            double batteryCapacity = 0;
            string plate = string.Empty;
            var random = new Random();

            if (dto.VehicleId.HasValue)
            {
                // Xe của người có tài khoản: Lấy thông tin thực từ DB và random pin
                var vehicle = await _uow.Vehicles.GetVehicleByIdAsync(dto.VehicleId.Value);
                if (vehicle == null) throw new Exception("Không tìm thấy xe");

                batteryCapacity = vehicle.BatteryCapacityKWh; // Dung lượng pin thực
                plate = vehicle.Plate;
            }
            else if (!string.IsNullOrEmpty(dto.VehiclePlate))
            {
                // Khách vãng lai: Kiểm tra nếu biển số tồn tại trong DB
                var vehicle = await _uow.Vehicles.GetByPlateAsync(dto.VehiclePlate);
                if (vehicle != null)
                {
                    // Nếu tồn tại, dùng thông tin xe đó
                    // model = vehicle.Model;
                    batteryCapacity = vehicle.BatteryCapacityKWh;
                    plate = vehicle.Plate;
                }
                else
                {
                    var chargingPost = await _uow.ChargingPosts.GetByIdAsync(session.PostId);
                    if (chargingPost == null) throw new Exception("Không tìm thấy trụ sạc");

                    // Nếu không tồn tại, random thông tin
                    // Lấy danh sách mẫu xe tương thích với ConnectorType của trụ
                    var compatibleModels = await _uow.VehicleModels.GetCompatibleModelsAsync(chargingPost.ConnectorType);
                    if (compatibleModels == null || !compatibleModels.Any())
                        throw new Exception("Không tìm thấy mẫu xe tương thích với trụ sạc.");
                    var compatibleModelsList = compatibleModels.ToList();
                    // Random chọn một mẫu xe
                    var randomModel = compatibleModelsList[random.Next(compatibleModelsList.Count)];
                    // model = randomModel.Model;
                    batteryCapacity = randomModel.BatteryCapacityKWh;


                }
            }

            // Random % pin hiện tại (10-80%) cho cả hai trường hợp
            session.StartBatteryPercentage = random.Next(10, 36);
            session.EndBatteryPercentage = session.StartBatteryPercentage;
            session.VehiclePlate = plate;

            session = await _uow.ChargingSessions.CreateAsync(session);
            await _uow.Complete();
            await _simulationService.StartSimulationAsync(session.Id, batteryCapacity);

            return session.MapToDto();
        }

        // Cập nhật năng lượng và pin, kiểm tra ví
        public async Task UpdateEnergyAsync(EnergyUpdateDto update)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(update.SessionId);
            if (session == null) throw new Exception("Không tìm thấy session");

            if (session.Status != SessionStatus.Charging && session.Status != SessionStatus.Full)
            {
                throw new Exception("Không thể cập nhật ở trạng thái hiện tại");
            }

            // Cập nhật fields
            session.EnergyConsumed = update.EnergyConsumed;
            session.EndBatteryPercentage = (float)update.BatteryPercentage;
            session.Cost = (int)update.Cost;

            // Kiểm tra số dư ví nếu có VehicleId (user đăng ký)
            if (session.VehicleId.HasValue)
            {
                var ownerId = (await _uow.Vehicles.GetVehicleByIdAsync(session.VehicleId.Value))?.OwnerId;
                if (ownerId != null)
                {
                    var walletDto = await _walletService.GetWalletForUserAsync(ownerId);
                    if (walletDto != null && session.Cost > walletDto.Balance)
                    {
                        // Phí sạc vượt quá số dư, ngừng sạc
                        session.Status = SessionStatus.StoppedDueToInsufficientFunds;
                        _uow.ChargingSessions.Update(session);
                        await _hubContext.Clients.Group($"session-{update.SessionId}")
                            .SendAsync("ReceiveSessionStopped", update.SessionId, "Phí sạc vượt quá số dư ví");
                        return; // Kết thúc hàm, không tiếp tục cập nhật
                    }
                }
            }

            // Nếu pin đầy, chuyển status sang Full
            if (update.BatteryPercentage >= 100)
            {
                session.Status = SessionStatus.Full;
            }

            _uow.ChargingSessions.Update(session);

            // Gửi cập nhật realtime qua SignalR đến group của session
            await _hubContext.Clients.Group($"session-{update.SessionId}")
                .SendAsync("ReceiveSessionUpdate", update);
            await _uow.Complete();
        }

        // Kết thúc session
        public async Task<ChargingSessionDto> EndSessionAsync(int sessionId)
        {
            var session = await _uow.ChargingSessions.GetByIdAsync(sessionId);
            if (session == null) throw new Exception("Không tìm thấy session");

            if (session.Status != SessionStatus.Charging && session.Status != SessionStatus.Full)
            {
                throw new Exception("Không thể kết thúc ở trạng thái hiện tại");
            }

            // Chuyển sang Completed mà không trừ tiền (theo yêu cầu)
            session.Status = SessionStatus.Completed;
            session.EndTime = DateTime.UtcNow;

            _uow.ChargingSessions.Update(session);

            await _simulationService.StopSimulationAsync(sessionId);


            await _hubContext.Clients.Group($"session-{sessionId}")
                .SendAsync("ReceiveSessionEnded", sessionId, session.Status);

            await _uow.Complete();

            return session.MapToDto();
        }
    }
}