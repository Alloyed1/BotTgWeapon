using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Hosting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Telegram.Bot.Types.ReplyMarkups;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using OpenQA.Selenium.PhantomJS;
using Telegram.Bot.Types;
using VkNet.Model.Attachments;

namespace WebApplication2.Models
{
    public class VkBoy : IHostedService
    {
	    
	    private Timer _timer2;
 
	    public Task StartAsync(CancellationToken cancellationToken)
	    {
		    _timer2 = new Timer(Update, null, 0, 7000);
		    return Task.CompletedTask;
	    }

	    async void Update(object obj)
	    {
		    using (var client = new WebClient())
		    {
			    string gg = await client.DownloadStringTaskAsync("https://bottg.website/get");
		    }
	    }
	    

	    
	    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
	    

        
	    

       public async static Task CheckNewWeapon()
        {
	        try
            {
                using (var db = new DbNorthwind())
                {

                    await Task.Delay(500);
                    List<LastQuery> checkList = await db.LastQuery
                                                    .Where(w => w.IsWatching == 1)
                                                    .ToListAsync();
                    
                    foreach (var item in checkList)
                    {

                        var listWeapon = await db.WeaponList
                                .Where(w => w.Text.ToLower().Contains(item.Query.ToLower()) || w.FirstComment.ToLower().Contains(item.Query.ToLower())).ToListAsync();
                        var listViews = await db.UserViews.Where(w => w.ChatId == int.Parse(item.ChatId)).ToListAsync();
                        
                        var newWeaponList = new List<WeaponList>();
						var listView = new List<UserViews>();
						
                        foreach (var weapon in listWeapon)
                        {
                            if (listViews.FirstOrDefault(s => s.GroupId == weapon.GroupId.ToString() && s.PhotoId == weapon.PhotoId.ToString()) == null)
                            {
                                newWeaponList.Add(weapon);
								listView.Add(new UserViews { ChatId = Convert.ToInt32(item.ChatId), PhotoId = weapon.PhotoId.ToString(), GroupId = weapon.GroupId.ToString() });
                            }
                        }
						db.BulkCopy(listView);
						
						var botClient = await Bot.GetBotClientAsync();
                        foreach (var items in newWeaponList
	                        .Where(w => w.StartTime < DateTime.Now && w.StartTime.AddDays(2) > DateTime.Now)
	                        .OrderByDescending(w => w.StartTime))
                        {
							if(items.Text == "")
							{
								await botClient.SendPhotoAsync(item.ChatId, photo: items.Src, caption: items.FirstComment, replyMarkup: new InlineKeyboardMarkup(
																		InlineKeyboardButton.WithUrl("Перейти", $"https://vk.com/photo{items.GroupId}_{items.PhotoId}")
															  ));
							}
							else
							{
								await botClient.SendPhotoAsync(item.ChatId, photo: items.Src, caption: items.Text, replyMarkup: new InlineKeyboardMarkup(
																		InlineKeyboardButton.WithUrl("Перейти", $"https://vk.com/photo{items.GroupId}_{items.PhotoId}")
															  ));
							}
                            
                        }
                    }
                }
            }
            catch(Exception e){

            }
        }
    }
}