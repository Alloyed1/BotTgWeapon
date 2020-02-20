using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
	public class AllUsersNotifyCommand : Command
	{
		public override string Name => "/notify";
		public override bool Contains(Message message)
		{
			if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}
		public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			var settings = configuration.GetSection("Settings").Get<Settings>();
			if (message.Chat.Id != settings.AdminChatId)
			{
				return;
			}

			await using (var db = new DbNorthwind())
			{
				var users = await db.LastQuery.Select(s => s.ChatId).ToListAsync();
				users.ForEach(f =>
				{
					botClient.SendTextMessageAsync(long.Parse(f), message.Text.Replace(@"/notify ", ""));
				});
			}

		}
	}
}
