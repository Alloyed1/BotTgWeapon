using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
	public class ShowResultCommand : CommandMessage
	{
		public override string Name => "Показать результат";
		public Settings settings { get; set; }
		

		public override bool Contains(Message message)
		{
			if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}

		async Task<List<WeaponList>> GetLastQuery(string chatId)
		{
			using (var db = new DbNorthwind())
			{
				var query = await db.LastQuery.FirstOrDefaultAsync(w => w.ChatId == chatId);
				if (query == null)
				{
					return null;
				}

				var turnsList = (await db.ViewsTurns
						.Where(w => w.ChatId == int.Parse(chatId))
						.ToListAsync())
					.Take(settings.CountMessage)
					.ToList();
				
				
				var list = await db.WeaponList
					.Where(w => turnsList.Select(s => s.WeaponListId).Contains(w.Id))
					.ToListAsync();

				await DeleteItems(chatId);
				return list;
				


			}
		}

		public async Task DeleteItems(string chatId)
		{
			using (var db = new DbNorthwind())
			{
				var turnsList = (await db.ViewsTurns
						.Where(w => w.ChatId == int.Parse(chatId))
						.ToListAsync())
					.Take(settings.CountMessage)
					.ToList();
				
				foreach (var item in turnsList)
				{

					await db.ViewsTurns
						.Where(w => w.ChatId == int.Parse(chatId) && w.WeaponListId == item.WeaponListId)
						.DeleteAsync();
				}
			}
				
				
		}

		async Task<string> GetLatQueryText(string chatId)
		{
			using (var db = new DbNorthwind())
			{
				return (await db.LastQuery
					.FirstOrDefaultAsync(s => s.ChatId == chatId)).Query;
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
					InlineKeyboardButton.WithCallbackData("Проверить", "Проверить" + vkId)

				});

				text += Environment.NewLine + Environment.NewLine + "❗❗❗ Найден в списках мошенников ❗❗❗";
				
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
					InlineKeyboardButton.WithCallbackData("Проверить", "Проверить" + vkId)
					
				});
				
				text += Environment.NewLine + Environment.NewLine + "✅ В списках мошенников не найден";
				
				await botClient.SendPhotoAsync(chatId, photo: scr, caption: text,
					replyMarkup: list
					);
			}
			
		}

		public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			settings = configuration.GetSection("Settings").Get<Settings>();
			
			var chatId = message.Chat.Id;
			var list = await GetLastQuery(chatId.ToString());
			
			var count = 0;
			using (var db = new DbNorthwind())
			{
				count = await db.ViewsTurns
					.Where(w => w.ChatId == chatId)
					.CountAsync();
			}
			

			var countShow = settings.CountMessage;
			if (count >= countShow) countShow = settings.CountMessage;
			else countShow = count;
			
			
			if (!list.Any())
			{
				await botClient.SendTextMessageAsync(chatId, "Все результаты были показаны");
			}
			else
			{
				
				
				ReplyKeyboardMarkup keyboard4 = new[]
				{
					
					new []{"Помощь"},
					new[]{"Поиск по категориям"}
				};
				keyboard4.ResizeKeyboard = true;

				
				bool isStop = false;


				var viewList = new List<UserViews>();
				foreach (var lis in list)
				{
					viewList.Add(new UserViews
						{ChatId = (int) chatId, GroupId = lis.GroupId.ToString(), PhotoId = lis.PhotoId.ToString()});
				}
				
				using (var db = new DbNorthwind())
				{
					db.BulkCopy(viewList);
				}
				 
				foreach (var lis in list)
				{

					if (lis.Text.Length > 430)
					{
						lis.Text = lis.Text.Substring(0, 430);
					}

					lis.Text += Environment.NewLine + Environment.NewLine + $"📅 Дата публикации: {lis.StartTime:dd'/'MM'/'yyyy HH:mm:ss}";

					await using var db = new DbNorthwind();
					var kidal = await db.Kidals.FirstOrDefaultAsync(f => f.VkId == lis.UserId);

					await SendPhoto(lis.Text, $"https://vk.com/photo{lis.GroupId}_{lis.PhotoId}", lis.Src,lis.UserId, (int)chatId, botClient, kidal);


					if (list.Last() == lis)
					{
						ReplyKeyboardMarkup ReplyKeyboard = new[]
						{
							new[] { $"Показать результат ещё {countShow} (Осталось {count})", "Поиск по категориям"},
							new []{"Помощь", "Вкл.авто уведомление"},
						};
						ReplyKeyboard.ResizeKeyboard = true;

						if (count != 0) {
							var listMarkup = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
							{
								InlineKeyboardButton.WithCallbackData("Показать результат", "Показать результат")
							});
							await botClient.SendTextMessageAsync(chatId, $"Нажмите на кнопку в меню, чтобы показать ещё {countShow}",
								replyMarkup: ReplyKeyboard);
						}


						else
						{
							ReplyKeyboard = new[]
							{
								new []{"Вкл.авто уведомление", "Поиск по категориям"},
								new[]{"Помощь"}
							};
							ReplyKeyboard.ResizeKeyboard = true;
							await botClient.SendTextMessageAsync(chatId, "Все результаты были показаны, сделайте повторный запрос или включите уведомления по этому.",
								replyMarkup: ReplyKeyboard);
						}
							
					}
					Thread.Sleep(settings.Delay);

				}
			}

		}
	}
}