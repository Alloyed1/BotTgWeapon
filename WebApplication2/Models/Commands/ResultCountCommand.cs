using System;
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
            await AddInQueryAsync(message.Chat.Id.ToString(), message.Text);
            
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

                var queryListAnd = message.Text.ToLower().Split(" и ").ToList();
                var queryListOr = message.Text.ToLower().Split(" или ").ToList();
                
                var reslist = new List<WeaponList>();
                
                if (queryListAnd.Count == queryListOr.Count)
                {
                    reslist = await GetQuery(false, false, new List<string>(), message.Text);
                }
                else if (queryListAnd.Count > queryListOr.Count)
                {
                    reslist = await GetQuery(true, false, queryListAnd, message.Text);
                }
                else
                {
                    reslist = await GetQuery(false, true, queryListOr, message.Text);
                }



                
                
                var ggg = reslist.Select(s => s.Text).Distinct()
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


                
                await AddQuery(chatId.ToString(), message.Text, reslist);
            }
            
            

            
        }

        async Task AddInQueryAsync(string chatId, string query)
        {
            using (var db = new DbNorthwind())
            {
                await db.InsertAsync(new Querys()
                {
                    Date =  DateTime.Now,
                    Query = query,
                    ChatId = chatId
                });
            }
        }
        
        async Task AddQuery(string chatId, string query, List<WeaponList> reslist)
            {
                using(var db = new DbNorthwind())
                {
                    await db.ViewsTurns
                        .Where(w => w.ChatId == int.Parse(chatId))
                        .DeleteAsync();

                    var list = reslist
                        .OrderByDescending(w => w.StartTime)
                        .Take(100)
                        .ToList();
                    
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

        async Task<List<WeaponList>> GetQuery(bool and, bool or, List<string> list, string message)
            {
                using (var db = new DbNorthwind())
                {
                    if (and)
                    {
                        if (list.Count == 2)
                        {
                            return await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(list[0].ToLower())
                                            && w.Text.ToLower().Contains(list[1].ToLower())).ToListAsync();
                        }
                        if (list.Count == 3)
                        {
                            return await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(list[0].ToLower())
                                            && w.Text.ToLower().Contains(list[1].ToLower())
                                            && w.Text.ToLower().Contains(list[2].ToLower())).ToListAsync();
                        }
                        if (list.Count == 4)
                        {
                            return await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(list[0].ToLower())
                                            && w.Text.ToLower().Contains(list[1].ToLower())
                                            && w.Text.ToLower().Contains(list[2].ToLower())
                                            && w.Text.ToLower().Contains(list[3].ToLower())).ToListAsync();
                        }
                        else
                        {
                            return  await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(message.ToLower()))
                                .ToListAsync();
                        }

                    }
                    else if (or)
                    {
                        if (list.Count == 2)
                        {
                            return await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(list[0].ToLower())
                                            || w.Text.ToLower().Contains(list[1].ToLower())).ToListAsync();
                        }
                        if (list.Count == 3)
                        {
                            return await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(list[0].ToLower())
                                            || w.Text.ToLower().Contains(list[1].ToLower())
                                            || w.Text.ToLower().Contains(list[2].ToLower())).ToListAsync();
                        }
                        if (list.Count == 4)
                        {
                            return await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(list[0].ToLower())
                                            || w.Text.ToLower().Contains(list[1].ToLower())
                                            || w.Text.ToLower().Contains(list[2].ToLower())
                                            || w.Text.ToLower().Contains(list[3].ToLower())).ToListAsync();
                        }
                        else
                        {
                            return  await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(message.ToLower()))
                                .ToListAsync();
                        }
                    }
                    else
                    {
                        return  await db.WeaponList
                            .Where(w => w.Text.ToLower().Contains(message.ToLower()))
                            .ToListAsync();
                    }
                }
            }
    }
}