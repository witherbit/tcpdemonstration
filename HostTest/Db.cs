using HostTest.Tables;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostTest
{
    internal static class Db
    {
        public static readonly string DatabaseName = "Wither";
        public static SqlSugarClient Sql {  get; private set; }
        public static void Initialize()
        {
            Tweak.WriteLine($"~m~{DateTime.Now.ToString("G")} [SQL]: Initializing...");
            Sql = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Server=.\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                LanguageType = LanguageType.English
            });
            Sql.Aop.OnLogExecuting = Logging;
            Create();
            Tweak.WriteLine($"~m~{DateTime.Now.ToString("G")} [SQL]: Initialized");
        }

        private static void Logging(string arg1, SugarParameter[] arg2)
        {
            Tweak.WriteLine($"~m~{DateTime.Now.ToString("G")} [SQL]: {arg1}");
        }

        public static void Create()
        {
            Sql.DbMaintenance.CreateDatabase(DatabaseName, null);
            Sql = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = $"Server=.\\SQLEXPRESS;Database={DatabaseName};Trusted_Connection=True;TrustServerCertificate=True;",
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,
                LanguageType = LanguageType.English
            });
            Sql.Aop.OnLogExecuting = Logging;
            Sql.CodeFirst.InitTables(typeof(Users));
            Sql.CodeFirst.InitTables(typeof(Connections));
        }

        public static Users GetUserByLogin(this string login)
        {
            var list = Sql.Queryable<Users>().Where(user => user.Login == login).ToList();
            if(list.Count > 0)
                return list[0];
            else
                return null;
        }

        public static Users GetUserById(this int id)
        {
            var list = Sql.Queryable<Users>().Where(user => user.Id == id).ToList();
            if (list.Count > 0)
                return list[0];
            else
                return null;
        }

        public static int InsertUser(this Users user)
        {
            return Sql.Insertable(user).ExecuteCommand();
        }
    }
}
