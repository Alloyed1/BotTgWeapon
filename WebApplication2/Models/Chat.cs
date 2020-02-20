using System.ComponentModel.DataAnnotations;
using LinqToDB.Mapping;

namespace WebApplication2.Models
{
    [Table(Name = "Chats")]
    public class Chat
    {
        [Key]
        [Column(Name = "Id")]
        [PrimaryKey, Identity]
        public int Id { get; set; }
        [Column(Name = "FirstName")]
        public string FirstName { get; set; }
        [Column(Name = "LastName")]
        public string LastName { get; set; }
        [Column(Name = "UserName")]
        public string UserName { get; set; }
        [Column(Name = "ChatId")]
        public string ChatId { get; set; }
        [Column(Name = "CategorySearch")]
        public string CategorySearch { get; set; }

    }
}