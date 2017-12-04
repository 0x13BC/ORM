using System;
using System.Xml;
using System.Xml.Serialization;

namespace com.castsoftware.tools
{
	/// <summary>
	/// Cette classe décrit une colonne d'une <see cref="Table"/>
	/// </summary>
	public class Column
	{
		#region CONSTRUCTORS
		public Column()
		{
			return;
		}

		public Column( string name )
		{
			_name = name;            
			_attributeName = toCamel(name);
			return;
		}

		#endregion

		#region PROPERTIES
		[XmlAttribute( "attributeName" )]
		public string AttributeName
		{
			get { return _attributeName; }
			set { _attributeName = value; }
		}

		[XmlAttribute( "code" )]
		public bool Code
		{
			get { return _code; }
			set { _code = value; }
		}

		[XmlAttribute( "columnName" )]
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

		[XmlAttribute( "description" )]
		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		[XmlAttribute( "identity" )]
		public bool IdentityColumn
		{
			get { return _identityColumn; }
			set { _identityColumn = value; }
		}

		[XmlAttribute( "nullable" )]
		public bool Nullable
		{
			get { return _nullable; }
			set { _nullable = value; }
		}

		[XmlAttribute( "primaryKeyMember" )]
		public bool PrimaryKeyMember
		{
			get { return _primaryKeyMember; }
			set { _primaryKeyMember = value; }
		}

		[XmlIgnore()]
		public Table Owner
		{
			get { return _owner; }
			set { _owner = value; }
		}

		[XmlAttribute( "sqlLength" )]
		public int SqlLength
		{
			get { return _sqlLength; }
			set { _sqlLength = value; }
		}

		[XmlAttribute( "sqlPrecision" )]
		public int SqlPrecision
		{
			get { return _sqlPrecision; }
			set { _sqlPrecision = value; }
		}

		[XmlAttribute( "sqlScale" )]
		public int SqlScale
		{
			get { return _sqlScale; }
			set { _sqlScale = value; }
		}

		[XmlAttribute( "sqlType" )]
		public string SqlType
		{
			get { return _sqlType; }
			set { _sqlType = value; }
		}
		#endregion

		#region METHODS
		/// <summary>
		/// Retourne le type SQL correspondant au type de la colonne.
		/// </summary>
		/// <returns>
		/// </returns>
		public string GetSqlTypeDefinition()
		{
			string realType = _sqlType;

			switch ( _sqlType )
			{
				case "decimal":
				case "numeric":
					return realType + "(" + _sqlScale.ToString() + "," + _sqlPrecision.ToString() + ")";
				case "nchar":
				case "nvarchar":
                //					realType = "char";
                //					goto case "char";
				case "char":
				case "varbinary":
				case "varchar":
					return realType + "(" + _sqlLength.ToString() + ")";
				default:
					return realType;
			}
		}

		/// <summary>
		/// Incrémente le numéro de génération de base de données.
		/// </summary>
		private void IncrementDbGeneration()
		{
			if ( _owner != null ) { _owner.IncrementDbGeneration(); }
			return;
		}

        public static string toCamel(string p_input)
        {            
            return Table.toCamel(p_input);
        }
		#endregion

		#region ATTRIBUTES
		private string _attributeName;
		/// <summary>
		/// Indique si la colonne a valeur de code pour accéder aux enregistrements
		/// de la table à laquelle appartient cette colonne.
		/// </summary>
		private bool _code = false;
		private string _description = "";
		private bool _identityColumn;
		private string _name;
		private bool _nullable;
		/// <summary>
		/// Table à laquelle appartient cette colonne.
		/// </summary>
		private Table _owner;
		private bool _primaryKeyMember;
		private int _sqlLength;
		private int _sqlPrecision;
		private int _sqlScale;
		private string _sqlType;
		#endregion
	}
}
