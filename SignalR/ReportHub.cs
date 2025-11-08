using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace API.SignalR;

    public class ReportHub : Hub
    {
        // Khi staff tạo report mới, server gọi hàm này để gửi tới tất cả admin
        public async Task SendReport(object report)
        {
            await Clients.Group("Admins").SendAsync("ReceiveReport", report);
        }

        // Khi client kết nối, nếu là admin thì add vào group Admins
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user?.IsInRole("Admin") == true)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }

            await base.OnConnectedAsync();
        }
    }

