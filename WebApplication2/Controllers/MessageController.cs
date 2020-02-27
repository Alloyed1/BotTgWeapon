using System;
using System.Linq;

using System.Threading.Tasks;
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
        public async Task<int> Get()
        {

            using(var db = new DbNorthwind())
            {
                return await db.WeaponList.Where(w => w.Text == "" && w.FirstComment != "0").CountAsync();
            }
        }

        [HttpPost]
        [Route("/post")]
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
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(500)
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
                Console.WriteLine("123");
                var commands = Bot.Commands;
                var message = update.Message;
                

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
            }
            else if (update.Type == UpdateType.CallbackQuery)
            {
                await botClient.AnswerCallbackQueryAsync(
                    callbackQueryId: update.CallbackQuery.Id,
                    text: $"Received {update.CallbackQuery.Data}"
                );
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