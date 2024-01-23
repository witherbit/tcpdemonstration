using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostTest.Tables
{
    public class Connections
    {

        [SugarColumn(IsNullable = true)]
        public int UserId { get; set; }
        [SugarColumn(ColumnDataType = "Varchar(30)", IsNullable = false)]
        public string Address { get; set; }
        [SugarColumn(ColumnDataType = "Varchar(100)", IsNullable = false)]
        public string ConnectionId { get; set; }

        [SugarColumn(IsNullable = false)]
        public DateTime Connect { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime Disconnect { get; set; }

        [SugarColumn(IsNullable = false)]
        public bool IsAuthorized { get; set; }
    }
}
