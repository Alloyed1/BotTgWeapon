using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
	[Table(Name = "Querys")]
	public class Querys
	{
		[Key]
		[Column(Name = "Id")]
		[PrimaryKey, Identity]
		public int Id { get; set; }
		[Column(Name = "ChatId")]
		public string ChatId { get; set; }
		[Column(Name = "Query")]
		public string Query { get; set; }
		[Column(Name = "Date")]
		public DateTime Date { get; set; }
	}
}
