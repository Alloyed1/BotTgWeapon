using System.ComponentModel.DataAnnotations;
using LinqToDB.Mapping;

namespace WebApplication2.Models
{
    [Table(Name = "LastQuery")]
    public class LastQuery
    {
        [Key]
        [Column(Name = "Id")]
        [PrimaryKey, Identity]
        public int Id { get; set; }
        [Column(Name = "Query")]
        public string Query { get; set; }
        [Column(Name = "ChatId")]
        public string ChatId { get; set; }
        [Column(Name = "IsWatching")]
        public int IsWatching { get; set; }
    }
}