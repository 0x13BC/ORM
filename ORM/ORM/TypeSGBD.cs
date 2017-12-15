using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
   
        public class TypeSGBD
        {

            public TypeSGBD MYSQL { get; set; }
            public TypeSGBD POSTEGRESQL { get; set; } = null;
            public TypeSGBD SQLSERVER { get; set; } = null;
        }
}

