namespace com.castsoftware.tools
{
	partial class DataGeneratorMainForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.mainMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newDescriptorFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDescriptorFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.produceScriptFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.produceCodeFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.serverName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.databaseName = new System.Windows.Forms.TextBox();
            this.generateButton = new System.Windows.Forms.Button();
            this.tablesGrid = new System.Windows.Forms.DataGridView();
            this.columnsGrid = new System.Windows.Forms.DataGridView();
            this.updateScriptsButton = new System.Windows.Forms.Button();
            this.btVODALFiles = new System.Windows.Forms.Button();
            this.btProduceScriptSQL = new System.Windows.Forms.Button();
            this.llbCheckAll = new System.Windows.Forms.LinkLabel();
            this.mainMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnsGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu
            // 
            this.mainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.mainMenu.Location = new System.Drawing.Point(0, 0);
            this.mainMenu.Name = "mainMenu";
            this.mainMenu.Size = new System.Drawing.Size(918, 24);
            this.mainMenu.TabIndex = 0;
            this.mainMenu.Text = "&File";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newDescriptorFileToolStripMenuItem,
            this.openDescriptorFileToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newDescriptorFileToolStripMenuItem
            // 
            this.newDescriptorFileToolStripMenuItem.Name = "newDescriptorFileToolStripMenuItem";
            this.newDescriptorFileToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.newDescriptorFileToolStripMenuItem.Text = "&New descriptor file";
            this.newDescriptorFileToolStripMenuItem.Click += new System.EventHandler(this.newDescriptorFileToolStripMenuItem_Click);
            // 
            // openDescriptorFileToolStripMenuItem
            // 
            this.openDescriptorFileToolStripMenuItem.Name = "openDescriptorFileToolStripMenuItem";
            this.openDescriptorFileToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.openDescriptorFileToolStripMenuItem.Text = "&Open descriptor file ...";
            this.openDescriptorFileToolStripMenuItem.Click += new System.EventHandler(this.openDescriptorFileToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.saveAsToolStripMenuItem.Text = "&Save as ...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.produceScriptFileToolStripMenuItem,
            this.produceCodeFilesToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // produceScriptFileToolStripMenuItem
            // 
            this.produceScriptFileToolStripMenuItem.Enabled = false;
            this.produceScriptFileToolStripMenuItem.Name = "produceScriptFileToolStripMenuItem";
            this.produceScriptFileToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.produceScriptFileToolStripMenuItem.Text = "&Produce script file ...";
            this.produceScriptFileToolStripMenuItem.Click += new System.EventHandler(this.produceScriptFileToolStripMenuItem_Click);
            // 
            // produceCodeFilesToolStripMenuItem
            // 
            this.produceCodeFilesToolStripMenuItem.Enabled = false;
            this.produceCodeFilesToolStripMenuItem.Name = "produceCodeFilesToolStripMenuItem";
            this.produceCodeFilesToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.produceCodeFilesToolStripMenuItem.Text = "Produce CDL and DAL files";
            this.produceCodeFilesToolStripMenuItem.Click += new System.EventHandler(this.ProduceFilesToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Server";
            // 
            // serverName
            // 
            this.serverName.Location = new System.Drawing.Point(55, 39);
            this.serverName.Name = "serverName";
            this.serverName.Size = new System.Drawing.Size(175, 20);
            this.serverName.TabIndex = 2;
            this.serverName.TextChanged += new System.EventHandler(this.serverName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(249, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Database";
            // 
            // databaseName
            // 
            this.databaseName.Location = new System.Drawing.Point(308, 38);
            this.databaseName.Name = "databaseName";
            this.databaseName.Size = new System.Drawing.Size(173, 20);
            this.databaseName.TabIndex = 4;
            this.databaseName.TextChanged += new System.EventHandler(this.databaseName_TextChanged);
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(777, 39);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(129, 23);
            this.generateButton.TabIndex = 5;
            this.generateButton.Text = "&Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
            // 
            // tablesGrid
            // 
            this.tablesGrid.AllowUserToAddRows = false;
            this.tablesGrid.AllowUserToDeleteRows = false;
            this.tablesGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.tablesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tablesGrid.Location = new System.Drawing.Point(12, 68);
            this.tablesGrid.Name = "tablesGrid";
            this.tablesGrid.Size = new System.Drawing.Size(894, 334);
            this.tablesGrid.TabIndex = 6;
            // 
            // columnsGrid
            // 
            this.columnsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.columnsGrid.Location = new System.Drawing.Point(12, 426);
            this.columnsGrid.Name = "columnsGrid";
            this.columnsGrid.Size = new System.Drawing.Size(894, 236);
            this.columnsGrid.TabIndex = 7;
            // 
            // updateScriptsButton
            // 
            this.updateScriptsButton.Enabled = false;
            this.updateScriptsButton.Location = new System.Drawing.Point(12, 668);
            this.updateScriptsButton.Name = "updateScriptsButton";
            this.updateScriptsButton.Size = new System.Drawing.Size(121, 23);
            this.updateScriptsButton.TabIndex = 8;
            this.updateScriptsButton.Text = "1- Update Scripts";
            this.updateScriptsButton.UseVisualStyleBackColor = true;
            this.updateScriptsButton.Click += new System.EventHandler(this.updateScriptsButton_Click);
            // 
            // btVODALFiles
            // 
            this.btVODALFiles.Enabled = false;
            this.btVODALFiles.Location = new System.Drawing.Point(209, 668);
            this.btVODALFiles.Name = "btVODALFiles";
            this.btVODALFiles.Size = new System.Drawing.Size(136, 23);
            this.btVODALFiles.TabIndex = 9;
            this.btVODALFiles.Text = "2- Produce VO/DAL files";
            this.btVODALFiles.UseVisualStyleBackColor = true;
            this.btVODALFiles.Click += new System.EventHandler(this.btVODALFiles_Click);
            // 
            // btProduceScriptSQL
            // 
            this.btProduceScriptSQL.Enabled = false;
            this.btProduceScriptSQL.Location = new System.Drawing.Point(370, 668);
            this.btProduceScriptSQL.Name = "btProduceScriptSQL";
            this.btProduceScriptSQL.Size = new System.Drawing.Size(164, 23);
            this.btProduceScriptSQL.TabIndex = 10;
            this.btProduceScriptSQL.Text = "2- Produce Script SQL files";
            this.btProduceScriptSQL.UseVisualStyleBackColor = true;
            this.btProduceScriptSQL.Click += new System.EventHandler(this.btProduceScriptSQL_Click);
            // 
            // llbCheckAll
            // 
            this.llbCheckAll.AutoSize = true;
            this.llbCheckAll.Location = new System.Drawing.Point(812, 405);
            this.llbCheckAll.Name = "llbCheckAll";
            this.llbCheckAll.Size = new System.Drawing.Size(94, 13);
            this.llbCheckAll.TabIndex = 11;
            this.llbCheckAll.TabStop = true;
            this.llbCheckAll.Text = "Check all obsolete";
            this.llbCheckAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llbCheckAll_LinkClicked);
            // 
            // DataGeneratorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 703);
            this.Controls.Add(this.llbCheckAll);
            this.Controls.Add(this.btProduceScriptSQL);
            this.Controls.Add(this.btVODALFiles);
            this.Controls.Add(this.updateScriptsButton);
            this.Controls.Add(this.columnsGrid);
            this.Controls.Add(this.tablesGrid);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.databaseName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.serverName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.mainMenu);
            this.MainMenuStrip = this.mainMenu;
            this.Name = "DataGeneratorMainForm";
            this.Text = "Data layer generator";
            this.Load += new System.EventHandler(this.DataGeneratorMainForm_Load);
            this.mainMenu.ResumeLayout(false);
            this.mainMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.columnsGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip mainMenu;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem newDescriptorFileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openDescriptorFileToolStripMenuItem;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox serverName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox databaseName;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.Button generateButton;
		private System.Windows.Forms.DataGridView tablesGrid;
		private System.Windows.Forms.DataGridView columnsGrid;
		private System.Windows.Forms.Button updateScriptsButton;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem produceScriptFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem produceCodeFilesToolStripMenuItem;
        private System.Windows.Forms.Button btVODALFiles;
        private System.Windows.Forms.Button btProduceScriptSQL;
        private System.Windows.Forms.LinkLabel llbCheckAll;

	}
}

