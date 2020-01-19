using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    public class MessageController : ControllerBase
    {

        [HttpGet]
        [Route("/get")]
        public async Task<string> Get()
        {
            var bot = await Bot.GetBotClientAsync();
            return bot.GetWebhookInfoAsync().Result.PendingUpdateCount.ToString();
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
        

        [HttpPost]
        [Route("/post")]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            if (update == null) return Ok();

            var commands = Bot.Commands;
            var message = update.Message;
            var botClient = await Bot.GetBotClientAsync();

            var isCommand = false;

            foreach (var command in commands)
            {
                if (!command.Contains(message)) continue;
                
                isCommand = true;
                _ = command.Execute(message, botClient);

                break;
            }

            if (!isCommand)
            {
                _ = commands[0].Execute(message, botClient);
            }
            
            
            return Ok();

        }
    }
}