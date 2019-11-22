using System.ComponentModel.DataAnnotations;
using LinqToDB.Mapping;

namespace WebApplication2.Models
{
    [Table(Name = "UserViews")]
    public class UserViews
    {
        [Key]
        [Column(Name = "Id")]
        [PrimaryKey, Identity]
        public int Id { get; set; }
        [Column(Name = "GroupId")]
        public string GroupId { get; set; }
        [Column(Name = "PhotoId")]
        public string PhotoId { get; set; }
        [Column(Name = "ChatId")]
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
    }
}