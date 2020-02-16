using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
	public class Settings
	{
		public int CountMessage { get; set; }
		public int CountMessageVip { get; set; }
		public int AdminChatId { get; set; }
		public List<string> Albums { get; set; }
	}
}
