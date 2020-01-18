using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Models
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {

        }
        


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<Favorite>().HasKey(k => new { k.Id, k.UserId });
        }
        public DbSet<Chat> Chats  { get; set; }
        public DbSet<WeaponList> WeaponList { get; set; }
        public DbSet<LastQuery> LastQuery { get; set; }
        public DbSet<UserViews> UserViews { get; set; }
		public DbSet<ViewsTurns> ViewsTurns { get; set; }
	}
}