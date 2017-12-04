using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace com.castsoftware.tools
{
    class DALGenerator
    {

        internal DALGenerator(Table table)
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

                string className = _table.ClassName;
                string l_classParamName = className;//.Remove( 1 ).ToLower() + className.Substring( 1 );
                StringBuilder methodsContent = new StringBuilder();

                List<Column> primaryKeyColumnsList = new List<Column>();
                List<Column> nonPrimaryKeyColumnsList = new List<Column>();
                List<Column> identityColumnsList = new List<Column>();
                List<Column> nonIdentityColumnsList = new List<Column>();
                List<Column> allColumnsList = new List<Column>();
                foreach (Column currentColumn in _table.Columns)
                {
                    if (currentColumn.PrimaryKeyMember)
                    { primaryKeyColumnsList.Add(currentColumn); }
                    else
                    { nonPrimaryKeyColumnsList.Add(currentColumn); }

                    if (currentColumn.IdentityColumn)
                    { identityColumnsList.Add(currentColumn); }
                    else
                    { nonIdentityColumnsList.Add(currentColumn); }

                    allColumnsList.Add(currentColumn);
                }

                StringBuilder parametersComment = new StringBuilder();
                StringBuilder methodParameters = new StringBuilder();
                StringBuilder psPKParameters = new StringBuilder();
                StringBuilder psSelectParameters = new StringBuilder();
                StringBuilder psParameters = new StringBuilder();
                StringBuilder attributesInitialisation = new StringBuilder();
                StringBuilder psNonPKParameters = new StringBuilder();
                StringBuilder psOutputParameters = new StringBuilder();
                StringBuilder identityAttributesInitialisation = new StringBuilder();
                StringBuilder psNonIdentityParameters = new StringBuilder();

                foreach (Column currentColumn in primaryKeyColumnsList)
                {
                    if (methodParameters.Length > 1)
                    { methodParameters.Append(", "); }
                    methodParameters.AppendFormat
                        ("{0} p{1}"
                        , getType(currentColumn)
                        , currentColumn.AttributeName//.Remove( 1 ).ToLower() + currentColumn.AttributeName.Substring( 1 )
                        );
                    parametersComment.AppendFormat
                        (PARAMETER_COMMENT_TEMPLATE
                        , currentColumn.AttributeName//.Remove( 1 ).ToLower() + currentColumn.AttributeName.Substring( 1 )
                        , formatComment(currentColumn.Description, false)
                        );
                    psPKParameters.AppendFormat
                        (PS_PARAMETERS_TEMPLATE
                        , currentColumn.Name.Replace("-", "_")
                        , currentColumn.AttributeName//.Remove( 1 ).ToLower() + currentColumn.AttributeName.Substring( 1 )
                        );
                    psSelectParameters.AppendFormat
                        (PS_PARAMETERS_SELECT_TEMPLATE
                        , currentColumn.Name.Replace("-", "_")
                        , currentColumn.AttributeName//.Remove( 1 ).ToLower() + currentColumn.AttributeName.Substring( 1 )
                        );
                    psOutputParameters.AppendFormat(PS_OUTPUT_PARAMETERS_TEMPLATE, currentColumn.AttributeName,
                        currentColumn.Name.Replace("-", "_"));
                    if (currentColumn.Nullable)
                    {
                        identityAttributesInitialisation.AppendFormat
                            (IDENTITY_ATTRIBUTE_INIT_TEMPLATE
                            , l_classParamName
                            , currentColumn.AttributeName
                            , currentColumn.Name.Replace("-", "_")
                            );
                    }
                    else
                    {
                        identityAttributesInitialisation.AppendFormat
                            (NULLABLE_IDENTITY_ATTRIBUTE_INIT_TEMPLATE
                            , l_classParamName
                            , currentColumn.AttributeName
                            , currentColumn.Name.Replace("-", "_")
                            );
                    }
                }
                {
                    int i = 0;
                    foreach (Column currentColumn in allColumnsList)
                    {
                        attributesInitialisation.AppendFormat
                            (ATTRIBUTE_INIT_TEMPLATE
                            , currentColumn.AttributeName
                            , getReaderCommand(currentColumn, i++)
                            );
                        psParameters.AppendFormat
                            (currentColumn.Nullable
                              ? (getType(currentColumn).Contains("?")
                                    ? (getType(currentColumn).Contains("TimeCode"))
                                        ? PS_TIMECODE_NULLABLE_SPECIAL_PARAMETER_TEMPLATE
                                        : PS_NULLABLE_SPECIAL_PARAMETERS_TEMPLATE
                                    : PS_NULLABLE_PARAMETERS_TEMPLATE
                                )
                              : (getType(currentColumn).Contains("TimeCode"))
                                    ? PS_TIMECODE_PARAMETERS_TEMPLATE
                                    : PS_PARAMETERS_TEMPLATE
                            , currentColumn.Name.Replace("-", "_")
                            , l_classParamName + "." + currentColumn.AttributeName
                            );
                    }
                }

                foreach (Column currentColumn in nonPrimaryKeyColumnsList)
                {
                    psNonPKParameters.AppendFormat
                        (currentColumn.Nullable
                            ? (getType(currentColumn).Contains("?")
                                    ? (getType(currentColumn).Contains("TimeCode"))
                                        ? PS_TIMECODE_NULLABLE_SPECIAL_PARAMETER_TEMPLATE
                                        : PS_NULLABLE_SPECIAL_PARAMETERS_TEMPLATE
                                    : PS_NULLABLE_PARAMETERS_TEMPLATE
                              )
                              : (getType(currentColumn).Contains("TimeCode"))
                                    ? PS_TIMECODE_PARAMETERS_TEMPLATE
                                    : PS_PARAMETERS_TEMPLATE
                        , currentColumn.Name.Replace("-", "_")
                        , l_classParamName + "." + currentColumn.AttributeName
                        );
                }
                foreach (Column currentColumn in nonIdentityColumnsList)
                {
                    psNonIdentityParameters.AppendFormat
                        (currentColumn.Nullable
                            ? (getType(currentColumn).Contains("?")
                                    ? (getType(currentColumn).Contains("TimeCode"))
                                        ? PS_TIMECODE_NULLABLE_SPECIAL_PARAMETER_TEMPLATE
                                        : PS_NULLABLE_SPECIAL_PARAMETERS_TEMPLATE
                                    : PS_NULLABLE_PARAMETERS_TEMPLATE

                              )
                              : (getType(currentColumn).Contains("TimeCode"))
                                    ? PS_TIMECODE_PARAMETERS_TEMPLATE
                                    : PS_PARAMETERS_TEMPLATE
                        , currentColumn.Name.Replace("-", "_")
                        , l_classParamName + "." + currentColumn.AttributeName
                        );
                }

                if (primaryKeyColumnsList.Count > 0)
                {
                    methodsContent.AppendFormat
                        (SELECT_IDENTITY_METHOD_TEMPLATE
                        , className
                        , parametersComment.ToString()
                        , methodParameters.ToString()
                        , psSelectParameters.ToString()
                        , attributesInitialisation.ToString()
                        , _table.ViewName
                        , cleanUpName(className)
                        );
                }

                if (_table.HasCode)
                {
                    Column codeColumn = _table.CodeColumn;
                    string columnParameter = String.Format
                        ("{0} p{1}"
                        , getType(codeColumn)
                        , codeColumn.AttributeName//.Remove( 1 ).ToLower() + codeColumn.AttributeName.Substring( 1 )
                        );
                    psSelectParameters.Length = 0;

                    psSelectParameters.AppendFormat
                        (PS_PARAMETERS_SELECT_TEMPLATE
                        , codeColumn.Name.Replace("-", "_")
                        , codeColumn.AttributeName//.Remove( 1 ).ToLower() + codeColumn.AttributeName.Substring( 1 )
                        );

                    methodsContent.AppendFormat
                        (SELECT_CODE_METHOD_TEMPLATE
                        , className
                        , parametersComment.ToString()
                        , columnParameter.ToString()
                        , psSelectParameters.ToString()
                        , attributesInitialisation.ToString()
                        , _table.ViewName
                        , cleanUpName(className)
                        , className + "_BY_" + codeColumn.AttributeName
                        );
                }

                methodsContent.AppendFormat
                    (INSTANTIATE_OBJECT_TEMPLATE
                    , className
                    , attributesInitialisation.ToString()
                    , cleanUpName(className)
                    );

                methodsContent.AppendFormat
                    (SELECT_LIST_METHOD_TEMPLATE
                    , className
                    , className
                    , cleanUpName(className)
                    , _table.ViewName
                    );

                if (_table.HasIdentity)
                {
                    methodsContent.AppendFormat
                        (INSERT_METHOD_WITH_IDENTITY_TEMPLATE
                        , className
                        , _table.ViewName
                        , psNonIdentityParameters.ToString()
                        , _table.GetIdentityColumnName()
                        , cleanUpName(className)
                        , l_classParamName
                        );
                }
                else
                {
                    methodsContent.AppendFormat
                        (INSERT_METHOD_TEMPLATE
                        , className
                        , _table.ViewName
                        , psNonIdentityParameters.ToString()
                        , cleanUpName(className)
                        , l_classParamName
                        );
                }
                if (primaryKeyColumnsList.Count > 0 && nonPrimaryKeyColumnsList.Count > 0)
                {
                    methodsContent.AppendFormat
                        (UPDATE_METHOD_TEMPLATE
                        , className
                        , _table.ViewName
                        , psParameters.ToString()
                        , cleanUpName(className)
                        , l_classParamName
                        );
                }

                if (primaryKeyColumnsList.Count > 0)
                {
                    methodsContent.AppendFormat
                        (DELETE_METHOD_TEMPLATE
                        , className
                        , _table.ViewName
                        , methodParameters.ToString()
                        , psPKParameters.ToString()
                        , parametersComment.ToString()
                        , cleanUpName(className)
                        );
                }

                return string.Format(CLASS_CONTENT, className, methodsContent.ToString(), formatComment(_table.Description, true));
            }
            catch (Exception) { throw; }// return string.Empty; }
        }

        private static string formatComment(string Comment, bool isClassComment)
        {
            StringBuilder sbComment = new StringBuilder(Comment);
            int beginIndex = 0;
            int count = 87;
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
                    if (isClassComment) { sbComment.Insert(splitIndex, "\r\n\t///"); }
                    else { sbComment.Insert(splitIndex, "\r\n\t\t///"); }
                    beginIndex = splitIndex + 2;
                }
                else { beginIndex = splitIndex + 1; }
            }
            return sbComment.ToString();
        }

        private static string getReaderCommand(Column column, int position)
        {
            switch (column.SqlType)
            {
                case "binary":
                case "varbinary":
                case "image":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_READER_VALUE_TEMPLATE, position, "SqlBytes") :
                        string.Format(READER_VALUE_TEMPLATE, position, "SqlBytes");
                case "int":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_SPECIALTYPE_READER_TEMPLATE, position, "Int32", "int") :
                        string.Format(READER_TEMPLATE, position, "Int32", "int");
                case "bigint":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_SPECIALTYPE_READER_TEMPLATE, position, "Int64", "bigint") :
                        string.Format(READER_TEMPLATE, position, "Int64", "bigint");
                case "smallint":
                    return (column.Nullable) ? 
                        string.Format(NULLABLE_SPECIALTYPE_READER_TEMPLATE, position, "Int16", "int") :
                        string.Format(READER_TEMPLATE, position, "Int16", "int");
                case "tinyint":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_SPECIALTYPE_READER_TEMPLATE, position, "Byte", "byte") :
                        string.Format(READER_TEMPLATE, position, "Byte", "byte");
                case "char":
                    if (column.SqlLength == 11)
                    {
                        return (column.Nullable) ?
                            string.Format(TIMECODE_NULLABLE_READER_TEMPLATE, position, "String") :
                            string.Format(TIMECODE_READER_VALUE_TEMPLATE, position, "String");
                            
                    }
                    else
                    {
                        return (column.Nullable) ?
                            string.Format(NULLABLE_READER_TEMPLATE, position, "String") + ".Trim()" :
                            string.Format(READER_TEMPLATE, position, "String") + ".Trim()";
                    }
                case "varchar":
                case "nvarchar":
                case "nchar":
                case "text":
                case "ntext":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_READER_TEMPLATE, position, "String") + ".Trim()" :
                        string.Format(READER_TEMPLATE, position, "String") + ".Trim()";
                case "datetime":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_SPECIALTYPE_READER_TEMPLATE, position, "DateTime", "DateTime") :
                        string.Format(READER_TEMPLATE, position, "DateTime", "DateTime");
                case "numeric":
                case "decimal":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_SPECIALTYPE_READER_TEMPLATE, position, "Decimal", "decimal") :
                        string.Format(READER_TEMPLATE, position, "Decimal", "decimal");
                case "V_OUI_NON":
                case "bit":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_SPECIALTYPE_READER_TEMPLATE, position, "Boolean", "bool") :
                        string.Format(READER_TEMPLATE, position, "Boolean");
                case "float":
                    return (column.Nullable) ?
                        string.Format(NULLABLE_SPECIALTYPE_READER_TEMPLATE, position, "Float", "float") :
                        string.Format(READER_TEMPLATE, position, "Float");
                default:
                    return "UNDEFINED TYPE";
            }
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
                case "text":
                case "ntext":
                case "nvarchar":
                case "nchar":
                    return "string";
                case "datetime":
                    return (column.Nullable) ? "DateTime?" : "DateTime";
                case "decimal":
                    return (column.Nullable) ? "decimal?" : "decimal";
                case "V_OUI_NON":
                case "bit":
                    return (column.Nullable) ? "bool?" : "bool";
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
        private static string cleanUpName(string p_name)
        {
            string cleanName = "";
            Match nameMatch = Regex.Match(p_name, REGEXP_CLEANUP_NAME);
            while (nameMatch.Success)
            {
                Group group = nameMatch.Groups[0];
                cleanName += group.Value + " ";
                nameMatch = nameMatch.NextMatch();
            }
            return cleanName.Remove(cleanName.Length - 1).ToLower();
        }

        #region ATTRIBUTES

        //private const string DAL_FILENAME = @"c:\temp\DAL\{0}_auto.cs";

        private const string CLASS_CONTENT =
            "// Ce fichier a été généré automatiquement. Veuillez ne pas modifier son contenu.\r\n"
            + "// Les ajouts eventuels sont a mettre dans le fichier DA_{0}.cs.\r\n"
            + "using System;\r\n"
            + "using System.Collections.Generic;\r\n"
            + "using System.Data;\r\n"
            + "using System.Data.SqlClient;\r\n"
            + "using System.Reflection;\r\n"
            + "using System.ServiceModel;\r\n"
            + "using System.Transactions;\r\n"
            + "\r\n"
            + "using ORM.Transats.ValueObjects;\r\n"
            + "\r\n"
            + "using ORM.Transats.Framework.Constantes;\r\n"
            + "using ORM.Transats.Framework.Exceptions;\r\n"
            + "using ORM.Transats.Framework.Journalisation;\r\n"
            + "using ORM.Transats.Framework.Reflection;\r\n"
            + "using ORM.Transats.Framework.Structures_Donnees;\r\n"
            + "using ORM.Transats.Framework.Transactions;\r\n"
            + "\r\n\r\n"
            + "namespace ORM.Transats.DataAccess\r\n"
            + "{{\r\n"
            + "\t/// <summary>\r\n"
            + "\t/// La classe ne contient que des méthodes statiques.\r\n"
            + "\t/// {2}\r\n"
            + "\t/// </summary>\r\n"
            + "\tpublic static partial class DA_{0}\r\n"
            + "\t{{\r\n"
            + "\t\t#region Méthodes\r\n"
            + "{1}"
            + "\t\t#endregion\r\n"
            + "\t}}\r\n"
            + "}}\r\n";

        private const string READER_TEMPLATE =
            "pReader.Get{1}({0})";

        private const string READER_VALUE_TEMPLATE =
            "pReader.Get{1}({0}).Value";

        private const string NULLABLE_READER_TEMPLATE =
            "pReader.IsDBNull({0}) ? null : pReader.Get{1}({0})";

        private const string NULLABLE_READER_VALUE_TEMPLATE =
            "pReader.IsDBNull({0}) ? null : pReader.Get{1}({0}).Value";

        private const string NULLABLE_SPECIALTYPE_READER_TEMPLATE =
            "pReader.IsDBNull({0}) ? null : new {2}?(pReader.Get{1}({0}))";

        private const string TIMECODE_READER_VALUE_TEMPLATE =
            "TimeCode.ParseDBValue(pReader.Get{1}({0})).Value";

        private const string TIMECODE_NULLABLE_READER_TEMPLATE =
            "pReader.IsDBNull({0}) ? null : TimeCode.ParseDBValue(pReader.Get{1}({0}))";

        private const string ATTRIBUTE_INIT_TEMPLATE =
            "\t\t\tvResult.{0} = {1};\r\n";

        private const string IDENTITY_ATTRIBUTE_INIT_TEMPLATE =
            "\t\t\t\t\tp{0}.{1} = Convert.ToInt32(vCommand.Parameters[\"@@{2}\"].Value);\r\n";

        private const string NULLABLE_IDENTITY_ATTRIBUTE_INIT_TEMPLATE =
            "\t\t\t\t\tp{0}.{1} = new int?(Convert.ToInt32(vCommand.Parameters[\"@@{2}\"].Value));\r\n";

        private const string PARAMETER_COMMENT_TEMPLATE =
            "\t\t/// <param name=\"p{0}\">\r\n"
            + "\t\t/// {1}\r\n"
            + "\t\t/// </param>\r\n";

        private const string PS_TIMECODE_PARAMETERS_TEMPLATE =
            "\t\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", p{1}.ToString()));\r\n";

        private const string PS_PARAMETERS_TEMPLATE =
            "\t\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", p{1}));\r\n";

        private const string PS_NULLABLE_PARAMETERS_TEMPLATE =
            "\t\t\t\t\t\t" + "if (p{1} != null)\r\n"
            + "\t\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", p{1}));\r\n"
            + "\t\t\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t\t" + "else\r\n"
            + "\t\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", DBNull.Value));\r\n"
            + "\t\t\t\t\t\t" + "}}\r\n";

        private const string PS_NULLABLE_SPECIAL_PARAMETERS_TEMPLATE =
            "\t\t\t\t\t\t" + "if (p{1}.HasValue)\r\n"
            + "\t\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", p{1}.Value));\r\n"
            + "\t\t\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t\t" + "else\r\n"
            + "\t\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", DBNull.Value));\r\n"
            + "\t\t\t\t\t\t" + "}}\r\n";

        private const string PS_TIMECODE_NULLABLE_SPECIAL_PARAMETER_TEMPLATE =
            "\t\t\t\t\t\t" + "if (p{1}.HasValue)\r\n"
            + "\t\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", p{1}.ToString()));\r\n"
            + "\t\t\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t\t" + "else\r\n"
            + "\t\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", DBNull.Value));\r\n"
            + "\t\t\t\t\t\t" + "}}\r\n";


        private const string PS_PARAMETERS_SELECT_TEMPLATE =
           "\t\t\t\t\t" + "vCommand.Parameters.Add(new SqlParameter(\"@{0}\", p{1}));\r\n";

        private const string PS_OUTPUT_PARAMETERS_TEMPLATE =
             "\t\t\t\t\t\t" + "SqlParameter lSqlParam{0} = new SqlParameter(\"@@{1}\", SqlDbType.Int, 4);\r\n"
             + "\t\t\t\t\t\t" + "vSqlParam{0}.Direction = ParameterDirection.ReturnValue;\r\n"
             + "\t\t\t\t\t\t" + "vCommand.Parameters.Add(vSqlParam{0});\r\n";

        private const string SELECT_IDENTITY_METHOD_TEMPLATE =
            "\t\t" + "/// <summary>\r\n"
            + "\t\t" + "/// Charge les informations d'un {6}\r\n"
            + "\t\t" + "/// </summary>\r\n"
            + "{1}"
            + "\t\t" + "/// <returns>\r\n"
            + "\t\t" + "/// Une instance de {6} récupéré dans la base à partir de sa clef primaire.\r\n"
            + "\t\t" + "/// Si aucune instance de {6} n'a été trouvée, la fonction renvoie une référence null.\r\n"
            + "\t\t" + "/// </returns>\r\n"
            + "\t\t" + "public static VO_{0} SGet{0}({2})\r\n"
            + "\t\t" + "{{\r\n"
            + "\t\t\t" + "using (SqlConnection vConnection = TFournisseurConnexion.RetourneConnexion())\r\n"
            + "\t\t\t" + "{{\r\n"
            + "\t\t\t\t" + "try\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "SqlCommand vCommand = new SqlCommand(\"PS_GET_{0}_auto\", vConnection);\r\n"
            + "\t\t\t\t\t" + "vCommand.CommandType = CommandType.StoredProcedure;\r\n"
            + "{3}"
            + "\t\t\t\t\t" + "return SInstanciate{0}FromDataBase(vCommand.ExecuteReader());\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "#region Exceptions : DAL\r\n"
            + "\t\t\t\t" + "catch (SqlException vExp)\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "// Création d'une exception personnalisée\r\n"
            + "\t\t\t\t\t" + "TExceptionTransats vExcepTransats = new TExceptionTransats(\r\n"
            + "\t\t\t\t\t\t\t\t" + "new TExceptionSource(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameFormattedAssembly(Assembly.GetExecutingAssembly()),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameClassExecuting(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameMethodExecuting()),\r\n"
            + "\t\t\t\t\t\t\t\t" + "EnumTypeException.Technical,\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.ErrorCode.ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.Message,\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.GetType().ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.StackTrace);\r\n"
            + "\t\t\t\t\t" + "// Envoi de l'exception\r\n"
            + "\t\t\t\t\t" + "throw new FaultException<TExceptionTransats>(vExcepTransats, new FaultReason(vExcepTransats.Source.Description));\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "#endregion\r\n"
            + "\t\t\t" + "}}\r\n"
            + "\t\t" + "}}\r\n\r\n";

        private const string SELECT_CODE_METHOD_TEMPLATE =
            "\t\t" + "/// <summary>\r\n"
            + "\t\t" + "/// Charge les informations d'un {6}\r\n"
            + "\t\t" + "/// </summary>\r\n"
            + "{1}"
            + "\t\t" + "/// <returns>\r\n"
            + "\t\t" + "/// Une instance de {6} récupéré dans la base à partir de son code.\r\n"
            + "\t\t" + "/// Si aucune instance de {6} n'a été trouvée, la fonction renvoie une référence null.\r\n"
            + "\t\t" + "/// </returns>\r\n"
            + "\t\t" + "public static VO_{0} SGet{0}({2})\r\n"
            + "\t\t" + "{{\r\n"
            + "\t\t\t" + "using (SqlConnection vConnection = TFournisseurConnexion.RetourneConnexion())\r\n"
            + "\t\t\t" + "{{\r\n"
            + "\t\t\t\t" + "try\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "SqlCommand vCommand = new SqlCommand(\"PS_GET_{7}_auto\", vConnection);\r\n"
            + "\t\t\t\t\t" + "vCommand.CommandType = CommandType.StoredProcedure;\r\n"
            + "{3}"
            + "\t\t\t\t\t" + "return SInstanciate{0}FromDataBase(vCommand.ExecuteReader());\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "#region Exceptions : DAL\r\n"
            + "\t\t\t\t" + "catch (SqlException vExp)\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "// Création d'une exception personnalisée\r\n"
            + "\t\t\t\t\t" + "TExceptionTransats vExcepTransats = new TExceptionTransats(\r\n"
            + "\t\t\t\t\t\t\t\t" + "new TExceptionSource(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameFormattedAssembly(Assembly.GetExecutingAssembly()),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameClassExecuting(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameMethodExecuting()),\r\n"
            + "\t\t\t\t\t\t\t\t" + "EnumTypeException.Technical,\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.ErrorCode.ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.Message,\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.GetType().ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.StackTrace);\r\n"
            + "\t\t\t\t\t" + "// Envoi de l'exception\r\n"
            + "\t\t\t\t\t" + "throw new FaultException<TExceptionTransats>(vExcepTransats, new FaultReason(vExcepTransats.Source.Description));\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "#endregion\r\n"
            + "\t\t\t" + "}}\r\n"
            + "\t\t" + "}}\r\n\r\n";

        private const string SELECT_LIST_METHOD_TEMPLATE =
            "\t\t" + "/// <summary>\r\n"
            + "\t\t" + "/// Charge tous les {2} présents dans la base.\r\n"
            + "\t\t" + "/// </summary>\r\n"
            + "\t\t" + "/// <returns>\r\n"
            + "\t\t" + "/// La liste de tous les {2} presents dans la base.\r\n"
            + "\t\t" + "/// Si aucune instance de {2} n'a été trouvée, la fonction renvoie un tableau \r\n"
            + "\t\t" + "/// de taille zero.\r\n"
            + "\t\t" + "/// </returns>\r\n"
            + "\t\t" + "public static VO_{0}[] SGetAll{0}s()\r\n"
            + "\t\t" + "{{\r\n"
            + "\t\t\t" + "using (SqlConnection vConnection = TFournisseurConnexion.RetourneConnexion())\r\n"
            + "\t\t\t" + "{{\r\n"
            + "\t\t\t\t" + "List<VO_{0}> v{0}sList = new List<VO_{0}>();\r\n"
            + "\t\t\t\t" + "try\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "SqlCommand vCommand = new SqlCommand(\"PS_GET_ALL_{0}s_auto\", vConnection);\r\n"
            + "\t\t\t\t\t" + "vCommand.CommandType = CommandType.StoredProcedure;\r\n"
            + "\t\t\t\t\t" + "SqlDataReader vReader = vCommand.ExecuteReader();\r\n"
            + "\t\t\t\t\t" + "VO_{0} v{1};\r\n"
            + "\t\t\t\t\t" + "while ((v{1} = SInstanciate{0}FromDataBase(vReader)) != null)\r\n"
            + "\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t" + "v{0}sList.Add(v{1});\r\n"
            + "\t\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "#region Exceptions : DAL\r\n"
            + "\t\t\t\t" + "catch (SqlException vExp)\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "// Création d'une exception personnalisée\r\n"
            + "\t\t\t\t\t" + "TExceptionTransats vExcepTransats = new TExceptionTransats(\r\n"
            + "\t\t\t\t\t\t\t\t" + "new TExceptionSource(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameFormattedAssembly(Assembly.GetExecutingAssembly()),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameClassExecuting(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameMethodExecuting()),\r\n"
            + "\t\t\t\t\t\t\t\t" + "EnumTypeException.Technical,\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.ErrorCode.ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.Message,\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.GetType().ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t" + "vExp.StackTrace);\r\n"
            + "\t\t\t\t\t" + "// Envoi de l'exception\r\n"
            + "\t\t\t\t\t" + "throw new FaultException<TExceptionTransats>(vExcepTransats, new FaultReason(vExcepTransats.Source.Description));\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "#endregion\r\n"
            + "\t\t\t\t" + "return v{0}sList.ToArray();\r\n"
            + "\t\t\t" + "}}\r\n"
            + "\t\t" + "}}\r\n\r\n";

        private const string INSTANTIATE_OBJECT_TEMPLATE =
              "\t\t" + "/// <summary>\r\n"
            + "\t\t" + "/// Charge les informations d'un {2}\r\n"
            + "\t\t" + "/// </summary>\r\n"
            + "\t\t" + "/// <param name=\"pReader\">\r\n"
            + "\t\t" + "/// Le reader à partir duquel récupérer les données de l'objet.\r\n"
            + "\t\t" + "/// </param>\r\n"
            + "\t\t" + "/// <returns>\r\n"
            + "\t\t" + "/// Une instance complète de {2} récupéré dans la base.\r\n"
            + "\t\t" + "/// Si aucune instance de {2} n'a été trouvée, la fonction renvoie une référence null.\r\n"
            + "\t\t" + "/// </returns>\r\n"
            + "\t\t" + "public static VO_{0} SInstanciate{0}FromDataBase(SqlDataReader pReader)\r\n"
            + "\t\t" + "{{\r\n"
            + "\t\t\t" + "if (!pReader.Read()) {{return null;}}\r\n"
            + "\t\t\t" + "VO_{0} vResult = new VO_{0}();\r\n"
            + "{1}"
            + "\t\t\t" + "return vResult;\r\n"
            + "\t\t" + "}}\r\n\r\n";

        private const string INSERT_METHOD_WITH_IDENTITY_TEMPLATE =
           "\t\t" + "/// <summary>\r\n"
           + "\t\t" + "/// Insère un {4} dans la base de données\r\n"
           + "\t\t" + "/// </summary>\r\n"
           + "\t\t" + "/// <param name=\"p{5}\">\r\n"
           + "\t\t" + "/// Le {4} à insérer.\r\n"
           + "\t\t" + "/// </param>\r\n"
           + "\t\t" + "public static void SCreate{0}(VO_{0} p{5})\r\n"
           + "\t\t" + "{{\r\n"
           + "\t\t\t" + "using (TransactionScope vScope = TFournisseurScope.SRetourneScope(TransactionScopeOption.Required))\r\n"
           + "\t\t\t" + "{{\r\n"
           + "\t\t\t\t" + "using (SqlConnection vConnection = TFournisseurConnexion.RetourneConnexion())\r\n"
           + "\t\t\t\t" + "{{\r\n"
           + "\t\t\t\t\t" + "try\r\n"
           + "\t\t\t\t\t" + "{{\r\n"
           + "\t\t\t\t\t\t" + "SqlCommand vCommand = new SqlCommand(\"PI_CREATE_{0}_auto\", vConnection);\r\n"
           + "\t\t\t\t\t\t" + "vCommand.CommandType = CommandType.StoredProcedure;\r\n"
           + "{2}"
           + "\t\t\t\t\t\t" + "p{5}.{3} = Convert.ToInt32(vCommand.ExecuteScalar());\r\n"
           + "\t\t\t\t\t" + "}}\r\n"
           + "\t\t\t\t\t" + "#region Exceptions : DAL\r\n"
           + "\t\t\t\t\t" + "catch (SqlException vExp)\r\n"
           + "\t\t\t\t\t" + "{{\r\n"
           + "\t\t\t\t\t\t" + "// Création d'une exception personnalisée\r\n"
           + "\t\t\t\t\t\t" + "TExceptionTransats vExcepTransats = new TExceptionTransats(\r\n"
           + "\t\t\t\t\t\t\t\t\t" + "new TExceptionSource(\r\n"
           + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameFormattedAssembly(Assembly.GetExecutingAssembly()),\r\n"
           + "\t\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameClassExecuting(),\r\n"
           + "\t\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameMethodExecuting()),\r\n"
           + "\t\t\t\t\t\t\t\t\t" + "EnumTypeException.Technical,\r\n"
           + "\t\t\t\t\t\t\t\t\t" + "vExp.ErrorCode.ToString(),\r\n"
           + "\t\t\t\t\t\t\t\t\t" + "vExp.Message,\r\n"
           + "\t\t\t\t\t\t\t\t\t" + "vExp.GetType().ToString(),\r\n"
           + "\t\t\t\t\t\t\t\t\t" + "vExp.StackTrace);\r\n"
           + "\t\t\t\t\t\t" + "// Envoi de l'exception\r\n"
           + "\t\t\t\t\t\t" + "throw new FaultException<TExceptionTransats>(vExcepTransats, new FaultReason(vExcepTransats.Source.Description));\r\n"
           + "\t\t\t\t" + "}}\r\n"
           + "\t\t\t\t\t" + "#endregion\r\n"
           + "\t\t\t\t" + "}}\r\n"
           + "\t\t\t\t" + "vScope.Complete();\r\n"
           + "\t\t\t" + "}}\r\n"
           + "\t\t" + "}}\r\n\r\n";

        private const string INSERT_METHOD_TEMPLATE =
            "\t\t" + "/// <summary>\r\n"
            + "\t\t" + "/// Insère un {3} dans la base de données\r\n"
            + "\t\t" + "/// </summary>\r\n"
            + "\t\t" + "/// <param name=\"p{4}\">\r\n"
            + "\t\t" + "/// Le {3} à insérer.\r\n"
            + "\t\t" + "/// </param>\r\n"
            + "\t\t" + "public static void SCreate{0}(VO_{0} p{4})\r\n"
            + "\t\t" + "{{\r\n"
            + "\t\t\t" + "using (TransactionScope vScope = TFournisseurScope.SRetourneScope(TransactionScopeOption.Required))\r\n"
            + "\t\t\t" + "{{\r\n"
            + "\t\t\t\t" + "using (SqlConnection vConnection = TFournisseurConnexion.RetourneConnexion())\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "try\r\n"
            + "\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t" + "SqlCommand vCommand = new SqlCommand(\"PI_CREATE_{0}_auto\", vConnection);\r\n"
            + "\t\t\t\t\t\t" + "vCommand.CommandType = CommandType.StoredProcedure;\r\n"
            + "{2}"
            + "\t\t\t\t\t\t" + "vCommand.ExecuteNonQuery();\r\n"
            + "\t\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t" + "#region Exceptions : DAL\r\n"
            + "\t\t\t\t\t" + "catch (SqlException vExp)\r\n"
            + "\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t" + "// Création d'une exception personnalisée\r\n"
            + "\t\t\t\t\t\t" + "TExceptionTransats vExcepTransats = new TExceptionTransats(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "new TExceptionSource(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameFormattedAssembly(Assembly.GetExecutingAssembly()),\r\n"
            + "\t\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameClassExecuting(),\r\n"
            + "\t\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameMethodExecuting()),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "EnumTypeException.Technical,\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.ErrorCode.ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.Message,\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.GetType().ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.StackTrace);\r\n"
            + "\t\t\t\t\t\t" + "// Envoi de l'exception\r\n"
            + "\t\t\t\t\t\t" + "throw new FaultException<TExceptionTransats>(vExcepTransats, new FaultReason(vExcepTransats.Source.Description));\r\n"
            + "\t\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t" + "#endregion\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "vScope.Complete();\r\n"
            + "\t\t\t" + "}}\r\n"
            + "\t\t" + "}}\r\n\r\n";

        private const string UPDATE_METHOD_TEMPLATE =
            "\t\t" + "/// <summary>\r\n"
            + "\t\t" + "/// Modifie un {3} dans la base de données\r\n"
            + "\t\t" + "/// </summary>\r\n"
            + "\t\t" + "/// <param name=\"p{4}\">\r\n"
            + "\t\t" + "/// Le {3} à modifier.\r\n"
            + "\t\t" + "/// </param>\r\n"
            + "\t\t" + "public static void SUpdate{0}(VO_{0} p{4})\r\n"
            + "\t\t" + "{{\r\n"
            + "\t\t\t" + "using (TransactionScope vScope = TFournisseurScope.SRetourneScope(TransactionScopeOption.Required))\r\n"
            + "\t\t\t" + "{{\r\n"
            + "\t\t\t\t" + "using (SqlConnection vConnection = TFournisseurConnexion.RetourneConnexion())\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "try\r\n"
            + "\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t" + "SqlCommand vCommand = new SqlCommand(\"PU_UPDATE_{0}_auto\", vConnection);\r\n"
            + "\t\t\t\t\t\t" + "vCommand.CommandType = CommandType.StoredProcedure;\r\n"
            + "{2}"
            + "\t\t\t\t\t\t" + "vCommand.ExecuteNonQuery();\r\n"
            + "\t\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t" + "#region Exceptions : DAL\r\n"
            + "\t\t\t\t\t" + "catch (SqlException vExp)\r\n"
            + "\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t" + "// Création d'une exception personnalisée\r\n"
            + "\t\t\t\t\t\t" + "TExceptionTransats vExcepTransats = new TExceptionTransats(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "new TExceptionSource(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameFormattedAssembly(Assembly.GetExecutingAssembly()),\r\n"
            + "\t\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameClassExecuting(),\r\n"
            + "\t\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameMethodExecuting()),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "EnumTypeException.Technical,\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.ErrorCode.ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.Message,\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.GetType().ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.StackTrace);\r\n"
            + "\t\t\t\t\t\t" + "// Envoi de l'exception\r\n"
            + "\t\t\t\t\t\t" + "throw new FaultException<TExceptionTransats>(vExcepTransats, new FaultReason(vExcepTransats.Source.Description));\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t" + "#endregion\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "vScope.Complete();\r\n"
            + "\t\t\t" + "}}\r\n"
            + "\t\t" + "}}\r\n\r\n";


        private const string DELETE_METHOD_TEMPLATE =
            "\t\t" + "/// <summary>\r\n"
            + "\t\t" + "/// Supprime un {5} dans la base de données\r\n"
            + "\t\t" + "/// </summary>\r\n"
            + "{4}"
            + "\t\t" + "public static void SDelete{0}({2})\r\n"
            + "\t\t" + "{{\r\n"
            + "\t\t\t" + "using (TransactionScope vScope = TFournisseurScope.SRetourneScope(TransactionScopeOption.Required))\r\n"
            + "\t\t\t" + "{{\r\n"
            + "\t\t\t\t" + "using (SqlConnection vConnection = TFournisseurConnexion.RetourneConnexion())\r\n"
            + "\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t" + "try\r\n"
            + "\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t" + "SqlCommand vCommand = new SqlCommand(\"PD_DELETE_{0}_auto\", vConnection);\r\n"
            + "\t\t\t\t\t\t" + "vCommand.CommandType = CommandType.StoredProcedure;\r\n"
            + "{3}"
            + "\t\t\t\t\t\t" + "vCommand.ExecuteNonQuery();\r\n"
            + "\t\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t" + "#region Exceptions : DAL\r\n"
            + "\t\t\t\t\t" + "catch (SqlException vExp)\r\n"
            + "\t\t\t\t\t" + "{{\r\n"
            + "\t\t\t\t\t\t" + "// Création d'une exception personnalisée\r\n"
            + "\t\t\t\t\t\t" + "TExceptionTransats vExcepTransats = new TExceptionTransats(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "new TExceptionSource(\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameFormattedAssembly(Assembly.GetExecutingAssembly()),\r\n"
            + "\t\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameClassExecuting(),\r\n"
            + "\t\t\t\t\t\t\t\t\t\t" + "TReflection.SGetNameMethodExecuting()),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "EnumTypeException.Technical,\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.ErrorCode.ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.Message,\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.GetType().ToString(),\r\n"
            + "\t\t\t\t\t\t\t\t\t" + "vExp.StackTrace);\r\n"
            + "\t\t\t\t\t\t" + "// Envoi de l'exception\r\n"
            + "\t\t\t\t\t\t" + "throw new FaultException<TExceptionTransats>(vExcepTransats, new FaultReason(vExcepTransats.Source.Description));\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t\t" + "#endregion\r\n"
            + "\t\t\t\t" + "}}\r\n"
            + "\t\t\t\t" + "vScope.Complete();\r\n"
            + "\t\t\t" + "}}\r\n"
            + "\t\t}}\r\n\r\n";

        private const string REGEXP_CLEANUP_NAME =
            "[A-Z][a-z][a-z]*|[A-Z][A-Z]*|[a-z]*";

        private Table _table;

        # endregion
    }
}
