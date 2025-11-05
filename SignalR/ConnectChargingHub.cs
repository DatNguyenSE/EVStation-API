using System.Security.Claims;
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

    //  Khi trụ sạc bắt đầu hoạt động
    public async Task NotifyConnect(string postId)
    {
        Console.WriteLine($"[NotifyConnect] Trụ {postId} đã bắt đầu hoạt động.");
        await Clients.Group("Operator").SendAsync("ConnectCharging", postId);
    }

    //  Khi trụ sạc ngắt kết nối
    public async Task NotifyDisconnect(string postId)
    {
        Console.WriteLine($"[NotifyDisconnect] Trụ {postId} đã ngắt kết nối.");
        await Clients.Group("Operator").SendAsync("DisconnectCharging", postId);
    }
}
