using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication2.Models.Commands
{
    public abstract class CommandCallBack
    {
        public abstract string Name { get; }
        public abstract Task Execute(CallbackQuery message, TelegramBotClient client, Microsoft.Extensions.Configuration.IConfiguration configuration);
        public abstract bool Contains(CallbackQuery message);
    }
}