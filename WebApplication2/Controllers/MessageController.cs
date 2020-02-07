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
        private int count = 0;
        public MessageController(IMemoryCache memoryCache, IConfiguration configuration)
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
            count = _configuration.GetSection("Settings").Get<Settings>().CountMessage;
        }
        [HttpGet]
        [Route("/get")]
        public async Task<int> Get()
        {
            using(var db = new DbNorthwind())
            {
                return await db.WeaponList.Where(w => w.Text == "").CountAsync();
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
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(600)
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
                _ = Task.Run(() => command.Execute(message, botClient, _configuration));

                break;
            }

            if (!isCommand)
            {
                await commands[0].Execute(message, botClient, _configuration);
            }

            return Ok();

        }

    }
}