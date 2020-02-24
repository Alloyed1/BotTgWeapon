using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands.Caregoryes
{
    public class AllCategoryCommand : CommandMessage
    {
        public override string Name => @"Все категории";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            await using var db = new DbNorthwind();
            Console.WriteLine(message.Chat.Id);
            await db.Chats.Where(w => w.ChatId == message.Chat.Id.ToString())
                .Set(s => s.CategorySearch, "")
                .UpdateAsync();
            await botClient.SendTextMessageAsync(message.Chat.Id, "Все категории включены");

        }
    }
}
