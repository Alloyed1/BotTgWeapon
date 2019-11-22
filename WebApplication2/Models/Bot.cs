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
                new StopCommand()
            };
            
            //TODO: Add more commands

            var proxy = new HttpToSocks5Proxy("168.235.93.240", 32659, "Ve75UG", "JtjpX6");
            botClient = new TelegramBotClient(AppSettings.Key, proxy);
            await botClient.SetWebhookAsync(AppSettings.url);
            return botClient;
        }
    }
}