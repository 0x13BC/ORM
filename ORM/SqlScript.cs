using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace com.castsoftware.tools
{
	public class SqlScript
	{
		public SqlScript()
		{
			return;
		}

		[XmlElement( "comment" )]
		public string Comment
		{
			get { return _comment; }
			set { _comment = value; }
		}

		[XmlElement( "script" )]
		public string Script
		{
			get { return _script; }
			set { _script = value; }
		}

		private string _comment;
		private string _script;
	}
}
