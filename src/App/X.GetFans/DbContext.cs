using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace X.Wx
{
    /// <summary>
    /// 在“管理NUGET程序包”安装LINQ程序包
    /// 然后使用DataContext就可以连接sqlite了
    /// 引用要添加  system.Data.Linq;system.Data.SQLite;system.Data.SQLite.Linq
    public class LpwDB : DataContext
    {
        public LpwDB(string connection, MappingSource mappingSource) : base(connection, mappingSource) { }
        public LpwDB(IDbConnection connection, MappingSource mappingSource) : base(connection, mappingSource) { }
        public LpwDB(string connectionString) : base(new SQLiteConnection(connectionString)) { }
        public LpwDB(IDbConnection connection) : base(connection) { }

        [Table(Name = "x_contact")]
        public class x_contact
        {
            [Column(Name = "contact_id", DbType = "bigint", IsPrimaryKey = true)]
            public int contact_id { get; set; }
            [Column(Name = "nickname", DbType = "nvarchar(250)")]
            public string nickname { get; set; }
        }
    }
}
