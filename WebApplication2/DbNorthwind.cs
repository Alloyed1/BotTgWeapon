using LinqToDB;
using WebApplication2.Models;
using Chat = Telegram.Bot.Types.Chat;

namespace WebApplication2
{
    public class DbNorthwind : LinqToDB.Data.DataConnection
    {
        public DbNorthwind() : base(ProviderName.SqlServer, @"Data Source=SQL6007.site4now.net;Initial Catalog=DB_A4FCA2_tgbot;User Id=DB_A4FCA2_tgbot_admin;Password=telegramBOT!2;") { }

        public ITable<Chat> Chats => GetTable<Chat>();
        public ITable<WeaponList> WeaponList => GetTable<WeaponList>();
        public ITable<LastQuery> LastQuery => GetTable<LastQuery>();
        public ITable<UserViews> UserViews => GetTable<UserViews>();
    }
}