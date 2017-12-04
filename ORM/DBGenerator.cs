using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb;
using System.Configuration;

namespace com.castsoftware.tools
{
    class DBGenerator
    {
        private OleDbConnection _connection;
                
        #region CONSTANTES
        /// <summary>
        /// 0 : Nom de la table auquel on a enleve le premier 'T'.
        /// </summary>
        private const string DROP_VIEW =
            "if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[V{0}]') "
            + "and OBJECTPROPERTY(id, N'IsView') = 1)\r\n"
            + "\tdrop view [dbo].[V{0}]\r\n"
            + "GO\r\n\r\n";

        /// <summary>
        /// 0 : Nom de la table
        /// 1 : type de la procedure (S,I,U)
        /// </summary>
        private const string DROP_PROC =
            "if exists (SELECT * from dbo.sysobjects where id = object_id(N'[dbo].[{0}_{1}]') "
            + "and OBJECTPROPERTY(id, N'IsProcedure') = 1)\r\n"
            + "\tdrop procedure [dbo].[{0}_{1}]\r\n"
            + "GO\r\n\r\n";

        /// <summary>
        /// 0 : Nom de la table auquel on a enleve le premier 'T'.
        /// 1 : Nom de la table.
        /// 2 : Liste de toutes les colonnes.
        /// </summary>
        private const string CREATE_VIEW =
            "CREATE VIEW dbo.V{0}\r\n"
            + "AS\r\n"
            + "SELECT {2}\r\n"
            + "FROM dbo.{1}\r\n"
            + "GO\r\n\r\n";

        /// <summary>
        /// 0 : Nom de la table
        /// 1 : Liste de paramètres
        /// 2 : Liste des champs selectionnes
        /// 3 : Liste des contraintes
        /// </summary>
        private const string CREATE_PROC_SELECT =
            "CREATE proc {0}_S ("
            + "{1}\r\n"
            + ")\r\n"
            + "AS\r\n"
            + "SELECT {2}\r\n"
            + "FROM {0}\r\n"
            + "WHERE {3}\r\n"
            + "GO\r\n\r\n";

        /// <summary>
        /// 0 : Nom de la table
        /// 1 : Liste des paramètres
        /// 2 : Liste des champs selectionnes
        /// 3 : Liste des valeurs
        /// </summary>
        private const string CREATE_PROC_INSERT =
            "CREATE proc {0}_I ("
            + "{1}\r\n"
            + ")\r\n"
            + "AS\r\n"
            + "INSERT INTO {0}("
            + "{2}\r\n"
            + ")\r\n"
            + "VALUES ("
            + "{3}\r\n"
            + ")\r\n"
            + "GO\r\n\r\n";

        /// <summary>
        /// 0 : nom de la table
        /// 1 : liste des paramètres
        /// 2 : liste des champs a mettre a jour
        /// 3 : liste des contraintes
        /// </summary>
        private const string CREATE_PROC_UPDATE =
            "CREATE proc {0}_U ("
            + "{1}\r\n"
            + ")\r\n"
            + "as\r\n"
            + "UPDATE {0}\r\n"
            + "SET"
            + "{2}\r\n"
            + "WHERE"
            + "{3}\r\n"
            + "GO\r\n\r\n";


        /// <summary>
        /// 0 : Nom de la colonne
        /// </summary>
        private const string COLUMN_LIST_SYNTAX =
            "\r\n\t[{0}],";

        /// <summary>
        /// 0 : Nom du parametre
        /// 1 : type du parametre
        /// </summary>
        private const string PARAM_LIST_SYNTAX =
            "\r\n\t@{0} {1},";

        /// <summary>
        /// 0 : Nom de la colonne
        /// 1 : Nom du parametre
        /// </summary>
        private const string WHERE_SYNTAX =
            "\r\n\t[{0}] = @{1} AND";

        /// <summary>
        /// 0 : Nom de la valeur 
        /// </summary>
        private const string VALUES_LIST_SYNTAX =
            "\r\n\t@{0} ,";

        /// <summary>
        /// 0 : Nom du champ a changer
        /// 1 : Nom de la valeur a changer
        /// </summary>
        private const string UPDATE_LIST_SYNTAX =
            "\r\n\t[{0}] = @{1},";

        #endregion

        static void Main(string[] args)
        {
            DBGenerator script = new DBGenerator();

            script._connection = new OleDbConnection(@"Provider=sqloledb;Data Source=localhost;Initial Catalog=;Integrated Security=SSPI;");
            script.CreateAllViews();
            script.CreateAllSP_S();
            script.CreateAllSP_I();
            script.CreateAllSP_U();
            Console.WriteLine("Done");
            Console.Read();
        }

