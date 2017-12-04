using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace com.castsoftware.tools
{
	public class XmlFileGenerator
	{
		public delegate void DbGenerationUpdateDelegate();
		public event DbGenerationUpdateDelegate DbGenerationUpdate;

		#region CONSTRUCTORS
		/// <summary>
		/// Instanciates a non file bound generator.
		/// </summary>
		public XmlFileGenerator()
		{
			_allMappings = new Mappings();
			_allMappings.Generator = this;
			return;
		}

		/// <summary>
		/// Instantiates a file generator that will save its results in the
		/// given file. On entry, if the file already exists it is loaded.
		/// </summary>
		/// <param name="path">
		/// File where to save results.
		/// </param>
		public XmlFileGenerator( string path )
		{
			if ( !File.Exists( path ) ) { _allMappings = new Mappings(); }
			else
			{
				using ( FileStream input = File.OpenRead( path ) )
				{
					_allMappings = (Mappings)(new XmlSerializer( typeof( Mappings ) ).Deserialize( input ));
					_allMappings.Generator = this;
				}
			}
			_path = path;
			_generationDone = true;
			return;
		}
		#endregion

		#region PROPERTIES
		/// <summary>
		/// Get or set database name.
		/// </summary>
		public string DatabaseName
		{
			get { return _allMappings.DatabaseName; }
			set
			{
				//if (_generationDone) { throw new NotSupportedException(); }
				_allMappings.DatabaseName = value;
			}
		}

		/// <summary>
		/// Get wether generation has been performed or not.
		/// </summary>
		public bool GenerationDone
		{
			get { return _generationDone; }
		}

		/// <summary>
		/// Get or set file path.
		/// </summary>
		public string Path
		{
			get { return _path; }
			set { _path = value; }
		}

		/// <summary>
		/// Get or set server name.
		/// </summary>
		public string ServerName
		{
			get { return _allMappings.ServerName; }
			set
			{
				//if (_generationDone) { throw new NotSupportedException(); }
				_allMappings.ServerName = value;
			}
		}

		/// <summary>
		/// Get a list of every user tables.
		/// </summary>
		public Table[] Tables
		{
			get { return _allMappings.AllTables; }
		}
		#endregion

		#region METHODS
		public bool AreDbScriptsUpToDate()
		{
			return (_allMappings.AreDbScriptsUpToDate());
		}

		internal void DbGenerationUpdated()
		{
			if ( DbGenerationUpdate != null ) { DbGenerationUpdate(); }
			return;
		}

		/// <summary>
		/// Generate file content from database catalog.
		/// </summary>
		/// <param name="connectionString">
		/// Connection string used for databaser access.
		/// </param>
		public void Generate( string connectionString )
		{
			using ( SqlConnection connection = new SqlConnection( connectionString ) )
			{
				connection.Open();
				SqlCommand command = new SqlCommand( SELECT_ALL_TABLES_NAME, connection );
				using ( SqlDataReader reader = command.ExecuteReader() )
				{

					while ( reader.Read() )
					{
						string tableName = reader.GetString( 0 );
						Table seekedTable = _allMappings.FindTable( tableName );

						if ( seekedTable == null )
						{ _allMappings.AddTable( new Table( tableName ) ); }
					}
				}

				Table[] allTables = _allMappings.AllTables;
				foreach ( Table scannedTable in allTables )
				{
					command = new SqlCommand( String.Format( SELECT_ALL_COLUMNS, scannedTable.Name ), connection );
					using ( SqlDataReader reader = command.ExecuteReader() )
					{
						try
						{
							while ( reader.Read() )
							{
								string columnName = reader.GetString( 0 );
								Column seekedColumn = scannedTable.FindColumn( columnName );
								if ( seekedColumn == null )
								{
									seekedColumn = new Column( columnName );
									scannedTable.AddColumn( seekedColumn );
								}
								seekedColumn.Nullable = reader.IsDBNull( 1 ) ? false : (reader.GetInt32( 1 ) == 1);
								seekedColumn.SqlType = reader.GetString( 2 );
								seekedColumn.SqlLength = reader.GetInt16( 3 );
								seekedColumn.SqlScale = reader.IsDBNull( 4 ) ? (int)0 : (int)reader.GetInt16( 4 );
								seekedColumn.SqlPrecision = reader.IsDBNull( 5 ) ? (int)0 : (int)reader.GetInt32( 5 );
								seekedColumn.IdentityColumn = reader.IsDBNull( 6 )? false : true;
                                seekedColumn.Description = reader.IsDBNull(7) ? String.Empty : reader.GetString(7);
							}
						}
                        catch (Exception ) {
                            throw;
                            }
					}



					// Primary key columns detection. Version JPF
					//command = new SqlCommand(String.Format(SELECT_PK_COLUMNS, scannedTable.Name), connection);
					//using (SqlDataReader reader = command.ExecuteReader())
					//{
					//    try
					//    {
					//        while (reader.Read())
					//        {
					//            int status = reader.GetInt32(1);
					//            if (((status & 1) == 0) || ((status & 16) == 0)) { continue; }
					//            string columnName = reader.GetString(0);
					//            Column seekedColumn = scannedTable.FindColumn(columnName);
					//            seekedColumn.PrimaryKeyMember = true;
					//        }
					//    }
					//    catch (Exception) { }
					//}

                    //Recherche des identity column. stockage dans un dictionnaire <colonne,table>
                    Dictionary<string,string> identityColumns = new Dictionary<string,string>();
                    command = new SqlCommand(SELECT_ALL_IDENTITY_COLUMNS, connection);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            identityColumns.Add(reader.GetString(0), reader.GetString(1));
                        }

                    }

					// Primary key columns detection. Version ABO
					// Reinitialisation de l'attribut PK.
					foreach ( Column currentColumn in scannedTable.Columns )
					{
						currentColumn.PrimaryKeyMember = false;
					}
					SqlDataAdapter dataAdapterPKInfo = new SqlDataAdapter( string.Format( "exec sp_pkeys '{0}'", scannedTable.Name ), connection );
					DataSet dataSetPKInfo = new DataSet();
					dataAdapterPKInfo.Fill( dataSetPKInfo );
					foreach ( DataRow currentRow in dataSetPKInfo.Tables[0].Rows )
					{
						string columnName = currentRow["COLUMN_NAME"].ToString();
						Column seekedColumn = scannedTable.FindColumn( columnName );
						seekedColumn.PrimaryKeyMember = true;
                        if (identityColumns.ContainsKey(columnName) &&
                            scannedTable.Name.Equals(identityColumns[columnName]))
                        {
                            seekedColumn.IdentityColumn = true;
                        }
					}

				}
			}
			_generationDone = true;
			return;
		}

		/// <summary>
		/// Let's caller know wether the generator has been read from a file or not.
		/// </summary>
		/// <returns></returns>
		public bool IsFileBound()
		{
			return (_path != null);
		}

		private void ProduceCreateScript( TextWriter writer, ObjectScript script )
		{
			StringBuilder stringBuilder = null;

			if ( (script != null) && (!string.IsNullOrEmpty( script.CreateScript.Script )) )
			{
				if ( !string.IsNullOrEmpty( script.CreateScript.Comment ) )
				{
					stringBuilder = new StringBuilder( script.CreateScript.Comment );
					writer.WriteLine( stringBuilder.Replace( "\n", "\r\n" ).Replace( "\r\r\n", "\r\n" ) );
				}
				stringBuilder = new StringBuilder( script.CreateScript.Script );
				writer.WriteLine( stringBuilder.Replace( "\n", "\r\n" ).Replace( "\r\r\n", "\r\n" ) );
				writer.WriteLine( SQL_BATCH_COMMAND_SEPARATOR );
				writer.WriteLine();
			}
			return;
		}

		private void ProduceDropScript( TextWriter writer, ObjectScript script )
		{
			StringBuilder stringBuilder = null;

			if ( (script != null) && (!string.IsNullOrEmpty( script.DropScript.Script )) )
			{
				if ( !string.IsNullOrEmpty( script.DropScript.Comment ) )
				{
					stringBuilder = new StringBuilder( script.DropScript.Comment );
					writer.WriteLine( stringBuilder.Replace( "\n", "\r\n" ).Replace( "\r\r\n", "\r\n" ) );
				}
				stringBuilder = new StringBuilder( script.DropScript.Script );
				writer.WriteLine( stringBuilder.Replace( "\n", "\r\n" ).Replace( "\r\r\n", "\r\n" ) );
				writer.WriteLine( SQL_BATCH_COMMAND_SEPARATOR );
				writer.WriteLine();
			}
			return;
		}

		/// <summary>
		/// Produce a full SQL script.
		/// </summary>
		/// <param name="writer">
		/// Writer that will receive scripts items.
		/// </param>
		public void ProduceSqlScript( string p_fileName )
		{
			if ( string.IsNullOrEmpty( p_fileName ) )
			{ throw new Exception( "Incorrect file name." ); };
            // Supprimé par ABO
			//if ( !File.Exists( p_fileName ) )
			//{ throw new Exception( string.Format( "file name{0} does not exist.", p_fileName ) ); };

			// script général
			// --------------
			using ( TextWriter writer = new StreamWriter( p_fileName ) )
			{
				try { ProduceSqlScript( writer ); }
				catch ( Exception ) { return; }
			}

			// scripts détaillés
			// -----------------
			string scriptsPath = null;
			// root des scripts
			scriptsPath = p_fileName.Substring( 0, p_fileName.LastIndexOf( '\\' ) ) + "\\Scripts";
			// création du root
			try { Directory.Delete( scriptsPath, true ); }
			catch ( Exception ) { }
			Directory.CreateDirectory( scriptsPath );
			// génération des scripts
			ProduceSqlSubScripts( scriptsPath );
		}

		/// <summary>
		/// Produce a full SQL script.
		/// </summary>
		/// <param name="writer">
		/// Writer that will receive scripts items.
		/// </param>
		private void ProduceSqlScript( TextWriter writer )
		{
			// Generate drop scripts
			foreach ( Table scannedTable in _allMappings.AllTables )
			{
				if ( scannedTable.Obsolete )
				{ continue; }
				ProduceDropScript( writer, scannedTable.ViewScript );
				ProduceDropScript( writer, scannedTable.SelectProcedureScript );
				ProduceDropScript( writer, scannedTable.SelectByCodeProcedureScript );
				ProduceDropScript( writer, scannedTable.SelectListProcedureScript );
				ProduceDropScript( writer, scannedTable.InsertProcedureScript );
				ProduceDropScript( writer, scannedTable.UpdateProcedureScript );
				ProduceDropScript( writer, scannedTable.DeleteProcedureScript );
			}

			// Generate create scripts
			foreach ( Table scannedTable in _allMappings.AllTables )
			{
				if ( scannedTable.Obsolete )
				{ continue; }
				ProduceCreateScript( writer, scannedTable.ViewScript );
				ProduceCreateScript( writer, scannedTable.SelectProcedureScript );
				ProduceCreateScript( writer, scannedTable.SelectByCodeProcedureScript );
				ProduceCreateScript( writer, scannedTable.SelectListProcedureScript );
				ProduceCreateScript( writer, scannedTable.InsertProcedureScript );
				ProduceCreateScript( writer, scannedTable.UpdateProcedureScript );
				ProduceCreateScript( writer, scannedTable.DeleteProcedureScript );
			}
			return;
		}

		private void ProduceSqlSubScripts( string p_scriptPath )
		{
			if ( string.IsNullOrEmpty( p_scriptPath ) )
			{ throw new Exception( "Incorrect directory name." ); };

			foreach ( Table scannedTable in _allMappings.AllTables )
			{
				if ( scannedTable.Obsolete )
				{ continue; }
				ProduceSqlSubScript( p_scriptPath, scannedTable.ViewScript );
				ProduceSqlSubScript( p_scriptPath, scannedTable.SelectProcedureScript );
				ProduceSqlSubScript( p_scriptPath, scannedTable.SelectByCodeProcedureScript );
				ProduceSqlSubScript( p_scriptPath, scannedTable.SelectListProcedureScript );
				ProduceSqlSubScript( p_scriptPath, scannedTable.InsertProcedureScript );
				ProduceSqlSubScript( p_scriptPath, scannedTable.UpdateProcedureScript );
				ProduceSqlSubScript( p_scriptPath, scannedTable.DeleteProcedureScript );
			}
		}

		private void ProduceSqlSubScript( string p_scriptPath, ObjectScript p_objectScript )
		{
			if ( string.IsNullOrEmpty( p_scriptPath ) )
			{ throw new Exception( "Incorrect directory name." ); };
			if ( p_objectScript == null )
			{ return; }

			string fileName = string.Format( "{0}\\{1}.prc", p_scriptPath, p_objectScript.ObjectName );

			if ( (p_objectScript == null) || (string.IsNullOrEmpty( p_objectScript.CreateScript.Script )) )
			{ return; }

			using ( TextWriter writer = new StreamWriter( fileName ) )
			{
				try
				{
					ProduceDropScript( writer, p_objectScript );
					ProduceCreateScript( writer, p_objectScript );
				}
				catch ( Exception ) { return; }
			}
		}

		public void ProduceFiles( string path )
		{
			foreach ( Table currentTable in Tables )
			{
				if ( currentTable.DataClassSourceCodeCDL != null )
				{
					string targetFile = string.Format( CDL_FILE_PATH, path, currentTable.ClassName );
					FileInfo targetFileInfo = new FileInfo( targetFile );

					if ( !targetFileInfo.Directory.Exists ) { targetFileInfo.Directory.Create(); }
					using ( TextWriter writer = new StreamWriter( targetFile ) )
					{
						writer.Write( currentTable.DataClassSourceCodeCDL );
					}
				}
				if ( currentTable.DataClassSourceCodeDAL != null )
				{
					string targetFile = string.Format( DAL_FILE_PATH, path, currentTable.ClassName );
					FileInfo targetFileInfo = new FileInfo( targetFile );

					if ( !targetFileInfo.Directory.Exists ) { targetFileInfo.Directory.Create(); }
					using ( TextWriter writer = new StreamWriter( targetFile ) )
					{
						writer.Write( currentTable.DataClassSourceCodeDAL );
					}
				}
			}
		}

		/// <summary>
		/// Save the current generation in file.
		/// </summary>
		public void Save()
		{
			if ( _path == null ) { throw new NotSupportedException(); }
			using ( FileStream output = File.Open( _path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None ) )
			{
				output.SetLength( 0 );
				new XmlSerializer( typeof( Mappings ) ).Serialize( output, _allMappings );
			}
			return;
		}

		/// <summary>
		/// Save result into a new file.
		/// </summary>
		/// <param name="newPath">
		/// New file path.
		/// </param>
		public void Save( string newPath )
		{
			File.Create( newPath );
			_path = newPath;
			Save();
			return;
		}

		public void UpdateScripts()
		{
			UpdateScripts( true );
			return;
		}

		public void UpdateScripts( bool forceUpdate )
		{
			_allMappings.UpdateScripts( forceUpdate );
			return;
		}
		#endregion

		#region ATTRIBUTES
        private const string SELECT_ALL_IDENTITY_COLUMNS =
            "SELECT sys.identity_columns.name AS colname, sys.tables.name AS tabname "+
            "FROM sys.identity_columns, sys.tables "+
            "WHERE sys.identity_columns.object_id = sys.tables.object_id";
		private const string SQL_BATCH_COMMAND_SEPARATOR = "GO";
		private const string SELECT_ALL_TABLES_NAME =
            " SELECT [name] from [sysobjects] "+
            " WHERE Objectproperty(id,'IsMSShipped') = 0 and [type] = 'U' "+
            "  and [name] not like 'sysdiag%' "+
            " ORDER BY [name]";

		private const string SELECT_ALL_COLUMNS =
            "SELECT [syscolumns].[name], [syscolumns].[isnullable], type_name(xtype), [syscolumns].[length], [syscolumns].[prec], [syscolumns].[scale], [syscolumns].[autoval],  COLDESC.value"
            + " FROM [syscolumns] left outer join ::fn_listextendedproperty (null, 'user', 'dbo', 'table','{0}','column',default) COLDESC"
            + " on COLDESC.[objname]COLLATE Latin1_General_CI_AS =  [syscolumns].[name] and COLDESC.name = 'MS_Description'"
            + " WHERE [syscolumns].[id] = object_id('{0}')";
		//"SELECT [syscolumns].[name], [syscolumns].[isnullable], [systypes].[name], [syscolumns].[length], [syscolumns].[prec], [syscolumns].[scale] "
		//+ "FROM [syscolumns], [systypes] "
		//+ "WHERE [syscolumns].[id] = object_id('{0}')"
		//+ "AND [syscolumns].[xtype] = [systypes].[xtype]";

		private const string SELECT_PK_COLUMNS =
            "SELECT COL_NAME(OBJECT_ID('{0}'), colid), [status] FROM [sysconstraints] WHERE [id] = OBJECT_ID('{0}')";

		private const string CDL_FILE_PATH =
            "{0}\\CDL\\VO_{1}_auto.cs";

		private const string DAL_FILE_PATH =
            "{0}\\DAL\\DA_{1}_auto.cs";

		private Mappings _allMappings = null;

		private bool _generationDone = false;

       // private List<string> _identityColumn;
		/// <summary>
		/// Path for file where to store the generator results. May be null.
		/// </summary>
		private string _path;
		#endregion
	}
}
