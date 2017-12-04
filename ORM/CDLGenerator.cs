using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;
using System.Text.RegularExpressions;

namespace com.castsoftware.tools
{
    public class CDLGenerator
    {

        internal CDLGenerator(Table table)
        {
            _table = table;
            return;
        }

        internal string Generate()
        {
            try
            {
                if (_table.Obsolete)
                { return string.Empty; }

                StringBuilder attributs = new StringBuilder();
                StringBuilder clonage = new StringBuilder();
                StringBuilder proprietes = new StringBuilder();
                StringBuilder equals = new StringBuilder();
                string className = "VO_" + _table.ClassName;

                foreach (Column currentColumn in _table.Columns)
                {
                    string type = getType(currentColumn);
                    string attributeName = "m" + currentColumn.AttributeName;//.Remove( 1 ).ToLower() + currentColumn.AttributeName.Substring( 1 );
                    string get = string.Format(GET, attributeName);
                    string set = string.Format(SET, attributeName);

                    attributs.AppendFormat(ATTRIBUTE, attributeName, type,
                        formatComment(currentColumn.Description, 2));

                    clonage.AppendFormat("\t\t\tlResult.{0} = {0};\r\n", attributeName);

                    proprietes.AppendFormat(PROPERTY, currentColumn.AttributeName, type, get + set,
                        cleanUpName(attributeName), formatComment(currentColumn.Description, 2));
                    equals.AppendFormat("\r\n\t\t\t\t(this.{0} == pObject.{0}) &&", currentColumn.AttributeName);
                }
                string pouet = equals.ToString().TrimEnd('&');
                return string.Format(CLASS_CONTENT, className, proprietes.ToString(),
                    attributs.ToString(), formatComment(_table.Description, 1),
                    clonage.ToString(),pouet);
            }
            catch (Exception) { throw; }// return string.Empty; }
        }

        private static string formatComment(string pDescription, int p_nbTab)
        {
            string p_comment = pDescription.Replace("\r\n", " ");
            StringBuilder sbComment = new StringBuilder(p_comment);
            int beginIndex = 0;
            int count = 87;
            string l_insert = "\r\n";

            for (int l_num = 0; l_num < p_nbTab; l_num++)
            {
                l_insert += '\t';
            }
            l_insert += "///";


            while (true)
            {
                bool doSplit = true;

                int splitIndex = beginIndex + count;
                if (splitIndex >= sbComment.Length) { break; }

                while (!Char.IsSeparator(sbComment[splitIndex]))
                {
                    if (splitIndex <= beginIndex)
                    {
                        splitIndex = beginIndex;
                        break;
                    }
                    if (sbComment[splitIndex] == '\n')
                    {
                        doSplit = false;
                        break;
                    }
                    splitIndex--;
                }
                if (doSplit)
                {
                    sbComment.Insert(splitIndex, l_insert);
                    beginIndex = splitIndex + 2;
                }
                else { beginIndex = splitIndex + 1; }
            }

            return sbComment.ToString();
        }

        private static string getType(Column column)
        {
            switch (column.SqlType)
            {
                case "binary":
                case "varbinary":
                case "image":
                    return "byte[]";
                case "int":
                case "smallint":
                case "tinyint":
                    return (column.Nullable) ? "int?" : "int";
                case "bigint":
                    return "Int64";
                case "char":
                    if (column.SqlLength == 11)
                    {
                        return (column.Nullable) ? "TimeCode?" : "TimeCode";
                    }
                    else
                    {
                        return "string";
                    }
                case "varchar":
                case "nvarchar":
                case "nchar":
                case "text":
                case "ntext":
                    return "string";
                case "datetime":
                    return (column.Nullable) ? "DateTime?" : "DateTime";
                case "decimal":
                    return (column.Nullable) ? "decimal?" : "decimal";
                case "bit":
                    return (column.Nullable) ? "bool?" : "bool";
                case "float":
                    return (column.Nullable) ? "float?" : "float";
                case "numeric":
                    return (column.Nullable) ? "decimal?" : "decimal";
                default:
                    return "UNDEFINED TYPE";
            }
        }

        /// <summary>
        /// Nettoie le nom 
        /// ex : "ListeTypeDocument" devient "liste type document"
        /// </summary>
        /// <param name="name">nom a netoyer</param>
        /// <returns>nom netoyé</returns>
        private static string cleanUpName(string name)
        {
            string cleanName = "";
            Match nameMatch = Regex.Match(name, REGEXP_CLEANUP_NAME);
            while (nameMatch.Success)
            {
                Group group = nameMatch.Groups[0];
                if (group.Value != "m")
                {
                   cleanName += group.Value + " ";
                }
                nameMatch = nameMatch.NextMatch();
        }
            cleanName = cleanName.Trim();
            cleanName = cleanName.Replace(" ", "_");
            cleanName = cleanName.ToUpper();
            return cleanName;
        }

