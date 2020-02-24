using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WebApplication2.Controllers;
using File = System.IO.File;

namespace WebApplication2.Models.Commands
{
    public class ResultCountCommand : CommandMessage
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
                Console.WriteLine(chatId);
                var categoryName = "";

                categoryName = (await db.Chats.FirstOrDefaultAsync(f => f.ChatId == chatId.ToString())).CategorySearch;
                

                var reslist = new List<WeaponList>();
                
                if (queryListAnd.Count == queryListOr.Count)
                {
                    reslist = await GetQuery(false, false, new List<string>(), message.Text, categoryName);
                }
                else if (queryListAnd.Count > queryListOr.Count)
                {
                    reslist = await GetQuery(true, false, queryListAnd, message.Text, categoryName);
                }
                else
                {
                    reslist = await GetQuery(false, true, queryListOr, message.Text, categoryName);
                }



                
                
                var ggg = reslist.Select(s => s.Text).Distinct()
                    .Count();
                    

                
                var list = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
                {
                    InlineKeyboardButton.WithCallbackData("Показать результат", "Показать результат")
                });




                if (ggg == 0)
                {
                    ReplyKeyboardMarkup ReplyKeyboard = new[]
                    {
                        new[] { "Поиск по категориям", "Помощь"},
                    };
                    ReplyKeyboard.ResizeKeyboard = true;
                    await botClient.SendTextMessageAsync(chatId, $@"Количество найденных лотов: {ggg} Нажми «Показать результат» либо сделай новый запрос.", replyMarkup: ReplyKeyboard);

                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId, $@"Количество найденных лотов: {ggg} Нажми «Показать результат» либо сделай новый запрос.", replyMarkup: list);
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

        async Task<List<WeaponList>> GetQuery(bool and, bool or, List<string> list, string message, string category = null)
            {
                
                using (var db = new DbNorthwind())
                {
                    var listWeapon = await db.WeaponList.ToListAsync();
                    var returnList = new List<WeaponList>();
                    if (and)
                    {
                        if (list.Count == 2)
                        {
                            returnList = listWeapon
                                .Where(w => Regex.IsMatch(w.Text, Regex.Escape(list[0]), RegexOptions.IgnoreCase)
                                            && Regex.IsMatch(w.Text, Regex.Escape(list[1]), RegexOptions.IgnoreCase)).ToList();
                        }
                        else if (list.Count == 3)
                        {
                            returnList = listWeapon
                                .Where(w => Regex.IsMatch(w.Text, Regex.Escape(list[0]), RegexOptions.IgnoreCase)
                                            && Regex.IsMatch(w.Text, Regex.Escape(list[1]), RegexOptions.IgnoreCase)
                                            && Regex.IsMatch(w.Text, Regex.Escape(list[2]), RegexOptions.IgnoreCase)).ToList();
                        }
                        else if (list.Count == 4)
                        {
                            returnList = listWeapon
                                .Where(w => Regex.IsMatch(w.Text, Regex.Escape(list[0]), RegexOptions.IgnoreCase)
                                            && Regex.IsMatch(w.Text, Regex.Escape(list[1]), RegexOptions.IgnoreCase)
                                            && Regex.IsMatch(w.Text, Regex.Escape(list[2]), RegexOptions.IgnoreCase)
                                            && Regex.IsMatch(w.Text, Regex.Escape(list[3]), RegexOptions.IgnoreCase)).ToList();
                        }
                        else
                        {
                            returnList =  listWeapon
                                .Where(w => Regex.IsMatch(w.Text, Regex.Escape(message), RegexOptions.IgnoreCase))
                                .ToList();
                        }

                    }
                    else if (or)
                    {
                        if (list.Count == 2)
                        {
                            returnList = listWeapon
                                .Where(w => Regex.IsMatch(w.Text, Regex.Escape(list[0]), RegexOptions.IgnoreCase)
                                            || Regex.IsMatch(w.Text, Regex.Escape(list[1]), RegexOptions.IgnoreCase)).ToList();
                        }
                        else if (list.Count == 3)
                        {
                            returnList = listWeapon
                                .Where(w => Regex.IsMatch(w.Text, Regex.Escape(list[0]), RegexOptions.IgnoreCase)
                                            || Regex.IsMatch(w.Text, Regex.Escape(list[1]), RegexOptions.IgnoreCase)
                                            || Regex.IsMatch(w.Text, Regex.Escape(list[2]), RegexOptions.IgnoreCase)).ToList();
                        }
                        else if (list.Count == 4)
                        {
                            returnList = listWeapon
                                .Where(w => Regex.IsMatch(w.Text, Regex.Escape(list[0]), RegexOptions.IgnoreCase)
                                            || Regex.IsMatch(w.Text, Regex.Escape(list[1]), RegexOptions.IgnoreCase)
                                            || Regex.IsMatch(w.Text, Regex.Escape(list[2]), RegexOptions.IgnoreCase)
                                            || Regex.IsMatch(w.Text, Regex.Escape(list[3]), RegexOptions.IgnoreCase)).ToList();
                        }
                        else
                        {
                            returnList =  listWeapon
                                .Where(w => Regex.IsMatch(w.Text, Regex.Escape(message), RegexOptions.IgnoreCase))
                                .ToList();
                        }
                    }
                    else
                    {
                        returnList =  listWeapon
                            .Where(w => Regex.IsMatch(w.Text, Regex.Escape(message), RegexOptions.IgnoreCase))
                            .ToList();
                    }
                    
                        
                    if (category == "Привода")
                        return returnList.Where(w => w.Category == 1).ToList();
                    if (category == "Снаряжение и защита")
                        return returnList.Where(w => w.Category == 2).ToList();
                    if (category == "Аксессуары и запчасти")
                        return returnList.Where(w => w.Category == 3).ToList();
                    
                    return returnList;
                }
            }
    }
}