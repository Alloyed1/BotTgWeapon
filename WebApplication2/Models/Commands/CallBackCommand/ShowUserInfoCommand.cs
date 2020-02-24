using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebApplication2.Models.Commands.CallBackCommand
{
	public class ShowUserInfoCommand : CommandCallBack
	{
		public override string Name => @"Проверить";
        
		public override bool Contains(CallbackQuery message)
		{
			if (message.Message.Type != Telegram.Bot.Types.Enums.MessageType.Photo)
				return false;

			return message.Data.Contains(this.Name);
		}
		public override async Task Execute(CallbackQuery message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			var chatId = message.Message.Chat.Id;
			var settings = configuration.GetSection("Settings").Get<Settings>();
			
			using var db = new DbNorthwind();

			int.TryParse(message.Data.Replace("Проверить", ""), out int userVkId);

			var listWeapon = await db.WeaponList.Where(w => w.UserId == userVkId).ToListAsync();
			
			if (listWeapon.Count > 1)
			{
				var kidal = await db.Kidals.FirstOrDefaultAsync(f => f.VkId == listWeapon[0].UserId);
				var list = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
				{
					InlineKeyboardButton.WithCallbackData("Показать лоты", "Показать лоты" + listWeapon[0].UserId)
				});
				if (kidal == null)
				{
					await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
					                                             + Environment.NewLine + $"Профиль: vk.com/id{listWeapon[0].UserId}" 
					                                             + Environment.NewLine + "В списках ненадежных продавцов не найден"
					                                             + Environment.NewLine + Environment.NewLine + $"Чтобы посмотреть все лоты данного человека нажмите на кнопку ({listWeapon.Count} лотов)", replyMarkup:list);
				}
				else
				{
					
					await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
					                                             + Environment.NewLine + $"Профиль: vk.com/id{listWeapon[0].UserId}" 
					                                             + Environment.NewLine + $"❗Найден в списке мошенников: https://vk.com/topic-{kidal.GroupId}_{kidal.TopicId}?post={kidal.PostId}"
					                                             + Environment.NewLine + Environment.NewLine + $"Чтобы посмотреть все лоты данного человека нажмите на кнопку ({listWeapon.Count} лотов)", replyMarkup:list);
				}
				
			}
			else
			{
				var kidal = await db.Kidals.FirstOrDefaultAsync(f => f.VkId == listWeapon[0].UserId);
				if (kidal == null)
				{
					await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
					                                             + Environment.NewLine + $"Профиль: vk.com/id{listWeapon[0].UserId}" 
					                                             + Environment.NewLine + "В списках ненадежных продавцов не найден"
					                                             + Environment.NewLine + Environment.NewLine + "❗Больше лотов у данного продавца не найдено");
				}
				else
				{
					
					await botClient.SendTextMessageAsync(chatId, "Информация по аккаунту: " 
					                                             + Environment.NewLine + $"Профиль: vk.com/id{listWeapon[0].UserId}" 
					                                             + Environment.NewLine + $"❗Найден в списке мошенников: https://vk.com/topic-{kidal.GroupId}_{kidal.TopicId}?post={kidal.PostId}"
					                                             + Environment.NewLine + Environment.NewLine + "❗Больше лотов у данного продавца не найдено");
				}
			}
			
			




		}
		async Task SendPhoto(string text, string url,string scr , int vkId , int chatId, TelegramBotClient botClient, Kidals kidals = null)
		{
			if (kidals != null)
			{
				var list = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
				{
					new InlineKeyboardButton()
					{
						Text = "Перейти",
						Url = url,
					},
					new InlineKeyboardButton()
					{
						Text = "❗Мошенник.Инфо.",
						Url = $"https://vk.com/topic-{kidals.GroupId}_{kidals.TopicId}?post={kidals.PostId}",
					},

				});
				await botClient.SendPhotoAsync(chatId, photo: scr, caption: text,
					replyMarkup: list);
			}
			else
			{
				var list = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
				{
					new InlineKeyboardButton()
					{
						Text = "Перейти",
						Url = url,
					},

				});
				
				await botClient.SendPhotoAsync(chatId, photo: scr, caption: text,
					replyMarkup: list
				);
			}
			
		}
	}
}
