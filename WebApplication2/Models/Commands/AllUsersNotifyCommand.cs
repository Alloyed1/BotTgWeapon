using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

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
			
		}
	}
}
