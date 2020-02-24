using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
    public class CountCommand : CommandMessage
    {
        public override string Name => @"/count";
        
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var chatId = message.Chat.Id;
            using var db = new DbNorthwind();

            var count = await db.WeaponList.CountAsync();
            await botClient.SendTextMessageAsync(chatId, $"Количество {count}", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
        
    }
}