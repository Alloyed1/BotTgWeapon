using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions;
using RestSharp.Serialization.Json;
using Telegram.Bot.Types.ReplyMarkups;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.Attachments;
using VkNet.Model.RequestParams;

namespace WebApplication2.Models
{

    public  class HangfireTasks
    {
	    private static IConfiguration _configuration;
	    public HangfireTasks(IConfiguration configuration)
	    {
		    _configuration = configuration;
	    }
	    
	    static List<string> userList = new List<string>
	    {
		    "2329557afb5eaa5f8280b747b1ca43320eee63e7098c0ed8fcf802d94ea3692ca8bfc0416547cb7b7e15c",
		    "74d89552338d10e3a6ddec113d6c5a481542afe13f176a5514303459ed9625ab47a4f68beff9499222b11",
		    "bb15ee18ad62811a5dbe158b26f7dd7edf30fbf0c42d5d8ecd42c49778b94e3b2e49009f569f4cca1c1b0"
	    };
	    
	    
		
	    public partial class Temperatures
	    {
		    [JsonProperty("response")]
		    public Response Response { get; set; }
	    }

	    public partial class Response
	    {
		    [JsonProperty("count")]
		    public long Count { get; set; }

		    [JsonProperty("items")]
		    public List<Item> Items { get; set; }
	    }
	    public partial class Item
	    {
		    [JsonProperty("id")]
		    public long Id { get; set; }

		    [JsonProperty("album_id")]
		    public long AlbumId { get; set; }

		    [JsonProperty("owner_id")]
		    public long OwnerId { get; set; }

		    [JsonProperty("user_id")]
		    public long UserId { get; set; }

		    [JsonProperty("sizes")]
		    public Size[] Sizes { get; set; }

			[JsonProperty("comments")]
			public Comments Comments { get; set; }

			[JsonProperty("text")]
		    public string Text { get; set; }
		    [JsonProperty("photo_604")]
		    public string Src { get; set; }
		    [JsonProperty("date")]
		    public long Date { get; set; }
		    [JsonProperty("from_id")]
		    public int FromId { get; set; }
	    }
	    public partial class Size
	    {
		    [JsonProperty("type")]
		    public string Type { get; set; }

		    [JsonProperty("url")]
		    public string Url { get; set; }

		    [JsonProperty("width")]
		    public long Width { get; set; }

		    [JsonProperty("height")]
		    public long Height { get; set; }
	    }
		

		public class Items
		{
			public string text { get; set; }
			[JsonProperty("from_id")]
			public int FromId { get; set; }
			[JsonProperty("date")]
			public int date { get; set; }
			[JsonProperty("id")]
			public int id { get; set; }
		}

		public class Comments
		{
			[JsonProperty("count")]
			public int Count { get; set; }
		}

		public class Root
		{
			public List<Item> response { get; set;}
		}
		

		public class Responsee
		{
			public int count { get; set; }
			public List<Item> items { get; set; }
		}

		public class RootObject
		{
			public Response response { get; set; }
		}

		public static DateTime UnixTimeToDateTime(long unixtime)
		{
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixtime).ToLocalTime();
			return dtDateTime;
		}

		public static async Task ParseKidalId()
		{
			using var db = new DbNorthwind();
			var list = await db.Kidals.Where(w => w.VkId == 0 && !w.IsDelete).ToListAsync();
			var client = new RestClient("https://api.vk.com/method");
			foreach (var item in list)
			{
				var request = new RestRequest("users.get");
				
				
				request.AddQueryParameter("access_token", userList[0]);
				request.AddQueryParameter("user_ids", item.VkLink.Replace("vk.com/", ""));
				request.AddQueryParameter("v", "5.103");

				var res = await client.ExecuteAsync(request);

				var res2 = JsonConvert.DeserializeObject<Root>(res.Content);
				if(res2.response == null)
				{
					await db.Kidals.Where(w => w == item)
						.Set(s => s.IsDelete, true)
						.UpdateAsync();
				}
				else
				{
					await db.Kidals.Where(w => w == item)
					.Set(s => s.VkId, res2.response[0].Id)
					.UpdateAsync();
				}

				
			}
		}
		

