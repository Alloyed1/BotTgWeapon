using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Telegram.Bot.Types.ReplyMarkups;
using VkNet;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace WebApplication2.Models
{
    public static class VkBoy
    {
        public async static Task GetWeaponList()
        {
            VkApi _api = new VkApi();
            var userList = new List<string>
                {
                    //"+79164676167;!Krot0070",
                    "+79015119808;!Krot0070",
                    //"+79015349585;!Krot0070",
                    "+79067688669;cw42PU",
					"+79164676167;!Krot0070"
				};
            try
            {
                _api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 123456,
                    Login = "+79067688669",
                    Password = "cw42PU",
                    Settings = Settings.Photos
                });
            }
            catch(Exception e)
            {
                _api.Authorize(new ApiAuthParams
                {
                    ApplicationId = 123456,
                    Login = "89015119808",
                    Password = "!Krot0070",
                    Settings = Settings.All
                });
            }
            
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
                    //"+79164676167;!Krot0070",
                    "+79015119808;!Krot0070",
                    //"+79015349585;!Krot0070",
                    "+79067688669;cw42PU",
					"+79164676167;!Krot0070"
				};
                    int activeUser = 0;
                    _api.Authorize(new ApiAuthParams
                    {
                        ApplicationId = 123456,
                        Login = userList[activeUser].Split(';')[0],
                        Password = userList[activeUser].Split(';')[1],
                        Settings = Settings.All
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
                                Count = 1000
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
                                    _api2.Authorize(new ApiAuthParams
                                    {
                                        ApplicationId = 123456,
                                        Login = userList[activeUser].Split(';')[0],
                                        Password = userList[activeUser].Split(';')[1],
                                        Settings = Settings.All
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

                }
            }
            catch(Exception e)
            {

            }
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