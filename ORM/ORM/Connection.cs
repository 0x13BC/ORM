using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using MySql.Data.MySqlClient;

namespace ORM
{
    public class ConnectionMySQL
    {
        
        private MySqlConnection connection;
       
        // Constructeur
        public ConnectionMySQL(string connectionString)
        {
            // Création de la chaîne de connexion
            connectionString  = "SERVER=127.0.0.1; DATABASE=mli; UID=root; PASSWORD=";
            this.connection = new MySqlConnection(connectionString);
        }

        // Méthode pour initialiser la connexion
        private Boolean InitConnexion()
        {
            try
            {
                if(connection.State == ConnectionState.Closed)
                {

                }

            }
            catch {

            }
            return false;
        }
        
    }
}
