using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class ReportHub : Hub
    {
        // Khi client kết nối -> thêm vào group theo userId
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnConnectedAsync();
        }

        // Khi client ngắt kết nối -> xoá khỏi group theo userId
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        // 🧾 Staff gửi thông báo tới Admin
        public async Task SendReportToAdmin(string adminId, object report)
        {
            var notification = new
            {
                Type = "ReportCreated",
                Title = "Báo cáo mới được tạo",
                Message = "Nhân viên vừa gửi một báo cáo mới.",
                Data = report,
                CreatedAt = DateTime.UtcNow
            };

            await Clients.Group(adminId).SendAsync("ReceiveNotification", notification);
        }

        // 🧰 Admin giao công việc cho Technician
        public async Task AssignTaskToTechnician(string technicianId, object task)
        {
            var notification = new
            {
                Type = "TaskAssigned",
                Title = "Công việc mới được giao",
                Message = "Bạn có nhiệm vụ mới cần xử lý.",
                Data = task,
                CreatedAt = DateTime.UtcNow
            };

            await Clients.Group(technicianId).SendAsync("ReceiveNotification", notification);
        }

        // ✅ Technician hoàn tất công việc -> thông báo lại cho Admin
        public async Task NotifyAdminTaskCompleted(string adminId, object report)
        {
            var notification = new
            {
                Type = "TaskCompleted",
                Title = "Công việc đã hoàn thành",
                Message = "Kỹ thuật viên đã hoàn tất một công việc bảo trì.",
                Data = report,
                CreatedAt = DateTime.UtcNow
            };

            await Clients.Group(adminId).SendAsync("ReceiveNotification", notification);
        }
    }
}
