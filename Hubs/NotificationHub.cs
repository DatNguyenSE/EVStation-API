using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    [Authorize] // Yêu cầu xác thực (đăng nhập) để kết nối vào Hub này
    public class NotificationHub : Hub<INotificationClient>
    {
        public override async Task OnConnectedAsync()
        {
            // Tự động thêm Admin vào group "Admins"
            if (Context.User.IsInRole(AppConstant.Roles.Admin))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }
            await base.OnConnectedAsync();
            Console.WriteLine($"---> Client connected: {Context.ConnectionId}");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // (Không bắt buộc) Xử lý khi user disconnect
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"---> Client disconnected: {Context.ConnectionId}");
        }
        
        // Bạn có thể định nghĩa các hàm để Client (Angular) gọi ngược lên Server
        // Ví dụ:
        // public async Task SendMessageToAdmin(string message)
        // {
        //     var userId = Context.UserIdentifier;
        //     // Gửi message này đến tất cả user trong group "Admins"
        //     await Clients.Group("Admins").SendAsync("ReceiveAdminMessage", userId, message);
        // }
    }
}