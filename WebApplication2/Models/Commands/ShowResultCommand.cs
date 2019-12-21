using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebApplication2.Models.Commands
{
	public class ShowResultCommand : Command
	{
		public override string Name => "Показать результат";

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

				return await db.WeaponList
					.HasUniqueKey(f => f.Text)
					.Where(w => w.Text.ToLower().Contains(query.Query) ||
					            w.FirstComment.ToLower().Contains(query.Query))
					
					.OrderByDescending(w => w.StartTime)
					.Take(100)
					.ToListAsync();
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

		public override async Task Execute(Message message, TelegramBotClient botClient)
		{
			var chatId = message.Chat.Id;
			var list = await GetLastQuery(chatId.ToString());
			if (list != null)
			{

				ReplyKeyboardMarkup keyboard4 = new[]
				{
					new[] { "Остановить"},
					new []{"Помощь"}
				};
				keyboard4.ResizeKeyboard = true;



				
				
				bool isStop = false;

				list = list.GroupBy(f => f.Text)
					.Select(g => g.First())
					.Take(50)
					.ToList();
				await botClient.SendTextMessageAsync(chatId, $"Чтобы остановить отправку сообщений - нажмите на кнопку. Вам будет показано {list.Count} последних результатов.",
					replyMarkup: keyboard4);
				
				await Task.Delay(1500);
				
				

				var viewList = new List<UserViews>();
				foreach (var lis in list)
				{
					viewList.Add(new UserViews
						{ChatId = (int) chatId, GroupId = lis.GroupId.ToString(), PhotoId = lis.PhotoId.ToString()});
				}

				await using (var db = new DbNorthwind())
				{
					db.BulkCopy(viewList);
				}
				
				foreach (var lis in list)
				{
					if (isStop)
					{
						break;
					}

					try
					{
						if (lis.Text.Length > 500)
						{
							lis.Text = lis.Text.Substring(0, 500);
						}
						
						await botClient.SendPhotoAsync(chatId, photo: lis.Src, caption: lis.Text,
								replyMarkup: new InlineKeyboardMarkup(
									InlineKeyboardButton.WithUrl("Перейти",
										$"https://vk.com/photo{lis.GroupId}_{lis.PhotoId}")
								));

						await Task.Delay(1000);
						if (list.Last() == lis)
						{

							ReplyKeyboardMarkup ReplyKeyboard = new[]
							{
								new[] { "Вкл.авто уведомление"},
								new []{"Помощь", "Показать результат"}
							};
							ReplyKeyboard.ResizeKeyboard = true;

							await botClient.SendTextMessageAsync(chatId, "Все результаты были показаны",
								replyMarkup: ReplyKeyboard);
						}
						

						if (await GetLatQueryText(chatId.ToString()) == string.Empty)
						{
							isStop = true;
						}

					}
					catch (Exception ex)
					{
						
					}

				}
			}

		}
	}
}