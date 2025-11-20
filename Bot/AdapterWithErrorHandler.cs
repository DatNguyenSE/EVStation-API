using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;

namespace API.Bot
{
    public class AdapterWithErrorHandler : CloudAdapter
    {
        public AdapterWithErrorHandler(BotFrameworkAuthentication auth, ILogger<IBotFrameworkHttpAdapter> logger, ConversationState conversationState = default)
        : base(auth, logger)
        {
            OnTurnError = async (turnContext, exception) =>
            {
                // Ghi log lỗi
                logger.LogError(exception, $"[OnTurnError] Lỗi bot: {exception.Message}");

                // Gửi thông báo lỗi cho người dùng
                await turnContext.SendActivityAsync("Đã có lỗi xảy ra. Vui lòng thử lại sau.");
            };
        }
    }
}