        #region creation des vues
        void CreateAllViews()
        {
            try
            {
                _connection.Open();
                string Tables = ConfigurationManager.AppSettings.Get("allTablesList");
                string[] nomTables = Tables.Split(';');
                using (StreamWriter fichierSql = new StreamWriter("Create_Views.sql"))
                {
                    foreach (string nomTable in nomTables) {CreateView(fichierSql, nomTable);}
                }
            }
            finally { _connection.Close(); }
        }

        void CreateView(StreamWriter fichierSql, string tableName)
        {
            // Récupération des noms de colonnes
            string columns = createColumnList(tableName);

            if (null == columns)
            {
                return;
            }
            // Suppression de la vue si elle existe déja
            fichierSql.Write(string.Format(DROP_VIEW, tableName.Substring(1)));
            // Creation de la vue
            fichierSql.Write(string.Format(CREATE_VIEW, tableName.Substring(1), tableName, columns));
        }
        #endregion

        #region Creation SP_S
        void CreateAllSP_S()
        {
            try
            {
                _connection.Open();
                string Tables = ConfigurationManager.AppSettings.Get("allTablesList");
                string[] nomTables;
                nomTables = Tables.Split(';');
                using (StreamWriter fichierSql = new StreamWriter("Create_SP_S.sql"))
                {
                    foreach (string nomTable in nomTables) { CreateSP_S(fichierSql, nomTable); }
                }
            }
            finally { _connection.Close(); }
        }

        void CreateSP_S(StreamWriter fichierSql, string tableName)
        {
            // Récupération des noms et type des parametres de la procedure
            string parameters = createParametersList(tableName, true);

            // On récupère les noms de toutes les colonnes.
            string columnsList = createColumnList(tableName);

            // On récupère les contraintes du select
            string contraintes = createContraintesList(tableName);
            if (null == contraintes || null == parameters || null == columnsList) { return; }
            fichierSql.Write(string.Format(DROP_PROC, tableName, "S"));
            fichierSql.Write(string.Format(CREATE_PROC_SELECT, new object[] { tableName, parameters, columnsList, contraintes }));

        }
        #endregion

        #region Creation SP_I
        void CreateAllSP_I()
        {
            try
            {
                _connection.Open();
                string Tables = ConfigurationManager.AppSettings.Get("u_iTablesList");
                using (StreamWriter fichierSql = new StreamWriter("Create_SP_I.sql"))
                {
                    string[] nomTables;
                    nomTables = Tables.Split(';');
                    foreach (string nomTable in nomTables)
                    {
                        CreateSP_I(fichierSql, nomTable);
                    }
                }
            }
            finally { _connection.Close(); }
        }

        void CreateSP_I(StreamWriter fichierSql, string tableName)
        {
            string parameters = createParametersList(tableName, false);
            string columns = createColumnList(tableName);
            string values = createValuesList(tableName);

            if (null == parameters || null == columns || null == values)
            {
                return;
            }
            // Creation du script d'insertion:
            fichierSql.Write(string.Format(DROP_PROC, tableName, "I"));
            fichierSql.Write(string.Format(CREATE_PROC_INSERT, tableName, parameters, columns, values));
        }
        #endregion

        #region Creation SP_U
        void CreateAllSP_U()
        {
            try
            {
                _connection.Open();
                string Tables = ConfigurationManager.AppSettings.Get("u_iTablesList");
                string[] nomTables;
                nomTables = Tables.Split(';');
                using (StreamWriter fichierSql = new StreamWriter("Create_SP_U.sql"))
                {
                    foreach (string nomTable in nomTables) {CreateSP_U(fichierSql, nomTable);}
                }
            }
            finally { _connection.Close(); }
        }

        void CreateSP_U(StreamWriter fichierSql, string tableName)
        {
            // il faut récupérer les clefs primaires de la table puis les colonnes.
            string parameters = createParametersList(tableName, false);
            string toUpdate = createUpdateList(tableName);
            string contraintes = createContraintesList(tableName);
            if ((null == contraintes) || (null == toUpdate)) {return;}
            fichierSql.Write(string.Format(DROP_PROC, tableName, "U"));
            fichierSql.Write(string.Format(CREATE_PROC_UPDATE, tableName, parameters, toUpdate, contraintes));
        }
        #endregion

