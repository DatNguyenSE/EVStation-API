using System.Security.Claims;
using API.DTOs.ChargingSession;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class ConnectCharging : Hub
{
    public override async Task OnConnectedAsync()
    {
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Operator";

        if (role == "Operator")
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Operator");
            Console.WriteLine($"[Operator Connected] {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var role = Context.User?.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "Operator")
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Operator");
            Console.WriteLine($"[Operator Disconnected] {Context.ConnectionId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    //  Khi trụ bắt đầu hoạt động
    public async Task NotifyConnect(ChargingEventDto data)
    {
        Console.WriteLine($"[NotifyConnect] PostId={data.PostId}, SessionId={data.SessionId}");
        await Clients.Group("Operator").SendAsync("ConnectCharging", data);
    }

    //  Khi trụ ngắt kết nối
    public async Task NotifyDisconnect(ChargingEventDto data)
    {
        Console.WriteLine($"[NotifyDisconnect] PostId={data.PostId}, SessionId={data.SessionId}");
        await Clients.Group("Operator").SendAsync("DisconnectCharging", data);
    }
}
