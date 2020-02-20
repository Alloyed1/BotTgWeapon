using System;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
    public class NotifyComand : Command
    {
        public override string Name => "Вкл.авто уведомление";
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var chatId = message.Chat.Id;
            using(var db = new DbNorthwind())
            {

                
                ReplyKeyboardMarkup ReplyKeyboard = new[]
                {
                    new[] { "Отключить автоматические уведомления", "Помощь"},
                };
                ReplyKeyboard.ResizeKeyboard = true;
                
                await db.LastQuery
                    .Where(w => w.ChatId == chatId.ToString())
                    .Set(s => s.IsWatching, 1)
                    .Set(s => s.StartWatchTime, DateTime.Now)
                    .UpdateAsync();

                var query = (await db.LastQuery
                    .FirstOrDefaultAsync(s => s.ChatId == chatId.ToString())).Query;
                await botClient.SendTextMessageAsync(chatId, $"Отслеживание новых объявлений по запросу \"{query}\" успешно включено", replyMarkup: ReplyKeyboard);
            }
        }

    }

    public interface IConfiguration
    {
    }
}