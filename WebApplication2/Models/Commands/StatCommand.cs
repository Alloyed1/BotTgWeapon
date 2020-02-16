using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication2.Models.Commands
{
	public class StatCommand : Command
	{
		public override string Name => @"/stat";
		public override bool Contains(Message message)
		{
			if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}
		public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			var chatId = (int)message.Chat.Id;
			var adminId = configuration.GetSection("Settings").Get<Settings>().AdminChatId;

			if (chatId != adminId)
			{
				return;
			}

			var text = "";
			using (var db = new DbNorthwind())
			{
				int countInDay = 0, countInWeek = 0, countInMoth = 0, uniqueUsersInDay = 0, uniqueUsersInWeek = 0, uniqueUsersInMoth = 0;
				var list = await db.Querys.Where(w => w.Date > DateTime.Now.AddDays(-30)).ToListAsync();

				countInDay = list.Where(w => w.Date.Day == DateTime.Now.Day).ToList().Count;
				countInWeek= list.Where(w => w.Date > DateTime.Now.AddDays(-7)).ToList().Count;
				countInMoth = list.Where(w => w.Date > DateTime.Now.AddDays(-30)).ToList().Count;
				
				uniqueUsersInDay = list.Where(w => w.Date.Day == DateTime.Now.Day).Select(s => s.ChatId).Distinct().ToList().Count;
				uniqueUsersInWeek= list.Where(w => w.Date > DateTime.Now.AddDays(-7)).Select(s => s.ChatId).Distinct().ToList().Count;
				uniqueUsersInMoth = list.Where(w => w.Date > DateTime.Now.AddDays(-30)).Select(s => s.ChatId).Distinct().ToList().Count;

				var countIsWatching = await db.LastQuery.Where(f => f.IsWatching == 1).CountAsync();

				text += $"Количество запросов: {countInDay}/{countInWeek}/{countInMoth}";
				text += Environment.NewLine + Environment.NewLine;
				
				text += $"Уникальных пользователей: {uniqueUsersInDay}/{uniqueUsersInWeek}/{uniqueUsersInMoth}"ж
				text += Environment.NewLine + Environment.NewLine;
				
				text += $"Подписаны на уведомление: {countIsWatching}";

				await botClient.SendTextMessageAsync(adminId, text);

			}
		}

		Task GetQueryInDay(ref int count)
		{
			using (var db = new DbNorthwind())
			{
				count = db.Querys.Where(w => w.Date.Day == DateTime.Now.Day).ToList().Count;
				return Task.CompletedTask;
			}

			
		}
		Task GetQueryInWeek(ref int count)
		{
			using (var db = new DbNorthwind())
			{
				count = db.Querys.Where(w => w.Date > DateTime.Now.AddDays(-7)).ToList().Count;
				return Task.CompletedTask;
			}
			
		}
		Task GetQueryInMoth(ref int count)
		{
			using (var db = new DbNorthwind())
			{
				count = db.Querys.Where(w => w.Date > DateTime.Now.AddDays(-30)).ToList().Count;
				return Task.CompletedTask;
			}
		}

		Task GetUsersInDay(ref int count)
		{
			using (var db = new DbNorthwind())
			{
				count = db.Querys
					.Where(w => w.Date.Day == DateTime.Now.Day)
					.Select(s => s.ChatId)
					.Distinct()
					.ToList().Count;
				
				return Task.CompletedTask;
			}
		}
		Task GetUsersInWeek(ref int count)
		{
			using (var db = new DbNorthwind())
			{
				count = db.Querys
					.Where(w => w.Date > DateTime.Now.AddDays(-7))
					.Select(s => s.ChatId)
					.Distinct()
					.ToList().Count;
				
				return Task.CompletedTask;
			}
		}
		Task GetUsersInMoth(ref int count)
		{
			using (var db = new DbNorthwind())
			{
				count = db.Querys
					.Where(w => w.Date > DateTime.Now.AddDays(-30))
					.Select(s => s.ChatId)
					.Distinct()
					.ToList().Count;
				
				return Task.CompletedTask;
			}
		}
	}
}
