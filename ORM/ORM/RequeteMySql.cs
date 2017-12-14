using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM
{
    class RequeteMySql
    {
        private string create;

        private string select;

        private string update;

        private string delete;

        private string insert;

        public string createMySql(string nomtable, string[] proprietes, string[] types , string[] values)
        {
            create = $"create table {nomtable} ( {proprietes} {types} {values}  primary key ,";
            for (int i = 0; i < proprietes.Length; i++)
            {
                create += $" {proprietes[i]} {types[i]} {values[i]}";
                if (i != proprietes.Length - 1)
                    create += ",";
                
            }
            create += ")";
            Console.WriteLine(create);

            return create;
        }
        public string selectMySql(string nomtable, string[] proprietes ,string[] values, string[] whereprop, string[] whereval, string order)
        {

            select = "select";
            if (proprietes.Length != 0)
            {
                for (int i = 0; i < proprietes.Length; i++)
                {
                    select += $" {proprietes[i]}";
                    if (i != proprietes.Length - 1)
                        select += ",";

                }
            }
            else
            {
                select += $" *";
            }
            select += $" from {nomtable}";
            if (whereprop.Length != 0)
            {
                select += " where";
                for (int i = 0; i < whereprop.Length; i++)
                {
                    select += $" {whereprop[i]} = {whereval[i]}";
                    if (i != whereprop.Length - 1)
                        select += ",";

                }
            }
            if (order != "")
            {
                select += $"order by {order}";
            }
            Console.WriteLine(select);

            return select;
        }

        public string updateMySql(string nomtable, string[] proprietes, string[] values, string[] whereprop, string[] whereval ,string order)
        {

            update = $"update {nomtable}";
            for (int i = 0; i < proprietes.Length; i++)
            {
                update += $" {proprietes[i]} = {values[i]}";
                if (i != proprietes.Length - 1)
                    update += ",";

            }
            update +="where" ;
            for (int i = 0; i < whereprop.Length; i++)
            {
                update += $" {whereprop[i]} = {whereval[i]}";
                if (i != whereprop.Length - 1)
                    update += ",";

            }
            if(order != "")
            {
                update += $"order by {order}";
            }
            Console.WriteLine(update);

            return update;

        }
        public string deleteMySql(string nomtable, string[] whereprop, string[] whereval,string order)
        {

            delete = $"delete from {nomtable} where";
            for (int i = 0; i < whereprop.Length; i++)
            {
                delete += $" {whereprop[i]} = {whereval[i]}";
                if (i != whereprop.Length - 1)
                    delete += ",";

            }
            if (order != "")
            {
                delete += $"order by {order}";
            }
            Console.WriteLine(delete);

            return delete;
        }
        public string insertMySql(string nomtable, string[] proprietes, string[] values)
        {

            insert = $"insert into{nomtable}";
            for (int i = 0; i < proprietes.Length; i++)
            {
                select += $" {proprietes[i]}";
                if (i != proprietes.Length - 1)
                    select += ",";

            }
            insert = $"values";
            for (int i = 0; i < values.Length; i++)
            {
                insert += $" {values[i]}";
                if (i != values.Length - 1)
                    insert += ",";

            }
            Console.WriteLine(insert);

            return insert;
        }
    }
}
