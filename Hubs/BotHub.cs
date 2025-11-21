using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace API.Hubs
{
    public class BotHub : Hub
    {
        // Phương thức này được Angular gọi khi người dùng gửi tin nhắn
        public async Task SendMessage(string userMessage)
        {
            // ----- SAO CHÉP LOGIC TỪ SimpleEvBot CỦA BẠN VÀO ĐÂY -----

            var raw = userMessage ?? "";
            var processed = NormalizeForCompare(raw);
            string botResponse = "";

            if (processed.Contains("chao") || processed.Contains("hello") || processed.Contains("hi"))
            {
                botResponse = "Chào bạn! Tôi là bot hỗ trợ trạm sạc. Bạn cần giúp gì?";
            }
            else if (processed.Contains("tim tram") || processed.Contains("tram sac") || processed.Contains("huong dan tim tram") || processed.Contains("huong dan tim"))
            {
                botResponse = "Hiện tại bạn có thể tìm trạm trên bản đồ. Bạn muốn tôi hiển thị các trạm gần nhất không?";
            }
            else if (processed.Contains("ho tro") || processed.Contains("ho-tro"))
            {
                botResponse = "Bạn có thể báo cáo sự cố hoặc gọi hotline 123456 để được hỗ trợ.";
            }
            else
            {
                botResponse = $"Tôi chưa hiểu ý bạn. Bạn đã nói: '{userMessage}'";
            }
            // ------------------------------------------------------

            // Gửi câu trả lời *chỉ* cho người dùng đã gọi
            await Clients.Caller.SendAsync("ReceiveMessage", botResponse);
        }

        private string NormalizeForCompare(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            // normalize and remove diacritics
            var s = input.Normalize(NormalizationForm.FormC).Replace('\u00A0', ' ');
            var formD = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var ch in formD)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(ch);
            }
            var noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);
            var lower = noDiacritics.ToLowerInvariant();
            var cleaned = Regex.Replace(lower, "[^\\p{L}\\p{Nd}\\s]", " ");
            var collapsed = Regex.Replace(cleaned, "\\s+", " ").Trim();
            return collapsed;
        }

        // Gửi tin nhắn chào mừng khi người dùng kết nối
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", "Chào mừng bạn đến với hệ thống quản lý trạm sạc!");
            await base.OnConnectedAsync();
        }
    }
}