using LinqToDB;
using WebApplication2.Models;
using Chat = WebApplication2.Models.Chat;

namespace WebApplication2
{
    public class DbNorthwind : LinqToDB.Data.DataConnection
    {
        public DbNorthwind() : base(ProviderName.SqlServer, @"Data Source=wpl33.hosting.reg.ru;Initial Catalog=u0865575_dbdb;User Id=u0865575_userDb;Password=J59&zx9i;") { }

        public ITable<Chat> Chats => GetTable<Chat>();
        public ITable<WeaponList> WeaponList => GetTable<WeaponList>();
        public ITable<LastQuery> LastQuery => GetTable<LastQuery>();
        public ITable<UserViews> UserViews => GetTable<UserViews>();
        public ITable<ViewsTurns> ViewsTurns => GetTable<ViewsTurns>();
    }
}