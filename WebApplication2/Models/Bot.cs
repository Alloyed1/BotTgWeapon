using System.Collections.Generic;
using System.Threading.Tasks;
using MihaZupan;
using Telegram.Bot;
using WebApplication2.Models.Commands;

namespace WebApplication2.Models
{
    public class Bot
    {
        private static TelegramBotClient botClient;
        private static List<Command> commandsList;
        
        public static IReadOnlyList<Command> Commands => commandsList.AsReadOnly();
        
        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }

            commandsList = new List<Command>()
            {
                new ResultCountCommand(),
                new CountCommand(),
                new StartCommand(),
                new ShowResultCommand(),
                new HelpCommand(),
                new NotifyComand(),
                new StopCommand(),
                new StopNotifyCommand()
            };
            

            var proxy = new HttpToSocks5Proxy("176.107.182.181", 44516, "Qye2Ck", "Z1G7oE");
            botClient = new TelegramBotClient(AppSettings.Key, proxy);
            await botClient.SetWebhookAsync(AppSettings.url);
            return botClient;
        }
    }
}