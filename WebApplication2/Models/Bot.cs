using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MihaZupan;
using Telegram.Bot;
using WebApplication2.Models.Commands;
using WebApplication2.Models.Commands.Caregoryes;

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
                new StopNotifyCommand(),
                new ReportCommand(),
                new ReportAdminCommand(),
                new GetIdCommand(),
                new StatCommand(),
                new AllUsersNotifyCommand(),
                new CategoriesShowCommand(),
                new OtherCommand(),
                new PrivodaCommand(),
                new SnarygAndArmorCommand(),
                new AllCategoryCommand()
            };
            

            //var proxy = new HttpToSocks5Proxy("176.53.172.126", 37219, "beJTqlYfJK", "vB8qnCswIi");
            botClient = new TelegramBotClient(AppSettings.Key);
            await botClient.SetWebhookAsync(AppSettings.url);
            return botClient;
        }
    }
}