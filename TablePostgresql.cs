using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.IO;

namespace ORM
{
    class TablePostgresql
    {
        public string nomtable;

        public PropretyPostgresql[] table;

        public string lefichier;

        public void TableMySql(string nom, PropretyPostgresql[] latable)
        {
            this.nomtable = nom;
            this.table = latable;
            formatageXml();
            //creationObject();
        }
        private void formatageXml()
        {
            lefichier = $"<table nom={nomtable}>";
            foreach (PropretyPostgresql pro in table)
            {
                lefichier += $"<propriete>";
                lefichier += $"<nom>{pro.nom}<nom>";
                lefichier += $"<type>{pro.type}<type>";
                lefichier += $"<null>{pro.isNullable}<null>";
                lefichier += $"<primary>{pro.isPrimaryKey}<primary>";
                lefichier += $"<propriete>";
            }
            lefichier += $"<table>";

        }
       /* public void creationObject()
        {
            string nomFichier = ConfigurationManager.AppSettings["FICHIER"];
            using (StreamWriter sw = new StreamWriter(nomFichier, true, Encoding.UTF8))
            {
                sw.WriteLine(lefichier);
            }
        }*/

    }
}
