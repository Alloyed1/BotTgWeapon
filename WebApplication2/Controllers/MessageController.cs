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
using Microsoft.Extensions.Configuration;
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
        private IConfiguration _configuration;
        public MessageController(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }
        [HttpGet]
        [Route("/get")]
        public async Task<int> Get()
        {
            using(var db = new DbNorthwind())
            {
                return await db.WeaponList.Where(w => w.Text == "" && w.FirstComment != "0").CountAsync();
            }
        }

        [HttpPost]
        [Route("/post")]
        public async Task<OkResult> Post([FromBody]Update update)
        {

            if (update == null) return Ok();

            string cache = "";
            if (!_memoryCache.TryGetValue(update.Message.Chat.Id, out cache))
            {
                _memoryCache.Set(update.Message.Chat.Id, "1", new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(900)
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
                await command.Execute(message, botClient, _configuration);

                break;
            }

            if (!isCommand)
            {
                await commands[0].Execute(message, botClient, _configuration);
            }

            return Ok();

        }

        [HttpGet]
        [Route("/notify")]
        public async Task Notify()
        {
            _ = Task.Run(() => HangfireTasks.Notify());
        }

        [HttpGet]
        [Route("/parseall")]
        public async Task ParseAll()
        {
            _ = Task.Run(() => HangfireTasks.ParseAllAlbumsVkAsync());
        }
        [HttpGet]
        [Route("/parsecomment")]
        public async Task ParseComment()
        {
            _ = Task.Run(() => HangfireTasks.ParseComment()); 
        }

    }
}