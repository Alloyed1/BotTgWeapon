﻿using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
    public abstract class CommandMessage
    {
        public  abstract string Name { get; }
        public abstract Task Execute(Message message, TelegramBotClient client, Microsoft.Extensions.Configuration.IConfiguration configuration);
        public abstract bool Contains(Message message);
    }
}