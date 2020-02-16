using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
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

	    static List<GroupAlbum> groupList = new List<GroupAlbum>
	    {
		    new GroupAlbum() {GroupId = -76629546, AlbumId = 203426857},
		    new GroupAlbum() {GroupId = -11571122, AlbumId = 229924509},
		    new GroupAlbum() {GroupId = -76629546, AlbumId = 203426992},
		    new GroupAlbum() {GroupId = -76629546, AlbumId = 203426935},
		    new GroupAlbum() {GroupId = -11571122, AlbumId = 218215712},
		    new GroupAlbum() {GroupId = -11571122, AlbumId = 229924703},
		    new GroupAlbum() {GroupId = -42520747, AlbumId = 265095887},
		    new GroupAlbum() {GroupId = -42520747, AlbumId = 265095549},
		    new GroupAlbum() {GroupId = -42520747, AlbumId = 255052787},
		    new GroupAlbum() {GroupId = -13212026, AlbumId = 271731709},
		    new GroupAlbum() {GroupId = -13212026, AlbumId = 270419996},
		    new GroupAlbum() {GroupId = -13212026, AlbumId = 270419973}
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
		}

		public class Comments
		{
			[JsonProperty("count")]
			public int Count { get; set; }
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

			var settings = _configuration.GetSection("Settings").Get<Settings>();
			
			var albumsList = new List<GroupAlbum>();
			
			settings.Albums.ForEach(f =>
			{
				f = f.Replace("https://vk.com/album", "");
				albumsList.Add(new GroupAlbum()
				{
					AlbumId = long.Parse(f.Split('_')[0]),
					GroupId = long.Parse(f.Split('_')[1])
				});
			});
			
			
			
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
					var bot = await Bot.GetBotClientAsync();
					await bot.SendTextMessageAsync(settings.AdminChatId,
						$"Не удалось спарсить альбом: https://vk.com/album{group.GroupId}_{group.AlbumId}");
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
							FirstComment = photo.Comments.Count.ToString()
						}) ;
					}

					var localRemoveList = weaponListDb.Where(w => w.GroupId == group.GroupId
														   && w.AlbumId == group.AlbumId
														   && photos.Response.Items.FirstOrDefault(f => f.Id == w.PhotoId) == null).ToList();

					removeList.AddRange(localRemoveList);
				}




			}
			Console.WriteLine("Finish");
			using (var db = new DbNorthwind())
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
						if(notif.Text.Length >= 450)
						{
							text = notif.Text.Substring(0, 450);
						}
						
						text += Environment.NewLine + Environment.NewLine + $"Дата публикации: {notif.StartTime}";
						
						_ = botClient.SendPhotoAsync(item.ChatId, photo: notif.Src, caption: text,
							replyMarkup: new InlineKeyboardMarkup(
								InlineKeyboardButton.WithUrl("Перейти",
									$"https://vk.com/photo{notif.GroupId}_{notif.PhotoId}")
							));

						countMessages++;
						
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