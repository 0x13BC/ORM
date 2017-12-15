using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
namespace ORM
{
   public class PostgreSql
    {
        public NpgsqlConnection conPostgeSql;
        public NpgsqlDataReader deconnPostgreSql;
        private NpgsqlCommand cmd;
        public string requete = "Null";
        private string Connetion = "Null";


        public void connectionSqlServ()
        {
            conPostgeSql = new NpgsqlConnection(Connetion);
            conPostgeSql.Open();
            Console.WriteLine("Vous êtes bien connectée a la base de données" + Connetion.ToString());
        }
        public void ExeDatabase()
        {
            cmd = new NpgsqlCommand(requete, conPostgeSql);
            deconnPostgreSql = cmd.ExecuteReader();
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnected");
            conPostgeSql.Dispose();
        }
        public void CreateUrl()
        {
            Connetion = "Server=localhost ;Port=5432 ;User Id=postgresql-x64-9.3 ;Password='' ;Database=ORM ";
            //Connetion = "Database = ORM ;Data Source=.;Initial Catalog=ORM;User ID=sa;Password=12345";
        }
    }
}
