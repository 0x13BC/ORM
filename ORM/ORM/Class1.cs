using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;

namespace ORM
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface)]
	internal class Class1
		: Attribute
    {
        public bool ContinueOnCapturedContext { get; set; }
		public Method Method { get; set; }
		public bool ApplyToDescendents {get; set; }
		
		public test()
		  : this(default(Method))
		{
		}
	
		public test(Method method)
		{
			this.Method = method;
		}
    }
}
