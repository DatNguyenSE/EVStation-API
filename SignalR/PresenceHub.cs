using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class PresenceHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.Others.SendAsync("UserOnline", Context.User?.FindFirstValue(ClaimTypes.GivenName));
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Clients.Others.SendAsync("UserOffline ", Context.User?.FindFirstValue(ClaimTypes.GivenName));
        await base.OnDisconnectedAsync(exception);
    }
}