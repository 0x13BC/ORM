using System;
using System.Collections;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace com.castsoftware.tools
{
	public class Table
	{
		#region CONSTRUCTORS
		public Table()
		{
			return;
		}

		public Table( string name )
		{
			_name = name;
			_className = toCamel(name);
			_obsolete = false;
			return;
		}
		#endregion

		#region PROPERTIES
		[XmlArray( "columns" ), XmlArrayItem( "column" )]
		public Column[] Columns
		{
			get { return (Column[])_columns.ToArray( typeof( Column ) ); }
			set
			{
				_columns.Clear();
				_columnsByName.Clear();
				_columns.AddRange( value );
				foreach ( Column scannedColumn in _columns )
				{
					scannedColumn.Owner = this;
					_columnsByName.Add( scannedColumn.Name, scannedColumn );
				}
				return;
			}
		}

		[XmlAttribute( "className" )]
		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}

		/// <summary>
		/// Retourne la colonne code de la table s'il en existe une. Retourne null sinon.
		/// </summary>
		public Column CodeColumn
		{
			get
			{
				foreach ( Column scannedColumn in Columns ) { if ( scannedColumn.Code ) { return scannedColumn; } }
				return null;
			}
		}

		[XmlElement( "dataClassSourceCodeCDL" )]
		public string DataClassSourceCodeCDL
		{
			get { return _dataClassSourceCodeCDL; }
			set { _dataClassSourceCodeCDL = value; }
		}

		[XmlElement( "dataClassSourceCodeDAL" )]
		public string DataClassSourceCodeDAL
		{
			get { return _dataClassSourceCodeDAL; }
			set { _dataClassSourceCodeDAL = value; }
		}

		/// <summary>
		///  Retourne ou modifie la description
		/// </summary>
		[XmlAttribute( "description" )]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		[XmlAttribute( "dbGeneration" )]
		public int DbGeneration
		{
			get { return _dbGeneration; }
			set { _dbGeneration = value; }
		}

		/// <summary>
		/// Retourne true si la colonne possède une colonne code.
		/// </summary>
		public bool HasCode
		{
			get { return CodeColumn != null; }
		}

		/// <summary>
		/// Retourne ou mets à jour le nom de la table
		/// </summary>
		[XmlAttribute( "tableName" )]
		public string Name
		{
			get { return _name; }
			set
			{
				IncrementDbGeneration();
				_name = value;
				return;
			}
		}

		/// <summary>
		/// Retourne le nom de la vue associée à la table
		/// </summary>
		public string ViewName
		{
			get { return ComputeViewName(); }
		}

		[XmlAttribute( "obsolete" )]
		public bool Obsolete
		{
			get { return _obsolete; }
			set
			{
				IncrementDbGeneration();
				_obsolete = value;
				return;
			}
		}

		[XmlElement( "selectByCodeProcedureScript" )]
		public ObjectScript SelectByCodeProcedureScript
		{
			get { return _selectByCodeProcedureScript; }
			set { _selectByCodeProcedureScript = value; }
		}

		[XmlElement( "selectProcedureScript" )]
		public ObjectScript SelectProcedureScript
		{
			get { return _selectProcedureScript; }
			set { _selectProcedureScript = value; }
		}

		[XmlElement( "selectListProcedureScript" )]
		public ObjectScript SelectListProcedureScript
		{
			get { return _selectListProcedureScript; }
			set { _selectListProcedureScript = value; }
		}

		[XmlElement( "insertProcedureScript" )]
		public ObjectScript InsertProcedureScript
		{
			get { return _insertProcedureScript; }
			set { _insertProcedureScript = value; }
		}

		[XmlElement( "updateProcedureScript" )]
		public ObjectScript UpdateProcedureScript
		{
			get { return _updateProcedureScript; }
			set { _updateProcedureScript = value; }
		}

		[XmlElement( "deleteProcedureScript" )]
		public ObjectScript DeleteProcedureScript
		{
			get { return _deleteProcedureScript; }
			set { _deleteProcedureScript = value; }
		}

		[XmlElement( "viewScript" )]
		public ObjectScript ViewScript
		{
			get { return _viewScript; }
			set { _viewScript = value; }
		}

		#endregion

		#region METHODS
		public void AddColumn( Column column )
		{
			if ( !_columnsByName.ContainsKey( column.Name ) )
			{
				_columns.Add( column );
				_columnsByName.Add( column.Name, column );
			}
			return;
		}

		private string ComputeAllColumnsList()
		{
			StringBuilder builder = new StringBuilder();

			foreach ( Column scannedColumn in Columns )
			{
				if ( builder.Length != 0 ) { builder.Append( ", " ); }
				builder.Append( "["+scannedColumn.Name+"]" );
			}
			return builder.ToString();
		}

		private string ComputeNonPrimaryKeyColumnsList()
		{
			StringBuilder builder = new StringBuilder();

			foreach ( Column scannedColumn in Columns )
			{
				if ( !scannedColumn.PrimaryKeyMember )
				{
					if ( builder.Length != 0 ) { builder.Append( ", " ); }
					builder.Append( "[" + scannedColumn.Name + "]" );
				}

			}
			return builder.ToString();
		}

		private string ComputeSelectProcedureName()
		{
			return "PS_GET_" + ClassName + "_auto";
		}

		private string ComputeSelectByCodeProcedureName()
		{
            return "PS_GET_" + ClassName + "_BY_" + CodeColumn.AttributeName + "_auto";
		}

		private string ComputeSelectListProcedureName()
		{
            return "PS_GET_ALL_" + ClassName + "s" + "_auto";
		}

		private string ComputeInsertProcedureName()
		{
            return "PI_CREATE_" + ClassName + "_auto";
		}

		private string ComputeUdpateProcedureName()
		{
            return "PU_UPDATE_" + ClassName + "_auto";
		}

		private string ComputeDeleteProcedureName()
		{
            return "PD_DELETE_" + ClassName + "_auto";
		}

		private string ComputeViewName()
		{
            return "V" + Name;
		}

		public Column FindColumn( string columnName )
		{
			return (Column)_columnsByName[columnName];
		}

		public bool HasIdentity
		{
			get
			{
				return (GetIdentityColumnName() != null);
			}
		}

		public string GetIdentityColumnName()
		{
			foreach ( Column current in Columns )
			{
				if ( current.IdentityColumn )
				{
					return current.AttributeName;
				}
			}
			return null;
		}

		internal void IncrementDbGeneration()
		{
			if ( _owner != null ) { _dbGeneration = _owner.GetNextDbGeneration(); }
			return;
		}

		private bool IsScriptToBeUpdated( ref ObjectScript candidate, bool forceUpdate )
		{
			if ( !forceUpdate
				&& (candidate != null)
				&& (candidate.DbGeneration == _dbGeneration) )
			{
				return false;
			}
			if ( candidate == null ) { candidate = new ObjectScript(); }
			return true;
		}

		internal void SetOwner( Mappings owner )
		{
			_owner = owner;
			return;
		}

		public void UpdateDataClassDefinition( bool forceUpdate )
		{
			if ( Obsolete ) { return; }
			CDLGenerator cdlgenerator = new CDLGenerator( this );

			_dataClassSourceCodeCDL = cdlgenerator.Generate();
			DALGenerator dalgenerator = new DALGenerator( this );
			_dataClassSourceCodeDAL = dalgenerator.Generate();
			return;
		}

		public void UpdateScripts()
		{
			UpdateScripts( false );
			return;
		}

		public void UpdateScripts( bool forceUpdate )
		{
			if ( Obsolete ) { return; }
			UpdateViewScript( forceUpdate );
			UpdateSelectProcedureScript( forceUpdate );
			UpdateInsertProcedureScript( forceUpdate );
			UpdateUpdateProcedureScript( forceUpdate );
			UpdateDeleteProcedureScript( forceUpdate );

			UpdateDataClassDefinition( forceUpdate );
			return;
		}

		private void UpdateSelectProcedureScript( bool forceUpdate )
		{
			if ( !IsScriptToBeUpdated( ref _selectProcedureScript, forceUpdate ) )
			{ return; }
			string l_ProcName = ComputeSelectProcedureName();
			StringBuilder parametersBuilder = new StringBuilder();
			StringBuilder conditionBuilder = new StringBuilder();

			_selectProcedureScript.DbGeneration = _dbGeneration;
			_selectProcedureScript.ObjectName = l_ProcName;

			foreach ( Column scannedColumn in Columns )
			{
				if ( scannedColumn.PrimaryKeyMember )
				{
					if ( parametersBuilder.Length > 0 )
					{
						parametersBuilder.Append( ", " );
						conditionBuilder.Append( " AND " );
					}
					parametersBuilder.Append( "@" + scannedColumn.Name.Replace( "-", "_" ) + " " );
					parametersBuilder.Append( scannedColumn.GetSqlTypeDefinition() );
					conditionBuilder.Append
						( String.Format
						  ( "[{0}] = @{1}"
						  , scannedColumn.Name
						  , scannedColumn.Name.Replace( "-", "_" )
						  )
						);
				}
			}
			if ( parametersBuilder.Length > 0 )
			{
				_selectProcedureScript.CreateScript.Comment = string.Format
					( CREATE_SELECT_PROCEDURE_COMMENT
					, Name
					, DateTime.Today.ToString( "d" )
					, ClassName
					);
				_selectProcedureScript.CreateScript.Script = string.Format
					( CREATE_SELECT_PROCEDURE_TEMPLATE
					, l_ProcName
					, parametersBuilder.ToString()
					, ComputeAllColumnsList()
					, ComputeViewName()
					, conditionBuilder.ToString()
					);

				_selectProcedureScript.DropScript.Comment = string.Format
					( DROP_PROCEDURE_COMMENT
					, l_ProcName
					);
				_selectProcedureScript.DropScript.Script = string.Format
					( DROP_PROCEDURE_TEMPLATE
					, l_ProcName
					);
			}

			l_ProcName = ComputeSelectListProcedureName();
			if ( !IsScriptToBeUpdated( ref _selectListProcedureScript, forceUpdate ) )
			{ return; }

			_selectListProcedureScript.DbGeneration = _dbGeneration;
			_selectListProcedureScript.ObjectName = l_ProcName;

			_selectListProcedureScript.CreateScript.Comment = string.Format
				( CREATE_SELECT_LIST_PROCEDURE_COMMENT
				, Name
				, DateTime.Today.ToString( "d" )
				, ClassName
				);
			_selectListProcedureScript.CreateScript.Script = string.Format
				( CREATE_SELECT_LIST_PROCEDURE_TEMPLATE
				, l_ProcName
				, ComputeAllColumnsList()
				, ComputeViewName()
				);

			_selectListProcedureScript.DropScript.Comment = string.Format
				( DROP_PROCEDURE_COMMENT
				, l_ProcName
				);
			_selectListProcedureScript.DropScript.Script = string.Format
				( DROP_PROCEDURE_TEMPLATE
				, l_ProcName
				);

			if ( !HasCode ) { return; }
			if ( !IsScriptToBeUpdated( ref _selectByCodeProcedureScript, forceUpdate ) )
			{ return; }

			Column codeColumn = CodeColumn;
			string sqlParameterName = "@" + codeColumn.Name.Replace( "-", "_" );

			l_ProcName = ComputeSelectByCodeProcedureName();
			parametersBuilder.Length = 0;
			parametersBuilder.Append( sqlParameterName + " " );
			parametersBuilder.Append( codeColumn.GetSqlTypeDefinition() );

			conditionBuilder.Length = 0;
			conditionBuilder.Append( String.Format( "[{0}] = {1}", codeColumn.Name, sqlParameterName ) );

			_selectByCodeProcedureScript.DbGeneration = _dbGeneration;
			_selectByCodeProcedureScript.ObjectName = l_ProcName;

			_selectByCodeProcedureScript.CreateScript.Comment = string.Format
				( CREATE_SELECT_PROCEDURE_COMMENT
				, Name
				, DateTime.Today.ToString( "d" )
				, ClassName
				);
			_selectByCodeProcedureScript.CreateScript.Script = string.Format
				( CREATE_SELECT_PROCEDURE_TEMPLATE
				, l_ProcName
				, parametersBuilder.ToString()
				, ComputeAllColumnsList()
				, ComputeViewName()
				, conditionBuilder.ToString()
				);

			_selectByCodeProcedureScript.DropScript.Comment = string.Format
				( DROP_PROCEDURE_COMMENT
				, l_ProcName
				);
			_selectByCodeProcedureScript.DropScript.Script = string.Format
				( DROP_PROCEDURE_TEMPLATE
				, l_ProcName
				);

			return;
		}

		private void UpdateInsertProcedureScript( bool forceUpdate )
		{
			if ( !IsScriptToBeUpdated( ref _insertProcedureScript, forceUpdate ) )
			{ return; }

			string l_ProcName = ComputeInsertProcedureName();
			StringBuilder parametersBuilder = new StringBuilder();
			StringBuilder valuesBuilder = new StringBuilder();
			StringBuilder insertBuilder = new StringBuilder();

			_insertProcedureScript.DbGeneration = _dbGeneration;
			_insertProcedureScript.ObjectName = l_ProcName;

			foreach ( Column scannedColumn in Columns )
			{
				if ( !scannedColumn.IdentityColumn )
				{
					if ( parametersBuilder.Length > 0 )
					{
						parametersBuilder.Append( ", " );
						valuesBuilder.Append( ", " );
						insertBuilder.Append( ", " );
					}
					parametersBuilder.AppendFormat( PARAMETER_TEMPLATE, scannedColumn.Name.Replace( "-", "_" ), scannedColumn.GetSqlTypeDefinition() );
					valuesBuilder.AppendFormat( VALUE_TEMPLATE, scannedColumn.Name.Replace( "-", "_" ) );
					insertBuilder.AppendFormat( COLUMN_TEMPLATE, scannedColumn.Name );
				}
			}

			if ( parametersBuilder.Length > 0 )
			{
				_insertProcedureScript.CreateScript.Comment = string.Format
					( HasIdentity
						? CREATE_INSERT_GET_IDENTITY_PROCEDURE_COMMENT
						: CREATE_INSERT_PROCEDURE_COMMENT
					, Name
					, DateTime.Today.ToString( "d" )
					, ClassName
					);
				_insertProcedureScript.CreateScript.Script = string.Format
					( HasIdentity 
						? CREATE_INSERT_GET_IDENTITY_PROCEDURE_TEMPLATE 
						: CREATE_INSERT_PROCEDURE_TEMPLATE
					, l_ProcName
					, parametersBuilder.ToString()
					, Name
					, insertBuilder.ToString()
					, valuesBuilder.ToString()
					);

				_insertProcedureScript.DropScript.Comment = string.Format
					( DROP_PROCEDURE_COMMENT
					, l_ProcName
					);
				_insertProcedureScript.DropScript.Script = string.Format
					( DROP_PROCEDURE_TEMPLATE
					, l_ProcName
					);
			}
			return;
		}

		private void UpdateUpdateProcedureScript( bool forceUpdate )
		{
			if ( !IsScriptToBeUpdated( ref _updateProcedureScript, forceUpdate ) )
			{ return; }

			string l_ProcName = ComputeUdpateProcedureName();
			StringBuilder parametersBuilder = new StringBuilder();
			StringBuilder setBuilder = new StringBuilder();
			StringBuilder conditionBuilder = new StringBuilder();

			_updateProcedureScript.DbGeneration = _dbGeneration;
			_updateProcedureScript.ObjectName = l_ProcName;

			foreach ( Column scannedColumn in Columns )
			{
				if ( scannedColumn.PrimaryKeyMember )
				{
					if ( conditionBuilder.Length > 0 )
					{
						conditionBuilder.Append( " AND " );
					}
					conditionBuilder.Append( String.Format( "[{0}] = @{1}",
						scannedColumn.Name, scannedColumn.Name.Replace( "-", "_" ) ) );
				}
				else
				{
					if ( setBuilder.Length > 0 )
					{
						setBuilder.Append( ", " );
					}
					setBuilder.Append( String.Format( "[{0}] = @{1}",
						scannedColumn.Name, scannedColumn.Name.Replace( "-", "_" ) ) );
				}
				if ( parametersBuilder.Length > 0 )
				{
					parametersBuilder.Append( ", " );
				}
				parametersBuilder.Append( "@" + scannedColumn.Name.Replace( "-", "_" ) + " " );
				parametersBuilder.Append( scannedColumn.GetSqlTypeDefinition() );
			}

			if ( parametersBuilder.Length > 0 && setBuilder.Length > 0 && conditionBuilder.Length > 0 )
			{
				_updateProcedureScript.CreateScript.Comment = string.Format
					( CREATE_UPDATE_PROCEDURE_COMMENT
					, Name
					, DateTime.Today.ToString( "d" )
					, ClassName
					);
				_updateProcedureScript.CreateScript.Script = string.Format
					( CREATE_UPDATE_PROCEDURE_TEMPLATE
					, l_ProcName
					, parametersBuilder.ToString()
					, ComputeViewName()
					, setBuilder.ToString()
					, conditionBuilder.ToString()
					);

				_updateProcedureScript.DropScript.Comment = string.Format
					( DROP_PROCEDURE_COMMENT
					, l_ProcName
					);
				_updateProcedureScript.DropScript.Script = string.Format
					( DROP_PROCEDURE_TEMPLATE
					, l_ProcName
					);
			}
			return;
		}

		private void UpdateDeleteProcedureScript( bool forceUpdate )
		{
			if ( !IsScriptToBeUpdated( ref _deleteProcedureScript, forceUpdate ) )
			{ return; }

			string l_ProcName = ComputeDeleteProcedureName();
			StringBuilder parametersBuilder = new StringBuilder();
			StringBuilder conditionBuilder = new StringBuilder();

			_deleteProcedureScript.DbGeneration = _dbGeneration;
			_deleteProcedureScript.ObjectName = l_ProcName;

			foreach ( Column scannedColumn in Columns )
			{
				if ( scannedColumn.PrimaryKeyMember )
				{
					if ( parametersBuilder.Length > 0 )
					{
						parametersBuilder.Append( ", " );
						conditionBuilder.Append( " AND " );
					}
					parametersBuilder.Append( "@" + scannedColumn.Name.Replace( "-", "_" ) + " " );
					parametersBuilder.Append( scannedColumn.GetSqlTypeDefinition() );
					conditionBuilder.Append
						( String.Format
						  ( "[{0}] = @{1}"
						  , scannedColumn.Name
						  , scannedColumn.Name.Replace( "-", "_" )
						  )
						);
				}
			}
			if ( parametersBuilder.Length > 0 )
			{
				_deleteProcedureScript.CreateScript.Comment = string.Format
					( CREATE_DELETE_PROCEDURE_COMMENT
					, Name
					, DateTime.Today.ToString( "d" )
					, ClassName
					);
				_deleteProcedureScript.CreateScript.Script = string.Format
					( CREATE_DELETE_PROCEDURE_TEMPLATE
					, l_ProcName
					, parametersBuilder.ToString()
					, ComputeViewName()
					, conditionBuilder.ToString()
					);

				_deleteProcedureScript.DropScript.Comment = string.Format
					( DROP_PROCEDURE_COMMENT
					, l_ProcName
					);
				_deleteProcedureScript.DropScript.Script = string.Format
					( DROP_PROCEDURE_TEMPLATE
					, l_ProcName
					);
			}
			return;
		}

		private void UpdateViewScript( bool forceUpdate )
		{
			if ( !IsScriptToBeUpdated( ref _viewScript, forceUpdate ) )
			{ return; }
			
			_viewScript.DbGeneration = _dbGeneration;
			_viewScript.ObjectName = ViewName;

			_viewScript.CreateScript.Comment = string.Format
				( CREATE_VIEW_COMMENT
				, Name
				, DateTime.Today.ToString( "d" )
				);
			_viewScript.CreateScript.Script = string.Format
				( CREATE_VIEW_TEMPLATE
				, ViewName
				, ComputeAllColumnsList()
				, Name
				);
			_viewScript.DropScript.Comment = string.Format
				( DROP_VIEW_COMMENT
				, ViewName
				);
			_viewScript.DropScript.Script = string.Format
				( DROP_VIEW_TEMPLATE
				, ViewName
				);
			return;
		}

        public static string toCamel(string p_input)
        {
            string l_result = "";
            if (p_input.Length > 1)
            {
                l_result = p_input.ToLower();
                string l_firstPart = "";
                string l_upperLetter = l_result.Substring(0, 1).ToUpper();
                string l_lastPart = l_result.Substring(1);
                l_result = l_firstPart + l_upperLetter + l_lastPart;
                int l_underscorePosition = l_result.IndexOf("_");
                while ((l_underscorePosition != -1) && (l_underscorePosition + 2 <= l_result.Length))
                {
                    l_firstPart = l_result.Substring(0, l_underscorePosition);
                    l_upperLetter = l_result.Substring(l_underscorePosition + 1, 1).ToUpper();
                    l_lastPart = l_result.Substring(l_underscorePosition + 2);
                    l_result = l_firstPart + l_upperLetter + l_lastPart;
                    l_underscorePosition = l_result.IndexOf("_");
                }
            }
            return l_result;
        }
		#endregion

		#region ATTRIBUTES
		private string _className = "";
		private ArrayList _columns = new ArrayList();
		private Hashtable _columnsByName = new Hashtable();
		private int _dbGeneration;
		private string _description = "";
		private string _name;
		private bool _obsolete;
		private Mappings _owner;
		private ObjectScript _selectProcedureScript;
		private ObjectScript _selectByCodeProcedureScript;
		private ObjectScript _selectListProcedureScript;
		private ObjectScript _insertProcedureScript;
		private ObjectScript _updateProcedureScript;
		private ObjectScript _deleteProcedureScript;
		private ObjectScript _viewScript;
		private string _dataClassSourceCodeCDL = "";
		private string _dataClassSourceCodeDAL = "";

		private const string COLUMN_TEMPLATE =
            "[{0}]";

		private const string PARAMETER_TEMPLATE =
            "@{0} {1}";

		private const string VALUE_TEMPLATE =
            "@{0}";

		private const string CREATE_VIEW_COMMENT 
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Vue sur la table {0} \r\n"
            + "--   \r\n"
//          + "--  Méthode appelante : {X} \r\n"
            + "--  Création :   le {1} par CAST \r\n"
            + "------------------------------------------------------------------------------------";

		private const string CREATE_VIEW_TEMPLATE 
            = "CREATE VIEW [dbo].[{0}] \r\n"
            + "  AS \r\n"
            + "  SELECT {1} \r\n"
            + "  FROM [dbo].[{2}]";

		private const string CREATE_SELECT_PROCEDURE_COMMENT
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Selection d'un enregistrement dans la table {0} \r\n"
            + "--   \r\n"
            + "--  Méthode appelante : DA_{2}.SGet{2}() \r\n"
            + "--  Création :   le {1} par CAST \r\n"
            + "------------------------------------------------------------------------------------";

		private const string CREATE_SELECT_PROCEDURE_TEMPLATE 
            = "CREATE proc {0} ({1}) AS \r\n"
            + "  SELECT {2} \r\n"
            + "  FROM [{3}] \r\n"
            + "  WHERE ({4}) \r\n"
            + "GO \r\n"
            + "GRANT EXEC ON {0} TO PUBLIC";

		private const string CREATE_SELECT_LIST_PROCEDURE_COMMENT
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Selection de tous les enregistrements de la table {0} \r\n"
            + "--   \r\n"
            + "--  Méthode appelante : DA_{2}.SGetAll{2}s() \r\n"
            + "--  Création :   le {1} par CAST \r\n"
            + "------------------------------------------------------------------------------------";

		private const string CREATE_SELECT_LIST_PROCEDURE_TEMPLATE 
            = "CREATE proc {0} AS \r\n"
            + "  SELECT {1} \r\n"
            + "  FROM [{2}] \r\n"
            + "GO \r\n"
            + "GRANT EXEC ON {0} TO PUBLIC";

		private const string CREATE_INSERT_PROCEDURE_COMMENT
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Insertion d'un enregistrement dans la table {0} \r\n"
            + "--   \r\n"
            + "--  Méthode appelante : DA_{2}.SCreate{2}() \r\n"
            + "--  Création :   le {1} par CAST \r\n"
            + "------------------------------------------------------------------------------------";

		private const string CREATE_INSERT_PROCEDURE_TEMPLATE 
            = "CREATE proc {0} ({1}) AS \r\n"
            + "  INSERT INTO [{2}] ({3}) \r\n"
            + "  VALUES ({4}) \r\n"
            + "GO \r\n"
            + "GRANT EXEC ON {0} TO PUBLIC";

		private const string CREATE_INSERT_GET_IDENTITY_PROCEDURE_COMMENT
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Insertion d'un enregistrement dans la table {0} \r\n"
            + "--  et retourne l'identifiant généré. \r\n"
            + "--   \r\n"
            + "--  Méthode appelante : DA_{2}.SCreate{2}() \r\n"
            + "--  Création :   le {1} par CAST \r\n"
            + "------------------------------------------------------------------------------------";

		private const string CREATE_INSERT_GET_IDENTITY_PROCEDURE_TEMPLATE 
            = "CREATE proc {0} ({1}) AS \r\n"
            + "  BEGIN \r\n"
            + "   INSERT INTO [{2}] ({3}) \r\n"
            + "   VALUES ({4}) \r\n"
            + "   SELECT @@identity \r\n"
            + "  END \r\n"
            + "GO \r\n"
            + "GRANT EXEC ON {0} TO PUBLIC";

		private const string CREATE_UPDATE_PROCEDURE_COMMENT
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Mise à jour d'un enregistrement dans la table {0} \r\n"
            + "--   \r\n"
            + "--  Méthode appelante : DA_{2}.SUpdate{2}() \r\n"
            + "--  Création :   le {1} par CAST \r\n"
            + "------------------------------------------------------------------------------------";

		private const string CREATE_UPDATE_PROCEDURE_TEMPLATE 
            = "CREATE proc {0} ({1}) AS \r\n"
            + "  UPDATE [{2}] \r\n"
            + "  SET {3} \r\n"
            + "  WHERE ({4}) \r\n"
            + "GO \r\n"
            + "GRANT EXEC ON {0} TO PUBLIC";


		private const string CREATE_DELETE_PROCEDURE_COMMENT
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Suppression d'un enregistrement dans la table {0} \r\n"
            + "--   \r\n"
            + "--  Méthode appelante : DA_{2}.SDelete{2}() \r\n"
            + "--  Création :   le {1} par CAST \r\n"
            + "------------------------------------------------------------------------------------";

		private const string CREATE_DELETE_PROCEDURE_TEMPLATE 
            = "CREATE proc {0} ({1}) AS \r\n"
            + "  DELETE [{2}] \r\n"
            + "  WHERE ({3}) \r\n"
            + "GO \r\n"
            + "GRANT EXEC ON {0} TO PUBLIC";

		private const string DROP_PROCEDURE_COMMENT
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Suppression de la procedure : \r\n"
            + "--  [dbo].[{0}] \r\n"
            + "------------------------------------------------------------------------------------";

		private const string DROP_PROCEDURE_TEMPLATE 
            = "if exists (SELECT * from dbo.sysobjects where id = object_id(N'[dbo].[{0}]') "
            + "and OBJECTPROPERTY(id, N'IsProcedure') = 1) \r\n"
			+ "  drop procedure [dbo].[{0}]";

		private const string DROP_VIEW_COMMENT
            = "------------------------------------------------------------------------------------\r\n"
            + "--  Suppression de la vue : \r\n"
            + "--  [dbo].[{0}] \r\n"
            + "------------------------------------------------------------------------------------";

		private const string DROP_VIEW_TEMPLATE 
            = "if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[{0}]') "
            + "and OBJECTPROPERTY(id, N'IsView') = 1) \r\n"
			+ "  drop view [dbo].[{0}]";

		#endregion
	}
}
