using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace WebApplication2.Models.Commands
{
    public class ResultCountCommand : Command
    {
        
        
        public override string Name => @"/show";
        
        public override bool Contains(Message message)
        {
            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return false;

            return message.Text.Contains(this.Name);
        }
        public override async Task Execute(Message message, TelegramBotClient botClient)
        {
            using (var db = new DbNorthwind())
            {
                var chatId = message.Chat.Id;
                var query = await db.LastQuery
                    .FirstOrDefaultAsync(w => w.ChatId == chatId.ToString());
                if (query != null)
                {
                    if (query.IsWatching == 1)
                    {
                        query.IsWatching = 0;
                        await db.UpdateAsync(query);
                        await botClient.SendTextMessageAsync(chatId, $"Уведомления по старому запросу \"{query.Query}\" были отключены");
                    }
                }



                var ggg = await db.WeaponList
                    .Where(w => w.Text.ToLower().Contains(message.Text.ToLower()) || w.FirstComment.ToLower().Contains(message.Text.ToLower()))
                    .ToListAsync();

                

                ReplyKeyboardMarkup ReplyKeyboard = new[]
                {
                    new[] { "Показать результат", "Помощь"},
                };
                ReplyKeyboard.ResizeKeyboard = true;



                if (ggg.Count == 0)
                {
                    await botClient.SendTextMessageAsync(chatId, $@"Количество найденных лотов: {ggg
                        .GroupBy(p => new { p.Text })
                        .Select(g => g.First())
                        .ToList()
                        .Count.ToString()} Нажми «Показать результат» либо сделай новый запрос.");

                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, $@"Количество найденных лотов: {ggg
                        .GroupBy(p => new { p.Text })
                        .Select(g => g.First())
                        .ToList()
                        .Count.ToString()} Нажми «Показать результат» либо сделай новый запрос.", replyMarkup: ReplyKeyboard);
                }



                await AddQuery(chatId.ToString(), message.Text);
            }
            
            async Task AddQuery(string chatId, string query)
            {
                using(var db = new DbNorthwind())
                {
                    var queryClass = await db.LastQuery.FirstOrDefaultAsync(w => w.ChatId == chatId);
                    if(queryClass == null)
                    {
                        await db.InsertAsync(new LastQuery { ChatId = chatId, Query = query });
                    }
                    else
                    {
                        await db.LastQuery
                            .Where(w => w.Id == queryClass.Id)
                            .Set(s => s.Query, query)
                            .UpdateAsync();		
                    }
                }
            }
        }
    }
}