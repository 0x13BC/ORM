using System;
using System.Text.RegularExpressions;

namespace ORM
{
    class Contact : IComparable<Contact>
    {
        private int id
        {
            get { return id; }
            set
            {
                if (id > 10000 && id < 99999) id = value;
                else;// Exception to do later
            }
        }
        private string name
        {
            get { return name; }
            set { name = value.ToUpper(); }
        }
        private string firstName;

        private string mail
        {
            get { return mail; }
            set
            {
                Regex regMail = new Regex(@"^[a-zA-Z0-9.-_]+@{1,1}[a-zA-Z0-9.-_]{2,}\.[a-zA-Z0-9.]{2,6}$");
                if (regMail.IsMatch(value)) mail = value;
                else; // THROW A EXCEPTION LATER
            }
        }

        private string address;

        private DateTime dateOfBirth;

        public int CompareTo(Contact other)
        {
            return this.id.CompareTo(other.id) != 0 ? this.id.CompareTo(other.id) : this.name.CompareTo(other.name);
        }

        public override string ToString()
        {
            return $"{this.id} {this.name} {this.firstName} {this.mail} {this.address} {this.dateOfBirth.ToString("dd/MM/yyyy")}";
        }
    }
}