        #region Methodes_Communes
        /// <summary>
        /// </summary>
        /// <param name="tableName">Nom de la table</param>
        /// <returns>string de la forme : [col1], [col2], [col3] </returns>
        string createColumnList(String tableName)
        {
            DataTable table = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName });
            List<string> allColumns = new List<string>();
            foreach (DataRow row in table.Rows) {allColumns.Add(row["column_name"].ToString());}
            if (0 == allColumns.Count) {return null;}
            StringBuilder columnList = new StringBuilder();
            foreach (string columnName in allColumns) {columnList.AppendFormat(COLUMN_LIST_SYNTAX, columnName);}
            return columnList.ToString().Remove(columnList.Length - 1);
        }

        string createUpdateList(string tableName)
        {
            List<string> primaryKeys = new List<string>();
            DataTable table = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, new object[] { null, null, tableName });
            foreach (DataRow row in table.Rows) {primaryKeys.Add(row["column_name"].ToString());}

            table = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName });
            List<string> nonPrimary = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                if (!primaryKeys.Contains(row["column_name"].ToString()))
                {
                    nonPrimary.Add(row["column_name"].ToString());
                }
            }
            if (0 == nonPrimary.Count) { return null; }
            StringBuilder updateList = new StringBuilder();
            foreach (string columnName in nonPrimary)
            {
                updateList.AppendFormat(UPDATE_LIST_SYNTAX, columnName, columnName.Replace("-", "_"));
            }
            return updateList.ToString().Remove(updateList.Length - 1);
        }

        string createParametersList(String tableName, bool primaryKeysOnly)
        {
            List<string> parametersName = new List<string>();
            List<string> parametersType = new List<string>();
            DataTable table = null;
			table = _connection.GetOleDbSchemaTable(
				(primaryKeysOnly) ? OleDbSchemaGuid.Primary_Keys : OleDbSchemaGuid.Columns,
				new object[] { null, null, tableName });
            foreach (DataRow row in table.Rows) {parametersName.Add(row["column_name"].ToString());}
            // Pour récupérer le type des champs
            string queryString = "select column_name, data_type, character_maximum_length from INFORMATION_SCHEMA.columns where table_name = '" + tableName + "'";
            OleDbDataAdapter da = new OleDbDataAdapter(queryString, _connection);
            DataSet ds = new DataSet();
            da.Fill(ds);
            table = ds.Tables[0];

            foreach (DataRow row in table.Rows)
            {
                if (parametersName.Contains(row["column_name"].ToString()))
                {
                    string parameterType = row["data_type"].ToString();
                    int length = 0;
                    try { length = (int)row["character_maximum_length"]; }
                    catch (Exception) { }
                    if (0 < length && length < 8000)// maximum length is 8000
                    {
                        parameterType += "(" + length + ")";
                    }
                    parametersType.Add(parameterType);
                }
            }
            if (0 == parametersName.Count) { return null; }
            StringBuilder parameters = new StringBuilder();
            for (int i = 0; i < parametersName.Count; i++)
            {
                parameters.AppendFormat(PARAM_LIST_SYNTAX, parametersName[i].Replace("-", "_"), parametersType[i]);
            }
            return parameters.ToString().Remove(parameters.Length - 1);
        }

        string createContraintesList(String tableName)
        {
            List<string> contraintesName = new List<string>();
            DataTable table = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, new object[] { null, null, tableName });
            foreach (DataRow row in table.Rows)
            {
                contraintesName.Add(row["column_name"].ToString());
            }
            if (0 == contraintesName.Count) { return null; }
            StringBuilder contraintes = new StringBuilder();
            foreach (string contrainte in contraintesName)
            {
                contraintes.AppendFormat(WHERE_SYNTAX, contrainte, contrainte.Replace("-", "_"));
            }
            return contraintes.ToString().Remove(contraintes.Length - 3);
        }

        private string createValuesList(string tableName)
        {
            DataTable table = _connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, new object[] { null, null, tableName });
            List<string> valuesList = new List<string>();
            foreach (DataRow row in table.Rows) {valuesList.Add(row["column_name"].ToString());}
            if (0 == valuesList.Count) {return null;}
            StringBuilder values = new StringBuilder();
            foreach (string value in valuesList)
            {
                values.AppendFormat(VALUES_LIST_SYNTAX, value.Replace("-", "_"));
            }
            return values.ToString().Remove(values.Length - 1);
        }
        #endregion
    }
}
