using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LinqToDB;
using Microsoft.Extensions.Logging;
using WebApplication2.Controllers;


namespace WebApplication2.Models.Commands
{
    public class StopCommand : Command
    {
        public override string Name => "Остановить";
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var chatId = message.Chat.Id;
            await SetQueryNull(chatId.ToString());
            await botClient.SendTextMessageAsync(chatId, "Отправка сообщений по этому запросу приостановлена", replyMarkup: null);
        }
        async Task SetQueryNull(string chatId)
        {
            using(var db = new DbNorthwind())
            {
                await db.LastQuery.Where(w => w.ChatId == chatId).Set(s => s.Query, String.Empty).UpdateAsync();
            }
        }
    }
}