using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace com.castsoftware.tools
{
	public class ObjectScript
	{
		#region CONSTRUCTORS
		public ObjectScript()
		{
			return;
		}
		#endregion

		#region PROPERTIES

		[XmlAttribute( "dbGeneration" )]
		public int DbGeneration
		{
			get { return _dbGeneration; }
			set { _dbGeneration = value; }
		}

		[XmlAttribute( "objectName" )]
		public string ObjectName
		{
			get { return _objectName; }
			set { _objectName = value; }
		}

		[XmlElement( "createScript" )]
		public SqlScript CreateScript
		{
			get
			{
				if (_createScript == null) { _createScript = new SqlScript(); }
				return _createScript;
			}
			set { _createScript = value; }
		}

		[XmlElement("dropScript")]
		public SqlScript DropScript
		{
			get
			{
				if (_dropScript == null) { _dropScript = new SqlScript(); }
				return _dropScript;
			}
			set { _dropScript = value; }
		}
		#endregion

		#region ATTRIBUTES
		private int _dbGeneration;
		private string _objectName;
		private SqlScript _createScript;
		private SqlScript _dropScript;
		#endregion
	}
}
