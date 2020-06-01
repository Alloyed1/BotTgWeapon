using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IMemoryCache _memoryCache;
        private IConfiguration _configuration;
        public MessageController(IMemoryCache memoryCache,
            IConfiguration configuration
            )
        {
            _memoryCache = memoryCache;
            _configuration = configuration;
        }
        [HttpGet]
        [Route("/get")]
        public async Task<string> Get()
        {
            await using var db = new DbNorthwind();

            var count = await db.WeaponList.Where(w => w.FirstComment == "" || w.Text == "").CountAsync();
            var url = await db.WeaponList.Where(w => w.FileId == null).CountAsync();

            return count + Environment.NewLine + url;
        }
        [HttpPost]
        [Route("/post")]
        public async Task<OkResult> OkResultAsync()
        {
            return Ok();
        }
        [HttpGet]
        [Route("/notify")]
        public async Task<OkResult> Notify()
        {
            return Ok();
        }
        [HttpGet]
        [Route("/success")]
        public async Task<string> Success()
        {
            var settings = Startup.StaticConfig.GetSection("Settings").Get<Settings>().Subs;
            return settings.ToString();
        }
        [HttpGet]
        [Route("/error")]
        public async Task<OkResult> Error()
        {
            return Ok();
        }

        [HttpPost]
        [Route("/postt")]
        public async Task<OkResult> Post([FromBody] Update update)
        {

            if (update == null) return Ok();

            string cache = "";
            if(update.Message != null)
            {
                if (!_memoryCache.TryGetValue(update.Message.Chat.Id, out cache))
                {
                    _memoryCache.Set(update.Message.Chat.Id, "1", new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(400)
                    });
                }
                else
                {
                    return Ok();
                }
            }
            
            var botClient = await Bot.GetBotClientAsync();
            if (update.Type == UpdateType.Message)
            {
                var commands = Bot.Commands;
                var message = update.Message;
                
                await using var db = new DbNorthwind();
                var user = await db.Chats.FirstOrDefaultAsync(f => f.ChatId == update.Message.Chat.Id.ToString());
                if (user != null && message?.Text != "Главное меню")
                {
                    if (user.IsCheck)
                    {
                        await commands[19].Execute(message, botClient, _configuration, _memoryCache);
                        return Ok();
                    }
                }
                

                var isCommand = false;

                foreach (var command in commands)
                {
                    if (!command.Contains(message)) continue;

                    isCommand = true;
                    
                    await command.Execute(message, botClient, _configuration, _memoryCache);

                    break;
                }

                if (!isCommand)
                {
                    await commands[0].Execute(message, botClient, _configuration,_memoryCache);
                }
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                var commands = Bot.CommandsCallBack;
                var message = update.CallbackQuery;

                foreach (var command in commands)
                {
                    if (!command.Contains(message)) continue;
                    await command.Execute(message, botClient, _configuration);

                    break;
                }
            }



            return Ok();

        }

    }
}