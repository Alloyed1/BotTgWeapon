using System;
using System.Collections.Generic;
using System.Linq;
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
					.Where(w => w.Text.ToLower().Contains(query.Query) ||
					            w.FirstComment.ToLower().Contains(query.Query))
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
				var keyboard4 = new ReplyKeyboardMarkup
				{
					Keyboard = new[]
					{
						new[]
						{
							new KeyboardButton("Остановить"),
							new KeyboardButton("Включить уведомления по этому запросу"),
							new KeyboardButton("Помощь"),
						},
					}
				};



				await botClient.SendTextMessageAsync(chatId, "Чтобы остановить отправку сообщений - нажмите на кнопку",
					replyMarkup: keyboard4);
				await Task.Delay(2000);
				bool isStop = false;
				var listt = list.Take(50);
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

				foreach (var lis in listt)
				{
					if (isStop)
					{
						break;
					}

					try
					{
						if (lis.Text == "")
						{
							await botClient.SendPhotoAsync(chatId, photo: lis.Src, caption: lis.FirstComment,
								replyMarkup: new InlineKeyboardMarkup(
									InlineKeyboardButton.WithUrl("Перейти",
										$"https://vk.com/photo{lis.GroupId}_{lis.PhotoId}")
								));
						}
						else
						{
							await botClient.SendPhotoAsync(chatId, photo: lis.Src, caption: lis.Text,
								replyMarkup: new InlineKeyboardMarkup(
									InlineKeyboardButton.WithUrl("Перейти",
										$"https://vk.com/photo{lis.GroupId}_{lis.PhotoId}")
								));
						}

						await Task.Delay(new Random().Next(400, 500));
						if (listt.Last() == lis)
						{
							var keyboard3 = new ReplyKeyboardMarkup
							{
								Keyboard = new[]
								{
									new[]
									{
										new KeyboardButton("Показать результат"),
										new KeyboardButton("Включить уведомления по этому запросу"),
										new KeyboardButton("Помощь"),
									},
								}
							};

							await botClient.SendTextMessageAsync(chatId, "Все результаты были показаны",
								replyMarkup: keyboard3);
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