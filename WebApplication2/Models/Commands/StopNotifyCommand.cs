using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
    public class StopNotifyCommand : CommandMessage
    {
        public override string Name => @"Откл. авто уведомление";
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var chatId = message.Chat.Id;
            using (var db = new DbNorthwind())
            {
                var query = await db.LastQuery
                    .FirstOrDefaultAsync(w => w.ChatId == chatId.ToString());
                if (query != null)
                {
                    if (query.IsWatching == 1)
                    {
                        
                        ReplyKeyboardMarkup ReplyKeyboard = new[]
                        {
                            new []{"Помощь", "Поиск по категориям"}
                        };
                        ReplyKeyboard.ResizeKeyboard = true;
                        
                        query.IsWatching = 0;
                        await db.UpdateAsync(query);
                        await botClient.SendTextMessageAsync(chatId, $"Автоматические уведомления по запрсу \"{query.Query}\" были отключены.\nМожете делать новые запросы", replyMarkup:ReplyKeyboard );
                    }
                }
            }
        }
    }
}