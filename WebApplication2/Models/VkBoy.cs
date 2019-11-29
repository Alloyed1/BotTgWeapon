using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Telegram.Bot.Types.ReplyMarkups;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using OpenQA.Selenium.PhantomJS;

namespace WebApplication2.Models
{
    public static class VkBoy
    {
        public async static Task GetWeaponList()
        {
            VkApi _api = new VkApi();
            var userList = new List<string>
                {

                    "2329557afb5eaa5f8280b747b1ca43320eee63e7098c0ed8fcf802d94ea3692ca8bfc0416547cb7b7e15c",
                    "74d89552338d10e3a6ddec113d6c5a481542afe13f176a5514303459ed9625ab47a4f68beff9499222b11",
					"bb15ee18ad62811a5dbe158b26f7dd7edf30fbf0c42d5d8ecd42c49778b94e3b2e49009f569f4cca1c1b0"
				};

                _api.Authorize(new ApiAuthParams
                {
                    AccessToken = "2329557afb5eaa5f8280b747b1ca43320eee63e7098c0ed8fcf802d94ea3692ca8bfc0416547cb7b7e15c"
                });

	        using(var db = new DbNorthwind())
			{

				await db.WeaponList.Where(w => w.FirstComment == "").DeleteAsync();

				List<GroupAlbum> list = new List<GroupAlbum>
				{
					new GroupAlbum {GroupId = -13212026, AlbumId = 269329297},
					new GroupAlbum {GroupId = -13212026, AlbumId = 269329282},
					new GroupAlbum {GroupId = -13212026, AlbumId = 269329241},
					new GroupAlbum {GroupId = -76629546, AlbumId = 203426992},
					new GroupAlbum {GroupId = -76629546, AlbumId = 203426857},
					new GroupAlbum {GroupId = -11571122, AlbumId = 229924509},
					new GroupAlbum {GroupId = -11571122, AlbumId = 218215712},
					new GroupAlbum {GroupId = -11571122, AlbumId = 229924703},
					new GroupAlbum {GroupId = -42520747, AlbumId = 238108558},
					new GroupAlbum {GroupId = -42520747, AlbumId = 255052787},
					new GroupAlbum {GroupId = -42520747, AlbumId = 265095549},
					new GroupAlbum {GroupId = -42520747, AlbumId = 265095678},
					new GroupAlbum {GroupId = -42520747, AlbumId = 265095819},
					new GroupAlbum {GroupId = -42520747, AlbumId = 265095843},
					new GroupAlbum {GroupId = -42520747, AlbumId = 265095861},
					new GroupAlbum {GroupId = -42520747, AlbumId = 265095887},
					new GroupAlbum {GroupId = -82590437, AlbumId = 265820397},
					new GroupAlbum {GroupId = -82590437, AlbumId = 265820440},
					new GroupAlbum {GroupId = -82590437, AlbumId = 265820351},



				};

				List<WeaponList> weaponLists = new List<WeaponList>();
				foreach (var item in list)
				{

					try
					{
						var query = _api.Photo.Get(new PhotoGetParams
						{
							OwnerId = item.GroupId,
							AlbumId = PhotoAlbumType.Id(item.AlbumId),
							Reversed = true,
							Extended = true,
							Count = 1000
						});

						foreach(var photo in query.Where(w => w.Text != ""))

                        {
							weaponLists.Add(new WeaponList { Text = photo.Text, PhotoId = (long)photo.Id, AlbumId = item.AlbumId, GroupId = item.GroupId, Src = photo.Sizes.OrderByDescending(w => w.Height).First().Src.ToString(), FirstComment = "" });
						}
						await Task.Delay(1000);
					}
					catch(Exception e)
					{
						//await botClient.SendTextMessageAsync(chatId, $"{item.GroupId} {item.AlbumId} ");
					}
			
				}
                await db.WeaponList.Where(w => w.FirstComment == "").DeleteAsync();
                db.BulkCopy(weaponLists);
                await _api.LogOutAsync();

            }
        }

