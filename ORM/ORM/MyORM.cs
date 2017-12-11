using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
    public class MyORM
    {
        public MyORM ()
        {
            
        }
        public void Home()
        {

            while (true)
            {
                afficherMenu();
                Console.WriteLine("Votre choix :");
                string choix = Console.ReadLine();
                switch (choix.ToLower())
                {
                    case "1":
                        SQLServer();
                        break;
                    case "2":
                        MySQL();
                        break;
                    case "3":
                        PostGreSQL();
                        break;
                    case "q":
                        return;
                    default:
                        Console.WriteLine("Erreur dans le choix");
                        break;
                }
            }

        }


        private static void SQLServer()
        {
            MServerSql SQLServ= new MServerSql();

        }



        private static void MySQL()
        {

        }

        private static void PostGreSQL()
        {

        }

        private static void afficherMenu()
        {
            Console.WriteLine("-- MENU --");
            Console.WriteLine("1- SQLServer");
            Console.WriteLine("2- MySQL");
            Console.WriteLine("3- PostGreSQL");
            Console.WriteLine("Q- Quitter");
        }
    }
}
