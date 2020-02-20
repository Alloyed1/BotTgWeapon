using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
    public class ReportCommand : Command
    {
        public override string Name => @"/reportAdmin";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            await botClient.SendTextMessageAsync(466739920, message.Text);
        }
    }
}