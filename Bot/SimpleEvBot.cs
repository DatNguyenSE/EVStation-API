using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace API.Bot
{
    public class SimpleEvBot : ActivityHandler
    {
        // Override phương thức này để xử lý tin nhắn
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Lấy tin nhắn của người dùng và chuyển về chữ thường
            var userMessage = turnContext.Activity.Text.ToLower().Trim();
            string botResponse = "";

            // ----- ĐÂY LÀ LOGIC ĐƠN GIẢN CỦA BẠN -----
            switch (userMessage)
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
                    // Lặp lại lời người dùng (Echo bot) nếu không hiểu
                    botResponse = $"Tôi chưa hiểu ý bạn. Bạn đã nói: '{turnContext.Activity.Text}'";
                    break;
            }
            // ------------------------------------------

            // Gửi tin nhắn trả lời lại cho người dùng
            await turnContext.SendActivityAsync(MessageFactory.Text(botResponse), cancellationToken);
        }

        // (Tùy chọn) Gửi tin nhắn chào mừng khi có người dùng mới tham gia
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text("Chào mừng bạn đến với hệ thống quản lý trạm sạc!"), cancellationToken);
                }
            }
        }
    }
}