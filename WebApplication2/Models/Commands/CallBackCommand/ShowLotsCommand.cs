﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebApplication2.Models.Commands.CallBackCommand
{
    public class ShowLotsCommand : CommandCallBack
    {
        public override string Name => @"Показать лоты";
        
        public override bool Contains(CallbackQuery message)
        {
            if (message.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Data.Contains(this.Name);
        }

        public override async Task Execute(CallbackQuery message, TelegramBotClient botClient,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var chatId = message.Message.Chat.Id;
            int.TryParse(message.Data.Replace("Показать лоты", ""), out int userVkId);
            
            using var db = new DbNorthwind();
            await db.ViewsTurns.Where(w => w.ChatId == chatId).DeleteAsync();

            var listWeapon = await db.WeaponList.Where(w => w.UserId == userVkId).ToListAsync();
            var viewsTurns = new List<ViewsTurns>();
            
            listWeapon.ForEach(x => viewsTurns.Add(new ViewsTurns()
            {
                WeaponListId = x.Id,
                ChatId = (int)chatId,
            }));
            
            ReplyKeyboardMarkup keyboard4 = new[]
            {
					
                new []{"Показать результат", "Поиск по категориям"},
                new[]{"Помощь", "Вкл.авто уведомление",}
            };
            keyboard4.ResizeKeyboard = true;
            
            await botClient.SendTextMessageAsync(chatId, "Нажмите 'Показать результат', чтобы увидеть лоты пользователя", replyMarkup:keyboard4);

            db.BulkCopy(viewsTurns);
            
            
        }
    }
}