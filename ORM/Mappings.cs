using System;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

namespace com.castsoftware.tools
{
	[XmlRoot("mappings")]
	public class Mappings
	{
		public void AddTable(Table table)
		{
			if (!_tablesByName.ContainsKey(table.Name))
			{
				_tables.Add(table);
				_tablesByName.Add(table.Name, table);
				table.SetOwner(this);
			}
			return;
		}

		public bool AreDbScriptsUpToDate()
		{
			return _dbGeneration >= (_dbNextGeneration - 1);
		}

		[XmlArray("tables"), XmlArrayItem("table")]
		public Table[] AllTables
		{
			get { return (Table[])_tables.ToArray(typeof(Table)); }
			set
			{
				_tables.Clear();
				_tablesByName.Clear();
				_tables.AddRange(value);
				foreach (Table scannedTable in _tables)
				{
					scannedTable.SetOwner(this);
					_tablesByName.Add(scannedTable.Name, scannedTable);
					int candidateGeneration = scannedTable.DbGeneration;
					if (candidateGeneration >= _dbNextGeneration)
					{
						_dbNextGeneration = candidateGeneration + 1;
					}
				}
				return;
			}
		}

		[XmlAttribute("databaseName")]
		public string DatabaseName
		{
			get { return _databaseName; }
			set { _databaseName = value; }
		}

		[XmlAttribute("dbGeneration")]
		public int DbGeneration
		{
			get { return _dbGeneration; }
			set { _dbGeneration = value; }
		}

		[XmlIgnore()]
		public XmlFileGenerator Generator
		{
			get { return _generator; }
			set
			{
				if (_generator != null) { throw new NotSupportedException(); }
				_generator = value;
			}
		}

		[XmlAttribute("serverName")]
		public string ServerName
		{
			get { return _serverName; }
			set { _serverName = value; }
		}

		public Table FindTable(string tableName)
		{
			return (Table) _tablesByName[tableName];
		}

		public int GetNextDbGeneration()
		{
			_generator.DbGenerationUpdated();
			return _dbNextGeneration++;
		}

		public void UpdateScripts()
		{
			UpdateScripts(false);
		}

		public void UpdateScripts(bool forceUpdate)
		{
			foreach (Table scannedTable in AllTables)
			{
				scannedTable.UpdateScripts(forceUpdate);
			}
			_dbGeneration = _dbNextGeneration - 1;
			return;
		}

		private string _databaseName;
		private int _dbGeneration;
		private int _dbNextGeneration;
		private XmlFileGenerator _generator;
		private string _serverName;
		private Hashtable _tablesByName = new Hashtable();
		private ArrayList _tables = new ArrayList();
	}
}
