using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ORM
{
    class MServerSql
    {
        private SqlConnection coSqlSer;
        public SqlDataReader dtRdSql;
        private SqlCommand cmd;
        public string requete;
        private string urlCon;

        public void connectionSqlServ()
        {
            coSqlSer = new SqlConnection(urlCon);
            coSqlSer.Open();
        }
        public void ExeDatabase()
        {
            cmd = new SqlCommand(requete, coSqlSer);
            dtRdSql = cmd.ExecuteReader();
        }

        public void Disconnect()
        {
            dtRdSql.Dispose();
            coSqlSer.Dispose();
    }
    }
}
