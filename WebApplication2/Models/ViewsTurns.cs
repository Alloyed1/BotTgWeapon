using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
	[Table(Name = "ViewsTurns")]
	public class ViewsTurns
	{
		[Key]
		[Column(Name = "Id")]
		[PrimaryKey, Identity]
		public int Id { get; set; }
		[Column(Name = "WeaponListId")]
		public int WeaponListId { get; set; }
		public WeaponList Weapon { get; set; }

		[Column(Name = "ChatId")]
		public int ChatId { get; set; }
		public Chat Chat { get; set; }
		
	}
}
