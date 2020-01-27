using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication2.Models.Commands
{
    public abstract class Command
    {
        public  abstract string Name { get; }
        public abstract Task Execute(Message message, TelegramBotClient client, Microsoft.Extensions.Configuration.IConfiguration configuration);
        public abstract bool Contains(Message message);
    }
}