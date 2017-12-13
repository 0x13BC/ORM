using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using System.Data;

namespace ORM
{
    class Connection
    {
        public void ConnectionPostgreSql(string Connection)
        {
            var connString = "Host=myserver;Username=mylogin;Password=mypass;Database=mydatabase";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
            }
        }
    }
}