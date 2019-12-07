using System;
using System.ComponentModel.DataAnnotations;
using LinqToDB.Mapping;

namespace WebApplication2.Models
{
    [Table(Name = "WeaponList")]
    public class WeaponList
    {
        [Key]
        [Column(Name = "Id")]
        [PrimaryKey, Identity]
        public int Id { get; set; }
        [Column(Name = "Src")]
        public string Src { get; set; }
        [Column(Name = "Text")]
        public string Text { get; set; }
        [Column(Name = "GroupId")]
        public long GroupId { get; set; }
        [Column(Name = "AlbumId")]
        public long AlbumId { get; set; }
        [Column(Name = "FirstComment")]
        public string FirstComment { get; set; }
        [Column(Name = "PhotoId")]
        public long PhotoId { get; set; }

        [Column(Name = "StartTime")]
        public DateTime StartTime { get; set; }


    }
}