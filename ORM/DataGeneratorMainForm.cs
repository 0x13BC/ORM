using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace com.castsoftware.tools
{
	public partial class DataGeneratorMainForm : Form
	{
		#region CONSTRUCTORS
		public DataGeneratorMainForm()
		{
			InitializeComponent();
			return;
		}

		private void DataGeneratorMainForm_Load( object sender, EventArgs e )
		{
			_fileGenerator = new XmlFileGenerator();
			_fileGenerator.DbGenerationUpdate += new XmlFileGenerator.DbGenerationUpdateDelegate( DbGenerationUpdated );
			connectionStringAttemptTimer.Tick += new EventHandler( BuildConnectionString );
			connectionStringAttemptTimer.Interval = 400;

            DescriptorOpened();
			return;
		}

		private void DbGenerationUpdated()
		{
			updateScriptsButton.Enabled = true;
			return;
		}
		#endregion

		#region METHODS
		/// <summary>
		/// Attempt to build a connection string from input boxes texts.
		/// On success this will enable the generate button.
		/// </summary>
		/// <param name="sender">
		/// Unused
		/// </param>
		/// <param name="e">
		/// Unused
		/// </param>
		private void BuildConnectionString( object sender, EventArgs e )
		{
			string connectionString;

			connectionString = string.Format( TEMPLATE_CONNECTION_STRING,
				serverName.Text, databaseName.Text );
			using ( SqlConnection connection = new SqlConnection( connectionString ) )
			{
				try
				{
					connection.Open();
					_connectionString = connectionString;
					generateButton.Enabled = true;
					generateButton.Focus();
				}
				catch ( Exception ) { }
			}
			connectionStringAttemptTimer.Stop();
			return;
		}

		private string BuildFileChooserFilter()
		{
			return string.Format( "Data descriptor(*{0})|*{0}|All files(*.*)|*.*",
				DEFAULT_DESCRIPTOR_EXTENSION );
		}

		private string BuildScriptFileChooserFilter()
		{
			return string.Format( "Sql script(*{0})|*{0}|All files(*.*)|*.*",
				DEFAULT_SCRIPT_EXTENSION );
		}

		/// <summary>
		/// Occurs each time a descriptor is closed.
		/// </summary>
		private void DescriptorClosed()
		{
			serverName.Enabled = false;
			databaseName.Enabled = false;
			generateButton.Enabled = false;
			saveToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Enabled = false;
			tablesGrid.Visible = false;
			columnsGrid.Visible = false;
			return;
		}

		/// <summary>
		/// Occurs each time a descriptor is opened from a file or a new descriptor
		/// is created.
		/// </summary>
		private void DescriptorOpened()
		{
			serverName.Text = _fileGenerator.ServerName;
			databaseName.Text = _fileGenerator.DatabaseName;
			if ( _fileGenerator.IsFileBound() )
			{
				serverName.Enabled = false;
				databaseName.Enabled = false;
				updateScriptsButton.Enabled = !_fileGenerator.AreDbScriptsUpToDate();
			}
			else { serverName.Focus(); }
			generateButton.Enabled = (_fileGenerator.IsFileBound());

			saveToolStripMenuItem.Enabled = true;
			saveAsToolStripMenuItem.Enabled = true;
			ShowTables();
			return;
		}

		/// <summary>
		/// Check for both server name and database name text box being filled.
		/// If so, prepare a connection string build attempt. The real attempt
		/// will occur after a short delay without additional input from the user.
		/// </summary>
		private void PrepareConnectionStringAttempt()
		{
			connectionStringAttemptTimer.Stop();
			if ( (serverName.Text == string.Empty) || (databaseName.Text == string.Empty) )
			{
				connectionStringAttemptTimer.Enabled = false;
				return;
			}
			connectionStringAttemptTimer.Enabled = true;
			connectionStringAttemptTimer.Start();
			return;
		}

		private void ShowTables()
		{
			if ( !_fileGenerator.GenerationDone )
			{
				tablesGrid.Visible = false;
				columnsGrid.Visible = false;
                llbCheckAll.Visible = false;
				return;
			}
			_tablesBinding = new BindingSource();
			_tablesBinding.DataSource = _fileGenerator.Tables;
			tablesGrid.DataSource = _tablesBinding;
			foreach ( DataGridViewColumn scannedColumn in tablesGrid.Columns )
			{
				scannedColumn.Visible = false;
			}
			DataGridViewColumn visibleColumn;
			int currentDisplayIndex = 0;

			visibleColumn = tablesGrid.Columns["Name"];
			visibleColumn.DisplayIndex = currentDisplayIndex++;
			visibleColumn.ReadOnly = true;
			//visibleColumn.Width = 80;
			visibleColumn.Visible = true;

			visibleColumn = tablesGrid.Columns["ClassName"];
			visibleColumn.DisplayIndex = currentDisplayIndex++;
			//visibleColumn.Width = 260;
			visibleColumn.Visible = true;

			visibleColumn = tablesGrid.Columns["Obsolete"];
			visibleColumn.DisplayIndex = currentDisplayIndex++;
			visibleColumn.Width = 55;
			visibleColumn.Visible = true;

			tablesGrid.CellValueChanged += new DataGridViewCellEventHandler( tablesGrid_CellValueChanged );
			tablesGrid.SelectionChanged += new EventHandler( tablesGrid_SelectionChanged );
			tablesGrid.CurrentCell = tablesGrid["ClassName", 0];

			ShowColumns();

			tablesGrid.Visible = true;
			columnsGrid.Visible = true;
            llbCheckAll.Visible = true;
			return;
		}

		private void ShowColumns()
		{
			_columnsBinding = new BindingSource();
			columnsGrid.DataSource = _columnsBinding;

			// la selection provoque l'affichage des colonnes
			tablesGrid_SelectionChanged( null, null );
		}

		void tablesGrid_SelectionChanged( object sender, EventArgs e )
		{
			if ( tablesGrid.CurrentCell == null )
			{ return; }
			_columnsBinding.DataSource = ((Table)tablesGrid.CurrentCell.OwningRow.DataBoundItem).Columns;

			// affichage des 4 colonnes !
			foreach ( DataGridViewColumn scannedColumn in columnsGrid.Columns )
			{
				scannedColumn.Visible = false;
			}
			DataGridViewColumn visibleColumn;
			int currentDisplayIndex = 0;

			visibleColumn = columnsGrid.Columns["Name"];
			visibleColumn.DisplayIndex = currentDisplayIndex++;
			visibleColumn.ReadOnly = true;
			visibleColumn.Width = 150;
			visibleColumn.Visible = true;

			visibleColumn = columnsGrid.Columns["PrimaryKeyMember"];
			visibleColumn.HeaderText = "PK";
			visibleColumn.DisplayIndex = currentDisplayIndex++;
			visibleColumn.ReadOnly = true;
			visibleColumn.Width = 40;
			visibleColumn.Visible = true;

			visibleColumn = columnsGrid.Columns["IdentityColumn"];
			visibleColumn.HeaderText = "Identity";
			visibleColumn.DisplayIndex = currentDisplayIndex++;
			visibleColumn.ReadOnly = true;
			visibleColumn.Width = 40;
			visibleColumn.Visible = true;

			visibleColumn = columnsGrid.Columns["AttributeName"];
			visibleColumn.DisplayIndex = currentDisplayIndex++;
			visibleColumn.Width = 130;
			visibleColumn.Visible = true;

            visibleColumn = columnsGrid.Columns["Description"];
            visibleColumn.DisplayIndex = currentDisplayIndex++;
            visibleColumn.Width = 400;
            visibleColumn.Visible = true;
			columnsGrid.Visible = true;
			return;
		}

		void tablesGrid_CellValueChanged( object sender, DataGridViewCellEventArgs e )
		{
			DataGridViewCell targetCell = tablesGrid[e.ColumnIndex, e.RowIndex];
			Table targetTable = (Table)targetCell.OwningRow.DataBoundItem;
			string changedPropertyName = targetCell.OwningColumn.Name;

			object cellValue = targetCell.Value;
			switch ( changedPropertyName )
			{
				case "ClassName":
					targetTable.ClassName = (string)cellValue;
					break;
				case "Obsolete":
					targetTable.Obsolete = (bool)cellValue;
					break;
				default:
					break;
			}
			return;
		}
		#endregion

		#region Events management
		/// <summary>
		/// Database name text box content has been changed. Schedule a 
		/// new connection string build attempt.
		/// </summary>
		/// <param name="sender">
		/// Unused
		/// </param>
		/// <param name="e">
		/// Unused
		/// </param>
		private void databaseName_TextChanged( object sender, EventArgs e )
		{
			PrepareConnectionStringAttempt();
			return;
		}

		/// <summary>
		/// Triggers a new generation.
		/// </summary>
		/// <param name="sender">
		/// Unused
		/// </param>
		/// <param name="e">
		/// Unused
		/// </param>
		private void generateButton_Click( object sender, EventArgs e )
        {
            NewMethod();
            return;
        }

        private void NewMethod()
        {
            try
            {
                _fileGenerator.ServerName = serverName.Text;
                _fileGenerator.DatabaseName = databaseName.Text;
                _fileGenerator.Generate(_connectionString);
                // On succes we don't want anymore the user to be able to
                // change either the server or database name.
                serverName.Enabled = false;
                databaseName.Enabled = false;
                ShowTables();
                // la ligne suivante devrait être supprimée

                updateScriptsButton.Enabled = true;
                btVODALFiles.Enabled = false;
                btProduceScriptSQL.Enabled = false;
                produceCodeFilesToolStripMenuItem.Enabled = false;
                produceScriptFileToolStripMenuItem.Enabled = false;

            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// Création d'un nouveau fichier descripteur.
        /// </summary>
        /// <param name="sender">
        /// Inutilisé
        /// </param>
        /// <param name="e">
        /// Inutilisé
        /// </param>
        private void newDescriptorFileToolStripMenuItem_Click( object sender, EventArgs e )
		{
			serverName.Text = string.Empty;
			serverName.Enabled = true;
			serverName.TabStop = true;

			databaseName.Text = string.Empty;
			databaseName.Enabled = true;
			databaseName.TabStop = true;
			return;
		}

		/// <summary>
		/// Choix d'un fichier descripteur déjà existant.
		/// </summary>
		/// <param name="sender">
		/// Inutilisé.
		/// </param>
		/// <param name="e">
		/// Inutilisé.
		/// </param>
		private void openDescriptorFileToolStripMenuItem_Click( object sender, EventArgs e )
		{
			FileDialog fileChooser = new OpenFileDialog();

			fileChooser.Filter = BuildFileChooserFilter();
			fileChooser.FilterIndex = 1;
			fileChooser.AddExtension = true;
			fileChooser.CheckFileExists = true;
			fileChooser.DereferenceLinks = true;
			fileChooser.InitialDirectory = Environment.CurrentDirectory;
			fileChooser.ShowHelp = true;
			fileChooser.Title = "Data generator file chooser";
			fileChooser.ValidateNames = true;
			if ( fileChooser.ShowDialog() != DialogResult.OK ) { return; }
			try
			{
				_fileGenerator = new XmlFileGenerator( fileChooser.FileName );
				produceScriptFileToolStripMenuItem.Enabled = true;
				_fileGenerator.DbGenerationUpdate += new XmlFileGenerator.DbGenerationUpdateDelegate( DbGenerationUpdated );
			}
			catch ( Exception )
			{
				// Something was wrong opening the file.
				return;
			}
			DescriptorOpened();
			return;
		}

		private void produceScriptFileToolStripMenuItem_Click( object sender, EventArgs e )
		{
			FileDialog fileChooser = new SaveFileDialog();

			fileChooser.Filter = BuildScriptFileChooserFilter();
			fileChooser.FilterIndex = 1;
			fileChooser.AddExtension = true;
			fileChooser.DefaultExt = DEFAULT_SCRIPT_EXTENSION;
			fileChooser.DereferenceLinks = true;
			fileChooser.InitialDirectory = Environment.CurrentDirectory;
			fileChooser.ShowHelp = true;
			fileChooser.Title = "SQL script file chooser";
			fileChooser.ValidateNames = true;
			if ( fileChooser.ShowDialog() != DialogResult.OK )
			{ return; }
			try { _fileGenerator.ProduceSqlScript( fileChooser.FileName ); }
			catch ( Exception ) { return; }
			return;
		}

		private void ProduceFilesToolStripMenuItem_Click( object sender, EventArgs e )
		{
			FolderBrowserDialog folderChooser = new FolderBrowserDialog();
			folderChooser.SelectedPath = Environment.CurrentDirectory;
			if ( folderChooser.ShowDialog() != DialogResult.OK ) { return; }
			try
			{
				_fileGenerator.ProduceFiles( folderChooser.SelectedPath );
			}
			catch ( Exception ) { return; }
			return;
		}

		/// <summary>
		/// Occurs each time File / Save menu item has been clicked.
		/// </summary>
		/// <param name="sender">
		/// Unused.
		/// </param>
		/// <param name="e">
		/// Unused.
		/// </param>
		private void saveToolStripMenuItem_Click( object sender, EventArgs e )
		{
			if ( (tablesGrid.CurrentCell != null)
				&& tablesGrid.CurrentCell.IsInEditMode )
			{
				tablesGrid.EndEdit();
			}
			if ( !_fileGenerator.IsFileBound() )
			{
				saveAsToolStripMenuItem_Click( sender, e );
			}
			else { _fileGenerator.Save(); }
			return;
		}

		private void saveAsToolStripMenuItem_Click( object sender, EventArgs e )
		{
			FileDialog fileChooser = new SaveFileDialog();

			fileChooser.Filter = BuildFileChooserFilter();
			fileChooser.FilterIndex = 1;
			fileChooser.AddExtension = true;
			fileChooser.DefaultExt = DEFAULT_DESCRIPTOR_EXTENSION;
			fileChooser.DereferenceLinks = true;
			fileChooser.InitialDirectory = Environment.CurrentDirectory;
			fileChooser.ShowHelp = true;
			fileChooser.Title = "Data generator file chooser";
			fileChooser.ValidateNames = true;
			if ( fileChooser.ShowDialog() != DialogResult.OK )
			{
				return;
			}
			string savedPath = _fileGenerator.Path;
			try
			{
				_fileGenerator.Path = fileChooser.FileName;
				_fileGenerator.Save();
			}
			catch ( Exception )
			{
				// Something was wrong opening the file.
				_fileGenerator.Path = savedPath;
				return;
			}
			return;
		}

		/// <summary>
		/// Server name text box content has been changed. Schedule a 
		/// new connection string build attempt.
		/// </summary>
		/// <param name="sender">
		/// Unused
		/// </param>
		/// <param name="e">
		/// Unused
		/// </param>
		private void serverName_TextChanged( object sender, EventArgs e )
		{
			PrepareConnectionStringAttempt();
			return;
		}

		private void updateScriptsButton_Click( object sender, EventArgs e )
		{
			_fileGenerator.UpdateScripts();
			produceScriptFileToolStripMenuItem.Enabled = true;
			updateScriptsButton.Enabled = false;

            btVODALFiles.Enabled = true;
            btProduceScriptSQL.Enabled = true;
            produceCodeFilesToolStripMenuItem.Enabled = true;
            produceScriptFileToolStripMenuItem.Enabled = true;
			return;
		}
		#endregion

		#region ATTRIBUTES
		/// <summary>
		/// Default generator file extension.
		/// </summary>
		private const string DEFAULT_DESCRIPTOR_EXTENSION = ".dgf";
		private const string DEFAULT_SCRIPT_EXTENSION = ".sql";
		private BindingSource _columnsBinding;
		private string _connectionString = null;
		private Timer connectionStringAttemptTimer = new Timer();
		/// <summary>
		/// XML file generator.
		/// </summary>
		private XmlFileGenerator _fileGenerator = null;
		private BindingSource _tablesBinding;
		private const string TEMPLATE_CONNECTION_STRING =
			"Data Source={0};Initial Catalog={1};Integrated Security=SSPI;Connect Timeout=3";
		#endregion

        private void btVODALFiles_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderChooser = new FolderBrowserDialog();
            folderChooser.SelectedPath = Environment.CurrentDirectory;
            if (folderChooser.ShowDialog() != DialogResult.OK) { return; }
            try
            {
                _fileGenerator.ProduceFiles(folderChooser.SelectedPath);
            }
            catch (Exception) { return; }
            return;
        }

        private void btProduceScriptSQL_Click(object sender, EventArgs e)
        {
            FileDialog fileChooser = new SaveFileDialog();

            fileChooser.Filter = BuildScriptFileChooserFilter();
            fileChooser.FilterIndex = 1;
            fileChooser.AddExtension = true;
            fileChooser.DefaultExt = DEFAULT_SCRIPT_EXTENSION;
            fileChooser.DereferenceLinks = true;
            fileChooser.InitialDirectory = Environment.CurrentDirectory;
            fileChooser.ShowHelp = true;
            fileChooser.Title = "SQL script file chooser";
            fileChooser.ValidateNames = true;
            if (fileChooser.ShowDialog() != DialogResult.OK)
            { return; }
            try { _fileGenerator.ProduceSqlScript(fileChooser.FileName); }
            catch (Exception) { return; }
            return;
        }

        private void llbCheckAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            foreach (DataGridViewRow row in tablesGrid.Rows)
            {
                row.Cells["Obsolete"].Value = true;
            }
            tablesGrid.RefreshEdit();
        }
	}
}
