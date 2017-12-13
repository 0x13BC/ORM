using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ORM
{
    public class MServerSql : TypeSGBD
    {
        public SqlConnection coSqlSer;
        public SqlDataReader dtRdSql;
        private SqlCommand cmd;
        public string requete="Null";
        private string urlCon="Null";


        public void connectionSqlServ()
        {
            coSqlSer = new SqlConnection(urlCon);
            coSqlSer.Open();
            Console.WriteLine("toto is connected" + urlCon.ToString());
        }
        public void ExeDatabase()
        {
            cmd = new SqlCommand(requete, coSqlSer);
            dtRdSql = cmd.ExecuteReader();
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnected");
            dtRdSql.Dispose();
            coSqlSer.Dispose();
    }
        public void CreateUrl()
        {
            urlCon = "Server = localhost; Database = SQL-SERV";
                    //Server=[server_name];Database=[database_name];Trusted_Connection=true
        }
    }
}
