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
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    public class MessageController : ControllerBase
    {
        public static readonly JsonSerializerSettings ConverterSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            }
        };

        [HttpGet]
        [Route("/get")]
        public async Task<string> Get()
        {
            return "123123";
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
                if (command.Contains(message))
                {
                    isCommand = true;
                    command.Execute(message, botClient);

                    break;
                }
            }

            if (!isCommand)
            {
                commands[0].Execute(message, botClient);
            }

            return Ok();

        }
    }
}