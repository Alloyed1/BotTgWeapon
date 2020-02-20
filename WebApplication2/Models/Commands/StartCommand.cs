using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types.ReplyMarkups;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => @"/start";

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
                if (await db.Chats.FirstOrDefaultAsync(w => w.ChatId == chatId.ToString()) == null)
                {
                    await AddUser(new Models.Chat { ChatId = chatId.ToString(), FirstName = message.From.FirstName, LastName = message.From.LastName, UserName = message.From.Username });
                }
            }
            ReplyKeyboardMarkup ReplyKeyboard = new[]
            {
                new []{"Помощь", "Поиск по категориям"}
            };
            ReplyKeyboard.ResizeKeyboard = true;
            await botClient.SendTextMessageAsync(chatId, "Приветствую! Введите слово для поиска или нажмите \"Помощь\"", replyMarkup: ReplyKeyboard);
        }
        public async Task AddUser(Models.Chat chat)
        {

            using(var db = new DbNorthwind())
            {
                await db.InsertWithInt32IdentityAsync(chat);
            }
        }
    }
}