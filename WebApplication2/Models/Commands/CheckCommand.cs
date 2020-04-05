using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebApplication2.Models.Commands
{
    public class CheckCommand : CommandMessage
    {
        public override string Name => @"/check";

        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
	        var chatId = message.Chat.Id;
            var link = "";
            try
            {
                link = message.Text.Split(' ')[1];
            } catch{return;}
            
            await using var db = new DbNorthwind();
            var kidal = new Kidals();

	        kidal = await db.Kidals.FirstOrDefaultAsync(f => f.VkLink == link || f.VkLink == link.Replace("https://", ""));

			var listWeapon = new List<WeaponList>();
	        if (kidal is null)
	        {
		        int.TryParse(link.Replace("vk.com/id", ""), out int idFirst);
		        int.TryParse(link.Replace("https://vk.com/id", ""), out int idSecond);
		        
		        listWeapon = await db.WeaponList.Where(w => w.UserId == idFirst || w.UserId == idSecond).ToListAsync();
	        }
	        else
		        listWeapon = await db.WeaponList.Where(w => w.UserId == kidal.VkId).ToListAsync();





	        if (listWeapon.Count > 1)
			{
				var list = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
				{
					InlineKeyboardButton.WithCallbackData("Показать лоты", "Показать лоты" + listWeapon[0].UserId)
				});
				if (kidal is null)
				{
					await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
					                                             + Environment.NewLine + $"Профиль: vk.com/id{listWeapon[0].UserId}" 
					                                             + Environment.NewLine + "✅ В списках ненадежных продавцов не найден"
					                                             + Environment.NewLine + Environment.NewLine + $"Чтобы посмотреть все лоты данного человека нажмите на кнопку ({listWeapon.Count} лотов)", replyMarkup:list);
				}
				else
				{
					
					await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
					                                             + Environment.NewLine + $"Профиль: vk.com/id{listWeapon[0].UserId}" 
					                                             + Environment.NewLine + $"❗ Найден в списке мошенников: https://vk.com/topic-{kidal.GroupId}_{kidal.TopicId}?post={kidal.PostId}"
					                                             + Environment.NewLine + Environment.NewLine + $"Чтобы посмотреть все лоты данного человека нажмите на кнопку ({listWeapon.Count} лотов)", replyMarkup:list);
				}
				
			}
			else
			{
				if (!listWeapon.Any())
				{
					if (kidal is null)
					{
						await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
						                                             + Environment.NewLine + $"Профиль: {link}" 
						                                             + Environment.NewLine + "✅В списках ненадежных продавцов не найден"
						                                             + Environment.NewLine + Environment.NewLine + " Лотов у данного продавца не найдено");
					}
					else
					{
						await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
						                                             + Environment.NewLine + $"Профиль: {link}" 
						                                             + Environment.NewLine + $"❗ Найден в списке мошенников: https://vk.com/topic-{kidal.GroupId}_{kidal.TopicId}?post={kidal.PostId}"
						                                             + Environment.NewLine + Environment.NewLine + $"Лотов у данного продавца не найдено");
					}
					
				}
				
				else if (kidal is null)
				{
					await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
					                                             + Environment.NewLine + $"Профиль: vk.com/id{listWeapon[0].UserId}" 
					                                             + Environment.NewLine + "✅ В списках ненадежных продавцов не найден"
					                                             + Environment.NewLine + Environment.NewLine + "Больше лотов у данного продавца не найдено");
				}
				else
				{
					
					await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
					                                             + Environment.NewLine + $"Профиль: vk.com/id{listWeapon[0].UserId}" 
					                                             + Environment.NewLine + $"❗ Найден в списке мошенников: https://vk.com/topic-{kidal.GroupId}_{kidal.TopicId}?post={kidal.PostId}"
					                                             + Environment.NewLine + Environment.NewLine + "Больше лотов у данного продавца не найдено");
				}
			}
        }

    }
}