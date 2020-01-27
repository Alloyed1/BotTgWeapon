﻿using System.Threading.Tasks;
using LinqToDB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebApplication2.Models.Commands
{
    public class StopNotifyCommand : Command
    {
        public override string Name => @"Отключить автоматические уведомления";
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
                            new []{"Помощь"}
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