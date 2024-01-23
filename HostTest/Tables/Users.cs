using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostTest.Tables
{
    public class Users
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnDataType = "Nvarchar(25)", IsNullable = false)]
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(20)", IsNullable = false)]
        public string Login { get; set; }
        [SugarColumn(ColumnDataType = "Varchar(100)", IsNullable = false)]
        public string Password { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(30)", IsNullable = true)]
        public string Email { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime Time { get; set; }
    }
}