        public async static Task GetWeaponListComments()
        {
	        try
            {
                VkApi _api = new VkApi();
                using (var db = new DbNorthwind())
                {

                    await db.WeaponList.Where(w => w.Text == "").DeleteAsync();

                    List<GroupAlbum> list = new List<GroupAlbum>
                {
                    new GroupAlbum {GroupId = -13212026, AlbumId = 269329297},
                    new GroupAlbum {GroupId = -13212026, AlbumId = 269329282},
                    new GroupAlbum {GroupId = -13212026, AlbumId = 269329241},
                    new GroupAlbum {GroupId = -76629546, AlbumId = 203426992},
                    new GroupAlbum {GroupId = -76629546, AlbumId = 203426857},
                    new GroupAlbum {GroupId = -11571122, AlbumId = 229924509},
                    new GroupAlbum {GroupId = -11571122, AlbumId = 218215712},
                    new GroupAlbum {GroupId = -11571122, AlbumId = 229924703},
                    new GroupAlbum {GroupId = -42520747, AlbumId = 238108558},
                    new GroupAlbum {GroupId = -42520747, AlbumId = 255052787},
                    new GroupAlbum {GroupId = -42520747, AlbumId = 265095549},
                    new GroupAlbum {GroupId = -42520747, AlbumId = 265095678},
                    new GroupAlbum {GroupId = -42520747, AlbumId = 265095819},
                    new GroupAlbum {GroupId = -42520747, AlbumId = 265095843},
                    new GroupAlbum {GroupId = -42520747, AlbumId = 265095861},
                    new GroupAlbum {GroupId = -42520747, AlbumId = 265095887},
                    new GroupAlbum {GroupId = -82590437, AlbumId = 265820397},
                    new GroupAlbum {GroupId = -82590437, AlbumId = 265820440},
                    new GroupAlbum {GroupId = -82590437, AlbumId = 265820351},



                };
                    var userList = new List<string>
                    {

	                    "2329557afb5eaa5f8280b747b1ca43320eee63e7098c0ed8fcf802d94ea3692ca8bfc0416547cb7b7e15c",
	                    "74d89552338d10e3a6ddec113d6c5a481542afe13f176a5514303459ed9625ab47a4f68beff9499222b11",
	                    "bb15ee18ad62811a5dbe158b26f7dd7edf30fbf0c42d5d8ecd42c49778b94e3b2e49009f569f4cca1c1b0"
                    };
                    int activeUser = 0;
                    _api.Authorize(new ApiAuthParams
                    {
                        AccessToken = userList[activeUser]
                    });

                    foreach (var item in list)
                    {

                        try
                        {
                            var query = _api.Photo.Get(new PhotoGetParams
                            {
                                OwnerId = item.GroupId,
                                AlbumId = PhotoAlbumType.Id(item.AlbumId),
                                Reversed = true,
                                Extended = true,
                                Count = 500
                            });
                            List<WeaponList> weaponLists = new List<WeaponList>();
                            foreach (var photo in query.Where(w => w.Text == "" && w.Comments.Count != 0).ToList())
                            {

                                try
                                {
                                    var getComments = await _api.Photo.GetCommentsAsync(new PhotoGetCommentsParams
                                    {
                                        OwnerId = item.GroupId,
                                        PhotoId = (ulong)photo.Id,
                                        Count = 1,
                                    });
                                    weaponLists.Add(new WeaponList { Text = "", PhotoId = (long)photo.Id, AlbumId = item.AlbumId, GroupId = item.GroupId, Src = photo.Sizes.OrderByDescending(w => w.Height).First().Src.ToString(), FirstComment = getComments[0].Text });
                                    if (weaponLists.Count > 100)
                                    {
                                        db.BulkCopy(weaponLists);
                                        weaponLists = new List<WeaponList>();
                                    }
                                    string dfghf = "sdg";
                                }
                                catch (Exception e)
                                {
                                    activeUser++;
                                    if (activeUser == userList.Count)
                                    {
                                        activeUser = 0;
                                    }
                                    VkApi _api2 = new VkApi();
                                    await _api.LogOutAsync();
                                    _api2.Authorize(new ApiAuthParams
                                    {
	                                    AccessToken = userList[activeUser]
                                    });
                                    
                                    _api = _api2;
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            //await botClient.SendTextMessageAsync(chatId, $"{item.GroupId} {item.AlbumId} ");
                        }
                        

                    }
                    await _api.LogOutAsync();

                }
            }
            catch(Exception e)
            {

            }
            
        }

        public async static Task UpdateSite()
        {
	        var driver = new PhantomJSDriver();
	        driver.Manage().Window.Maximize();
	        driver.Navigate().GoToUrl("https://bottg.website/get");
	        driver.Close();
        }

        public async static Task CheckNewWeapon()
        {
	        try
            {
                await using (var db = new DbNorthwind())
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
                        foreach (var items in newWeaponList)
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