		public static async Task ParseKidals()
		{
			var client = new RestClient("https://api.vk.com/method");
			var kidals = Startup.StaticConfig.GetSection("Settings").Get<Settings>().Kidals;
			foreach (var kidal in kidals)
			{
				var request = new RestRequest("board.getComments");

				var groupId = kidal.Replace("https://vk.com/topic-", "").Split('_')[0];
				var topicId = kidal.Replace("https://vk.com/topic-", "").Split('_')[1];

				request.AddQueryParameter("access_token", userList[0]);
				request.AddQueryParameter("group_id", groupId);
				request.AddQueryParameter("topic_id", topicId);
				//request.AddQueryParameter("preview_length", "-");
				request.AddQueryParameter("sort", "desc");
				request.AddQueryParameter("count", "100");
				request.AddQueryParameter("v", "5.103");

				var res = await client.ExecuteAsync(request);

				var response = JsonConvert.DeserializeObject<RootObject>(res.Content);
				
				await using var db = new DbNorthwind();
				var kidalsList = await db.Kidals.ToListAsync();
				var newKidalsList = new List<Kidals>();
				if (response.response != null)
				{
					foreach (var item in response.response.Items)
					{
						var regex = new Regex(@"vk.com/([^ \n]+)");
						MatchCollection matches = regex.Matches(item.Text);
						if (matches.Any())
						{
							foreach (Match math in matches)
							{
								if (math.Value.Contains("photo") ||
								    math.Value.Contains("wall") || math.Value.Contains("public"))
								{
									continue;
								}

								if (kidalsList.FirstOrDefault(f => f.VkLink == math.Value.Replace(",", "").Replace("?", "").Replace("!", "").Replace(")", "")) == null
								    || newKidalsList.Select(s => s.VkLink).Contains(math.Value.Replace(",", "").Replace("?", "").Replace("!", "").Replace(")", "")))
								{
									var regex2 = new Regex(@"vk.com/id[0-9\(\)]+");
									MatchCollection matches2 = regex2.Matches(math.Value);
									if (matches2.Any())
									{
										foreach (Match math2 in matches2)
										{
											try
											{
												if (kidalsList.FirstOrDefault(f =>
													    f.VkLink == math2.Value.Replace(",", "").Replace("?", "")
														    .Replace("!", "").Replace(")", "")) == null
												    || newKidalsList.Select(s => s.VkLink)
													    .Contains(math2.Value.Replace(",", "").Replace("?", "")
														    .Replace("!", "").Replace(")", "")))
												{
													newKidalsList.Add(new Kidals()
													{
														PostId = (int) item.Id,
														VkId = int.Parse(math2.Value.Replace("vk.com/id", "")
															.Replace(")", "")),
														VkLink = math.Value.Replace(",", "").Replace("?", "")
															.Replace("!", "").Replace(")", ""),
														IsDelete = false,
														GroupId = groupId,
														TopicId = topicId
													});
												}
											}
											catch { }
										}
										
											
									}
									else
									{
										try
										{
											newKidalsList.Add(new Kidals()
											{
												PostId = (int)item.Id,
												VkId = 0,
												VkLink = math.Value.Replace(",", "").Replace("?", "").Replace("!", "").Replace(")", ""),
												IsDelete = false,
												GroupId = groupId,
												TopicId = topicId
											});
										}
										catch { }

									}
								}
									
							}
							
						}

					}
				}
				

				db.BulkCopy(newKidalsList);



			}
		}
		public static async Task ParseComment()
		{
			var listParse = new List<WeaponList>();
			
			var client = new RestClient("https://api.vk.com/method");
			var active_user = 1;
			using (var db = new DbNorthwind())
			{
				listParse = await db.WeaponList
					.Where(w => w.Text == "" && int.Parse(w.FirstComment) > 0)
					.Take(4) 
					.ToListAsync();

				foreach (var item in listParse)
				{
					var request = new RestRequest("photos.getComments");

					request.AddQueryParameter("access_token", userList[active_user]);
					request.AddQueryParameter("owner_id", item.GroupId.ToString());
					request.AddQueryParameter("photo_id", item.PhotoId.ToString());
					//request.AddQueryParameter("preview_length", "-");
					request.AddQueryParameter("sort", "asc");
					request.AddQueryParameter("count", "1");
					request.AddQueryParameter("v", "5.33");

					var res = await client.ExecuteAsync(request);

					var reslist = JsonConvert.DeserializeObject<RootObject>(res.Content);
					if (reslist.response != null)
					{
						if(reslist.response.Count != 0)
						{
							if(reslist.response.Items.First().Text == "")
							{
								await db.WeaponList.Where(f => f.Id == item.Id).DeleteAsync();
							}
							else
							{
								item.Text = reslist.response.Items.FirstOrDefault()?.Text;
								await db.WeaponList.Where(w => w.Id == item.Id)
									.Set(s => s.Text, item.Text)
									.UpdateAsync();
							}
							
						}
						
					}
					
				}
				Console.WriteLine("Koool!");
				
				

			}
		}

