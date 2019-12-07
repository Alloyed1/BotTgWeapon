using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication2.Models.Commands
{
    public class HelpCommand : Command
    {
        public override string Name => @"Помощь";
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            var chatId = message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, "Бот для поиска по страйкбольным барахолкам."+ Environment.NewLine +" В случае замечаний/предложений для связи с администрацией напишите: /report текст сообщения", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}