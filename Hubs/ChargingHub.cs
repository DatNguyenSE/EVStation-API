using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.ChargingSession;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class ChargingHub : Hub
    {
        private readonly ILogger _logger;

        public ChargingHub(ILogger<ChargingHub> logger)
        {
            _logger = logger;
        }

        // Khi client (FE) muốn nhận cập nhật của 1 phiên sạc cụ thể
        public async Task JoinSessionGroup(int sessionId)
        {
            _logger.LogInformation($"JoinSessionGroup called with sessionId={sessionId}");
            await Groups.AddToGroupAsync(Context.ConnectionId, $"session-{sessionId}");
            System.Console.WriteLine("##################################################");
        }

        // Khi client rời khỏi group
        public async Task LeaveSessionGroup(int sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"session-{sessionId}");
        }
    }
}