using System;
using System.Threading.Tasks;
using LinqToDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebApplication2.Controllers;

namespace WebApplication2.Models.Commands
{
    public class HelpCommand : CommandMessage
    {
        public override string Name => @"Помощь";
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var chatId = message.Chat.Id;
            await using var db = new DbNorthwind();

            var countLots = await db.WeaponList.CountAsync();
            var countKidals = await db.Kidals.CountAsync();
            
            await botClient.SendTextMessageAsync(chatId, "Бот для поиска по страйкбольным барахолкам."
                                                         + Environment.NewLine +"Введи слово и бот найдет. Для сложного поиска можно использовать «и» или «или». Например: АК и Тюмень." 
                                                         + Environment.NewLine + Environment.NewLine +" В случае замечаний/предложений для связи с администрацией напишите: /report текст сообщения"
                                                         + Environment.NewLine + " Чтобы проверить человека в списках мошенников: /check ссылка на вк"
                                                         + Environment.NewLine + Environment.NewLine + $"✅ Количество лотов на данный момент : {countLots}"
                                                         + Environment.NewLine +  $"❗ Количество мошенников в базе : {countKidals}", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        }
    }
}