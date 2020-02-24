using System.ComponentModel.DataAnnotations;
using LinqToDB.Mapping;

namespace WebApplication2.Models
{
    [Table(Name = "Kidals")]
    public class Kidals
    {
        [Key]
        [Column(Name = "Id")]
        [PrimaryKey, Identity]
        public int Id { get; set; }
        [Column(Name = "VkId")]
        public int VkId { get; set; }
        [Column(Name = "VkLink")]
        public string VkLink { get; set; }
        [Column(Name = "PostId")]
        public int PostId { get; set; }
        [Column(Name = "GroupId")]
        public string GroupId { get; set; }
        [Column(Name = "TopicId")]
        public string TopicId { get; set; }
        [Column(Name = "IsDelete")]
        public bool IsDelete { get; set; }
    }
}