		public static async Task ParseAllTopicVkAsync()
		{
			var list = Startup.StaticConfig.GetSection("Settings").Get<Settings>().Topics;
			
			var active_user = 1;
			
			var addList = new List<WeaponList>();
			var removeList = new List<WeaponList>();
			var weaponListDb = new List<WeaponList>();

			await using var db = new DbNorthwind();
			weaponListDb = await db.WeaponList.Where(w => !w.IsAlbum).ToListAsync();
			
			var albumsList = new List<GroupAlbum>();
			
			list.ForEach(f =>
			{
				f = f.Replace("https://vk.com/topic-", "");
				var mass = f.Split('_').ToList();
				try
				{
					albumsList.Add(new GroupAlbum()
					{
						AlbumId = long.Parse(mass[1]),
						GroupId = long.Parse(mass[0]),
						Category = int.Parse(mass[2])
					});
				}
				catch { }

			});
			
			var client = new RestClient("https://api.vk.com/method");

			foreach (var item in albumsList)
			{
				
				var api = new VkApi();
				
				api.Authorize(new ApiAuthParams
				{
					AccessToken = userList[0]
				});

				var list2 = await api.Board.GetCommentsAsync(new BoardGetCommentsParams()
				{
					Count = 5,
					Sort = CommentsSort.Desc,
					GroupId = item.GroupId,
					TopicId = item.AlbumId,
				});

				foreach (var photo in list2.Items)
				{
					var src = ((Photo) photo.Attachments.FirstOrDefault()?.Instance).Sizes
						.OrderByDescending(d => d.Height).FirstOrDefault()?.Src;
				}
				
				
			}
			
			
			
			

		}


		public static async Task ParseAllAlbumsVkAsync()
		{
			
			var active_user = 1;
			
			var addList = new List<WeaponList>();
			var removeList = new List<WeaponList>();
			var weaponListDb = new List<WeaponList>();
			
			

			using (var db = new DbNorthwind())
			{
				weaponListDb = db.WeaponList.ToList();
			}
			Console.WriteLine("WeaponGet");
			var client = new RestClient("https://api.vk.com/method");

			var settings = Startup.StaticConfig.GetSection("Settings").Get<Settings>();
			
			var albumsList = new List<GroupAlbum>();
			
			settings.Albums.ForEach(f =>
			{
				f = f.Replace("https://vk.com/album", "");
				var mass = f.Split('_').ToList();
				try
				{
					albumsList.Add(new GroupAlbum()
					{
						AlbumId = long.Parse(mass[1]),
						GroupId = long.Parse(mass[0]),
						Category = int.Parse(mass[2])
					});
				}
				catch { }

			});

			var list = new List<GroupAlbum>();
			foreach (var group in albumsList)
			{
				var request = new RestRequest("photos.get");

				request.AddQueryParameter("access_token", userList[active_user]);
				request.AddQueryParameter("owner_id", group.GroupId.ToString());
				request.AddQueryParameter("album_id", group.AlbumId.ToString());
				request.AddQueryParameter("rev", "1");
				request.AddQueryParameter("extended", "1");
				request.AddQueryParameter("count", "1000");
				request.AddQueryParameter("v", "5.52");
				request.AddQueryParameter("extended", "1");

				var res = client.Execute(request).Content;
				var photos = new Temperatures();
				photos = JsonConvert.DeserializeObject<Temperatures>(res);
				
				if (photos.Response == null)
				{
					list.Add(group);
					continue;
				}

				Console.WriteLine(photos.Response.Items.Count);


				if (photos.Response.Items.Any())
				{
					var photosList = new List<Item>();
					foreach (var ph in photos.Response.Items)
					{

						if (weaponListDb.FirstOrDefault(f => f.Src == ph.Src) == null)
						{
							photosList.Add(ph);
						}


					}
					foreach (var photo in photosList)
					{
						addList.Add(new WeaponList()
						{
							Text = photo.Text,
							PhotoId = photo.Id,
							AlbumId = photo.AlbumId,
							GroupId = photo.OwnerId,
							Src = photo.Src,
							StartTime = UnixTimeToDateTime(photo.Date),
							FirstComment = photo.Comments.Count.ToString(),
							Category = group.Category,
							UserId = (int)photo.UserId,
							IsAlbum = true
						}) ;
					}

					var localRemoveList = weaponListDb.Where(w => w.GroupId == group.GroupId
														   && w.AlbumId == group.AlbumId
														   && photos.Response.Items.FirstOrDefault(f => f.Id == w.PhotoId) == null).ToList();

					removeList.AddRange(localRemoveList);
				}


				Thread.Sleep(900);

			}
			if (list.Any())
			{
				if (Settings.LastParse < DateTime.Now.AddHours(-1))
				{
					string albumsErrorList = "";

					list.ForEach(x => albumsErrorList += $"https://vk.com/album{x.GroupId}_{x.AlbumId} (https://vk.com/public{x.GroupId.ToString().Replace("-", "")})" + Environment.NewLine);

					Settings.LastParse = DateTime.Now;
					//var bot = await Bot.GetBotClientAsync();
					//await bot.SendTextMessageAsync(settings.AdminChatId,
					//	$"Не удалось спарсить альбомы: {albumsErrorList}");
				}
			}
			await using (var db = new DbNorthwind())
			{
				if (addList.Any())
				{
					db.BulkCopy(addList);
				}
				foreach (var item in removeList)
				{
					db.WeaponList
						.Where(w => w.Id == item.Id)
						.Delete();
				}
			}

			
			Console.WriteLine($"Удалено {removeList.Count}, добавлено {addList.Count}");


		}

