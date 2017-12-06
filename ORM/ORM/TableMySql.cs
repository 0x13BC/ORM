using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace ORM
{
    class TableMySql
    {
        public string nomtable;

        public ProprieteMySql[] table;

        public string lefichier;

        public TableMySql(string nom,ProprieteMySql[] latable)
        {
            this.nomtable = nom;
            this.table = latable;
            formatageXml();
            creationObject();
        }
        private void formatageXml()
        {
            lefichier = $"<{nomtable}>";

            lefichier += $"<{nomtable}>";
            
        }
        public void creationObject()
        {
            string nomFichier = ConfigurationManager.AppSettings["FICHIER"];
            using (StreamWriter sw = new StreamWriter(nomFichier, true, Encoding.UTF8))
            {
                sw.WriteLine(lefichier);
            }
        }
    }
}
