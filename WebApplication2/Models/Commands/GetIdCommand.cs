using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
	public class GetIdCommand : Command
	{
		public override string Name => "/getid";
		public override bool Contains(Message message)
		{
			if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}
		public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			var chatId = message.Chat.Id;
			await botClient.SendTextMessageAsync(chatId, "Ваш id: " + chatId, replyMarkup: null);
		}

	}
}
