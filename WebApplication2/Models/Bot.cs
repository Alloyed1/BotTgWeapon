using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MihaZupan;
using Telegram.Bot;
using WebApplication2.Models.Commands;
using WebApplication2.Models.Commands.CallBackCommand;
using WebApplication2.Models.Commands.Caregoryes;

namespace WebApplication2.Models
{
    public class Bot
    {
        private static TelegramBotClient botClient;
        private static List<CommandMessage> commandsList;
        private static List<CommandCallBack> commandsCallBack;

        public static IReadOnlyList<CommandMessage> Commands => commandsList.AsReadOnly();
        public static IReadOnlyList<CommandCallBack> CommandsCallBack => commandsCallBack.AsReadOnly();

        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }
            

            commandsList = new List<CommandMessage>()
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
                new AllCategoryCommand(),
                new AddKidalCommand(),
                new CheckCommand()
            };
            commandsCallBack = new List<CommandCallBack>()
            {
                new ShowUserInfoCommand(),
                
                new ShowLotsCommand()
            };
            

            //var proxy = new HttpToSocks5Proxy("176.107.176.76", 38967, "P8sGSP", "qRKqX5");
            botClient = new TelegramBotClient(AppSettings.Key);
            await botClient.SetWebhookAsync(AppSettings.url);
            return botClient;
        }
    }
}