﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication2.Controllers;

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
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var chatId = message.Chat.Id;
            await botClient.SendTextMessageAsync(chatId, "Бот для поиска по страйкбольным барахолкам."+ Environment.NewLine +" В случае замечаний/предложений для связи с администрацией напишите: /report текст сообщения", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}