        #region ATTRIBUTES
        //private const string CDL_FILENAME = @"c:\temp\CDL\{0}.cs";

        private const string CLASS_CONTENT =
            "// Ce fichier a été généré automatiquement. Veuillez ne pas modifier son contenu.\r\n"
            + "// Les ajouts eventuels sont a mettre dans un autre fichier.\r\n"
            + "using System;\r\n"
            + "using System.Collections.Generic;\r\n"
            + "using System.Text;\r\n"
            + "using System.Collections;\r\n"
            + "using System.Xml.Serialization;\r\n"
            + "using System.Runtime.Serialization;\r\n\r\n"
            + "using ORM.Transats.Framework.Structures_Donnees;\r\n"
            + "\r\n"
            + "namespace ORM.Transats.ValueObjects\r\n"
            + "{{\r\n"
            + "\t/// <summary>\r\n"
            + "\t/// Classe {0}\r\n"
            + "\t/// {3}\r\n"
            + "\t/// </summary>\r\n"
            + "\t[DataContract]\r\n"
            + "\tpublic partial class {0} : ICloneable\r\n"
            + "\t{{\r\n"
            + "\t\t#region Constructeurs\r\n"
            + "\t\t/// <summary>\r\n"
            + "\t\t/// Constructeur vide\r\n"
            + "\t\t/// </summary>\r\n"
            + "\t\tpublic {0}()\r\n"
            + "\t\t{{\r\n"
            + "\t\t\treturn;\r\n"
            + "\t\t}}\r\n"
            + "\t\t#endregion\r\n\r\n"
            + "\t\t#region Implémentation ICloneable\r\n"
            + "\t\t/// <summary>\r\n"
            + "\t\t/// Clonage d'une instance de la classe\r\n"
            + "\t\t/// </summary>\r\n"
            + "\t\t/// <returns>Objet cloné</returns>\r\n"
            + "\t\tpublic object Clone()\r\n"
            + "\t\t{{\r\n"
            + "\t\t\t{0} lResult = new {0}();\r\n"
            + "\t\t\t\r\n"
            + "{4}"
            + "\t\t\treturn lResult;\r\n"
            + "\t\t}}\r\n"
            + "\t\t#endregion\r\n\r\n"
            + "\t\t#region Methodes\r\n"
            + "\t\tpublic bool Equals({0} pObject)\r\n"
            + "\t\t{{\r\n"
            + "\t\t\treturn {5};\r\n"            
            + "\t\t}}\r\n"
            + "\t\t#endregion\r\n\r\n"
            + "\t\t#region Propriétés\r\n"
            + "{1}\r\n"
            + "\t\t#endregion\r\n\r\n"
            + "\t\t#region Attributs\r\n"
            + "{2}\r\n"
            + "\t\t#endregion\r\n"
            + "\t}}\r\n"
            + "}}";

        // 0 : Nom de la propriete
        // 1 : Type de l'attribut
        // 2 : Contenu de la propriete
        // 3 : Nom de la propriete enjolive
        // 4 : Description tiree du document xml
        private const string PROPERTY =
            "\t\t/// <summary>\r\n"
            + "\t\t/// Récupère ou modifie le {3}\r\n"
            + "\t\t/// {4}\r\n"
            + "\t\t/// </summary>\r\n"
           // + "\t\t[DataMember]\r\n"
            + "\t\tpublic {1} {0}\r\n"
            + "\t\t{{\r\n"
            + "{2}"
            + "\t\t}}\r\n\r\n";

        // 0 : Nom de l'attribut
        private const string GET =
            "\t\t\tget {{ return {0}; }}\r\n";

        // 0 : Nom de l'attribut
        private const string SET =
            "\t\t\tset {{ {0} = value; }}\r\n";

        // 0 : Nom de l'attribut
        // 1 : Type de l'attribut
        // 2 : Description tiree du fichier XML
        private const string ATTRIBUTE =
              "\t\t/// <summary>\r\n"
            + "\t\t/// {2}\r\n"
            + "\t\t/// </summary>\r\n"
            + "\t\tprivate {1} {0};\r\n";

        private const string REGEXP_CLEANUP_NAME =
            "[A-Z][a-z][a-z]*|[A-Z][A-Z]*|[a-z]*";

        private Table _table;

        # endregion
    }
}
