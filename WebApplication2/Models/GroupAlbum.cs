namespace WebApplication2.Models
{
    public class GroupAlbum
    {
        public long GroupId { get; set; }
        public long AlbumId { get; set; }
        public int Category { get; set; } = 0;
    }
}