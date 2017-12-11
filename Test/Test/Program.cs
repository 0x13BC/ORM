using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORM;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Contact essai = new Contact(100, "Jaja", "JOJO", "monmail@france.fr", "12 de saint terbancle", new DateTime(2001, 12, 10));
            Console.WriteLine(essai.ToString());
            Console.ReadKey();
            MyORM orm= new MyORM();
            orm.Home();
           

        }
    }
}
