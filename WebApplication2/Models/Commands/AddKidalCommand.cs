using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace WebApplication2.Models.Commands
{
	public class AddKidalCommand : CommandMessage
	{
		public override string Name => @"/add";
		public override bool Contains(Message message)
		{
			if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}
		public override async Task Execute(Message message, TelegramBotClient botClient, Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			var adminChatId = Startup.StaticConfig.GetSection("Settings").Get<Settings>().AdminChatId;
			var chatId = message.Chat.Id;

			if (adminChatId != chatId)
			{
				return;
			}
			
			var kidal = new Kidals();
			var request = new RestRequest("board.getComments");
			
			var client = new RestClient("https://api.vk.com/method");

			var groupId = message.Text.Replace("/add https://vk.com/topic-", "").Split('_')[0];
			var topicId = message.Text.Replace("/add https://vk.com/topic-", "").Split('_')[1].Split('?')[0];
			var commentId = message.Text.Replace("/add https://vk.com/topic-", "").Split('_')[1].Split('?')[1].Replace("post=", "");
			
			Console.WriteLine($"{groupId},{topicId},{commentId}");

			request.AddQueryParameter("access_token", "2329557afb5eaa5f8280b747b1ca43320eee63e7098c0ed8fcf802d94ea3692ca8bfc0416547cb7b7e15c");
			request.AddQueryParameter("group_id", groupId);
			request.AddQueryParameter("topic_id", topicId);
			request.AddQueryParameter("start_comment_id", commentId);
			request.AddQueryParameter("sort", "desc");
			request.AddQueryParameter("count", "1");
			request.AddQueryParameter("v", "5.103");

			var res = await client.ExecuteAsync(request);

			var response = JsonConvert.DeserializeObject<HangfireTasks.RootObject>(res.Content);
				
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
			
			
			if (newKidalsList.Any())
			{
				db.BulkCopy(newKidalsList);
				var resString = "Успешно добавлено" + Environment.NewLine;
				newKidalsList.ForEach(x => resString += x.VkLink + Environment.NewLine);
				await botClient.SendTextMessageAsync(chatId, $"{resString}");
				
			}
			else
			{
				await botClient.SendTextMessageAsync(chatId, $"Аккаунт уже есть");
			}
			
		}
	}
}
