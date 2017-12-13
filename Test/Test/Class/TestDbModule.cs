using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM;

namespace Test.Class
{
    class TestDbModule : DboConnectionModule
    {
        public override TypeSGBD GetDbType()
        {
            return new TypeSGBD();
        }

        public override string GetConnectionString()
        {
            return "xxxxx";
        }
    }
}
