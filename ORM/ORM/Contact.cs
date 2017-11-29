using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;

namespace ORM
{
    [Table (Name="Contacts")]
    public class Contact : IComparable<Contact>
    {
        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        private int id
        {
            get { return id; }
            set
            {
                if (id > 0 && id < 99999) id = value;
                else;//TODO Exception to do later
            }
        }
        [Column]
        private string name
        {
            get { return name; }
            set { name = value.ToUpper(); }
        }
        [Column]
        private string firstName;

        [Column]
        private string mail
        {
            get { return mail; }
            set
            {
                Regex regMail = new Regex(@"^[a-zA-Z0-9.-_]+@{1,1}[a-zA-Z0-9.-_]{2,}\.[a-zA-Z0-9.]{2,6}$");
                if (regMail.IsMatch(value)) mail = value;
                else; //TODO THROW A EXCEPTION LATER
            }
        }

        [Column]
        private string address;

        [Column]
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