		public static async Task Notify()
		{
			using (var db = new DbNorthwind())
			{
				var countMessages = 0;
				var botClient = await Bot.GetBotClientAsync();

				var addViewList = new List<UserViews>();
				
				var dbList = await db.WeaponList.Where(w => w.Text != "").ToListAsync();
				var list = await db.LastQuery.Where(w => w.IsWatching == 1).ToListAsync();
				foreach (var item in list)
				{
					var userView = await db.UserViews.Where(w => w.ChatId == int.Parse(item.ChatId)).ToListAsync();
					var notifyList = dbList
						.Where(w => !userView.Select(s => s.PhotoId).ToList().Contains(w.PhotoId.ToString()) 
						            && w.Text.ToLower().Contains(item.Query.ToLower())
						            && w.StartTime > item.StartWatchTime)
						.Take(3)
						.ToList();

					foreach (var notif in notifyList.OrderBy(d => d.StartTime))
					{
						var text = notif.Text;
						if(notif.Text.Length >= 430)
						{
							text = notif.Text.Substring(0, 430);
						}
						
						var kidal = await db.Kidals.FirstOrDefaultAsync(f => f.VkId == notif.UserId);
						if (kidal != null)
						{
							var listMarkup = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
							{
								new InlineKeyboardButton()
								{
									Text = "Перейти",
									Url = $"https://vk.com/photo{notif.GroupId}_{notif.PhotoId}",
								},
								new InlineKeyboardButton()
								{
									Text = "❗Мошенник.Инфо.",
									Url = $"https://vk.com/topic-{kidal.GroupId}_{kidal.TopicId}?post={kidal.PostId}",
								},
								InlineKeyboardButton.WithCallbackData("Проверить", "Проверить" + notif.UserId)

							});

							text += Environment.NewLine + Environment.NewLine + "❗❗❗ Найден в списках мошенников ❗❗❗";
							
							_ = botClient.SendPhotoAsync(item.ChatId, photo: notif.Src, caption: text,
								replyMarkup: listMarkup
							);

							countMessages++;
							
						}
						else
						{
							var listMarkup = new InlineKeyboardMarkup(new List<InlineKeyboardButton>()
							{
								new InlineKeyboardButton()
								{
									Text = "Перейти",
									Url = $"https://vk.com/photo{notif.GroupId}_{notif.PhotoId}",
								},
								InlineKeyboardButton.WithCallbackData("Проверить", "Проверить" + notif.UserId)
					
							});
				
							text += Environment.NewLine + Environment.NewLine + "✅ В списках мошенников не найден";
							
							text += Environment.NewLine + Environment.NewLine + $"📅 Дата публикации: {notif.StartTime:dd'/'MM'/'yyyy HH:mm:ss}";
						
							_ = botClient.SendPhotoAsync(item.ChatId, photo: notif.Src, caption: text,
								replyMarkup: listMarkup
								);

							countMessages++;
				

						}
						
						
						
					}
					foreach (var itm in notifyList)
					{
						addViewList.Add(new UserViews()
						{
							ChatId = int.Parse(item.ChatId),
							PhotoId = itm.PhotoId.ToString(),
							GroupId = itm.GroupId.ToString()
						});
					}

					if (countMessages >= 20)
					{
						break;
					}
					
				}

				db.UserViews.BulkCopy(addViewList);
			}
		}
	}
}