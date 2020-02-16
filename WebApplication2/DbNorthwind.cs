using LinqToDB;
using WebApplication2.Models;
using Chat = WebApplication2.Models.Chat;

namespace WebApplication2
{
    public class DbNorthwind : LinqToDB.Data.DataConnection
    {
        public DbNorthwind() : base(ProviderName.PostgreSQL95, @"Host=45.144.64.224;Port=5432;Database=database;Username=postgres;Password=cw42puQAZ") { }

        public ITable<Chat> Chats => GetTable<Chat>();
        public ITable<WeaponList> WeaponList => GetTable<WeaponList>();
        public ITable<LastQuery> LastQuery => GetTable<LastQuery>();
        public ITable<UserViews> UserViews => GetTable<UserViews>();
        public ITable<ViewsTurns> ViewsTurns => GetTable<ViewsTurns>();
        public ITable<Querys> Querys => GetTable<Querys>();
    }
}