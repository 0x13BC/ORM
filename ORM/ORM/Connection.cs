﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Npgsql.PostgresTypes;
using NpgsqlTypes;
using System.Data;

namespace ORM
{
    public class Connection
    {

        private TypeSGBD DBused;
        private string ConnectionString;

        virtual public TypeSGBD GetDbType()
        {
            return DBused;
        }

        public Connection()
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
                        PostgreSql();
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


        private static void PostgreSql()
        {
            PostgreSql SQLServ = new PostgreSql();
            SQLServ.CreateUrl();
            SQLServ.connectionSqlServ();
            SQLServ.Disconnect();

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
        virtual public string GetConnectionString()
        {
            return ConnectionString;
        }
    }
}