﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
    public class CategoriesShowCommand: CommandMessage
    {
        public override string Name => @"Поиск по категориям";
        
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var chatId = message.Chat.Id;
            //var categories = configuration.GetSection("Settings").Get<Settings>().Categories;
            
            
            ReplyKeyboardMarkup ReplyKeyboard = new[]
            {
                new []{"Привода", "Снаряжение и защита", "Аксессуары и запчасти"},
                new []{"Все категории", "Помощь", "Вкл.авто уведомление"}
            };
            ReplyKeyboard.ResizeKeyboard = true;

            await botClient.SendTextMessageAsync(chatId, "Выберите категорию", replyMarkup:ReplyKeyboard);


        }
    }
}