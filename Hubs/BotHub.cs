using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class BotHub : Hub
    {
        // Phương thức này được Angular gọi khi người dùng gửi tin nhắn
        public async Task SendMessage(string userMessage)
        {
            // ----- SAO CHÉP LOGIC TỪ SimpleEvBot CỦA BẠN VÀO ĐÂY -----

            var message = userMessage.ToLower().Trim();
            string botResponse = "";

            switch (message)
            {
                case "hello":
                case "hi":
                case "chào":
                    botResponse = "Chào bạn! Tôi là bot hỗ trợ trạm sạc. Bạn cần giúp gì?";
                    break;

                case "trạm sạc":
                case "tìm trạm":
                    botResponse = "Hiện tại bạn có thể tìm trạm trên bản đồ. Bạn muốn tôi hiển thị các trạm gần nhất không?";
                    break;

                case "hỗ trợ":
                    botResponse = "Bạn có thể báo cáo sự cố hoặc gọi hotline 123456 để được hỗ trợ.";
                    break;

                default:
                    botResponse = $"Tôi chưa hiểu ý bạn. Bạn đã nói: '{userMessage}'";
                    break;
            }
            // ------------------------------------------------------

            // Gửi câu trả lời *chỉ* cho người dùng đã gọi
            await Clients.Caller.SendAsync("ReceiveMessage", botResponse);
        }

        // Gửi tin nhắn chào mừng khi người dùng kết nối
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Chào mừng bạn đến với hệ thống quản lý trạm sạc!");
            await base.OnConnectedAsync();
        }
    }
}