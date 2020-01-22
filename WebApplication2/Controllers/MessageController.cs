using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using VkNet;
using VkNet.Enums.SafetyEnums;
using VkNet.Model;
using VkNet.Model.RequestParams;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IMemoryCache _memoryCache;
        public MessageController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }
        [HttpGet]
        [Route("/get")]
        public async Task<string> Get()
        {
            var api = new VkApi();
            string count = "123";
           
            return count;
        }

        [HttpPost]
        [Route("/send")]
        public async Task Send(string chatId, string photo, string caption, string link)
        {
            var botClient = await Bot.GetBotClientAsync();
            
            await botClient.SendPhotoAsync(chatId, photo: photo, caption: caption,
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithUrl("Перейти",
                        $"{link}")
                ));
        }
        
        [NonAction]
        public async Task Work(Update update)
        {
            
        }

        //[HttpGet]
        //[Route("/update")]
        //public async Task<string> Update()
        //{
        //    using(var db = new DbNorthwind())
        //    {
        //        using(var sr = new StreamReader(@"D:\weaponlist.txt"))
        //        {
        //            var text = sr.ReadToEnd();
        //            var list = JsonConvert.DeserializeObject<IEnumerable<WeaponList>>(text);
        //            db.WeaponList.Delete();
        //            db.WeaponList.BulkCopy(list);
        //            return list.Count().ToString();

        //        }
                

        //        //var list = db.WeaponList.ToList();
        //        //var text = JsonConvert.SerializeObject(list, Formatting.Indented);
        //        //var f = new FileInfo(@"D:\weaponlist.txt");
        //        //if (f.Exists)
        //        //{
        //        //    using(var sr = new StreamWriter(@"D:\weaponlist.txt"))
        //        //    {
        //        //        sr.Write(text);
        //        //    }
        //        //    return "Ok";
        //        //}
        //        //else
        //        //{
        //        //    return "nonexist";
        //        //}
        //    }
        //}
        

        [HttpPost]
        [Route("/post")]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            
            if (update == null) return Ok();

            string cache = "";
            if(!_memoryCache.TryGetValue(update.Message.Chat.Id, out cache))
            {
                _memoryCache.Set(update.Message.Chat.Id, "1", new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(800)
                });
            }
            else
            {
                return Ok();
            }

            

            var commands = Bot.Commands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();

            var isCommand = false;

            foreach (var command in commands)
            {
                if (!command.Contains(message)) continue;
                
                isCommand = true;
                _ = Task.Run(() => command.Execute(message, botClient));

                break;
            }

            if (!isCommand)
            {
                await commands[0].Execute(message, botClient);
            }

            return Ok();

        }
    }
}