using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication2.Models.Commands
{
    public class ReportAdminCommand : Command
    {
        public override string Name => @"/report";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            if (message.Text.Length > 9)
            {
                await botClient.SendTextMessageAsync(304003038, 
                    $@"Сообщение от @{message.From.Username}"+ Environment.NewLine + $@"Сообщение : {message.Text.Replace("/report", "")}");
                await botClient.SendTextMessageAsync(message.Chat.Id, "Ваше сообщение успешно отправлено администрации!");
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Минимальное количество символов - 10");
            }
            
        }
    }
}