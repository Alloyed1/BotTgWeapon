using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebApplication2.Models.Commands
{
    public class NotifyComand : Command
    {
        public override string Name => "Включить уведомления по этому запросу";
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            using(var db = new DbNorthwind())
            {
                var keyboard4 = new ReplyKeyboardMarkup
                {
                    Keyboard = new[]
                    {
                        new[]
                        {
                            new KeyboardButton("Отключить автоматические уведомления"),
                            new KeyboardButton("Помощь"),
                        },
                    }
                };
                await db.LastQuery
                    .Where(w => w.ChatId == chatId.ToString())
                    .Set(s => s.IsWatching, 1)
                    .UpdateAsync();

                var query = (await db.LastQuery
                    .FirstOrDefaultAsync(s => s.ChatId == chatId.ToString())).Query;
                await botClient.SendTextMessageAsync(chatId, $"Отслеживание новых объявлений по запросу \"{query}\" успешно включено", replyMarkup: keyboard4);
            }
        }

    }
}