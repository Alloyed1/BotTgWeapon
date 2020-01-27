using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
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
        public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
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



                var gggList = await db.WeaponList
                    .Where(w => w.Text.ToLower().Contains(message.Text.ToLower()) ||
                                w.FirstComment.ToLower().Contains(message.Text.ToLower()))
                    .ToListAsync();
                
                var ggg = gggList.GroupBy(f => f.Text)
                    .Select(g => g.First())
                    .Count();
                    

                

                ReplyKeyboardMarkup ReplyKeyboard = new[]
                {
                    new[] { "Показать результат", "Помощь"},
                };
                ReplyKeyboard.ResizeKeyboard = true;



                if (ggg == 0)
                {
                    await botClient.SendTextMessageAsync(chatId, $@"Количество найденных лотов: {ggg} Нажми «Показать результат» либо сделай новый запрос.");

                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, $@"Количество найденных лотов: {ggg} Нажми «Показать результат» либо сделай новый запрос.", replyMarkup: ReplyKeyboard);
                }


                
                await AddQuery(chatId.ToString(), message.Text);
            }
            
            async Task AddQuery(string chatId, string query)
            {
                using(var db = new DbNorthwind())
                {
                    await db.ViewsTurns
                        .Where(w => w.ChatId == int.Parse(chatId))
                        .DeleteAsync();
                    
                    var list = await db.WeaponList
                        .HasUniqueKey(f => f.Text)
                        .Where(w => w.Text.ToLower().Contains(query) ||
                                    w.FirstComment.ToLower().Contains(query))
					
                        .OrderByDescending(w => w.StartTime)
                        .Take(100)
                        .ToListAsync();

                    list = list.GroupBy(f => f.Text)
                        .Select(g => g.First()).Take(50).ToList();

                    var turnsList = new List<ViewsTurns>();

                    foreach (var item in list)
                    {
                        turnsList.Add(new ViewsTurns()
                        {
                            ChatId = int.Parse(chatId),
                            WeaponListId = item.Id,
                        });
                    }

                    db.ViewsTurns.BulkCopy(turnsList);
                    
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