using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Text.RegularExpressions;

namespace ORM
{
    [Table (Name="Contacts")]
    public class Contact : IComparable<Contact>
    {

        public Contact(int id, string name, string firstname, string mail, string adress, DateTime dateofbirth)
        {
            this.Id = id;
            this.Name = name;
            this.FirstName = firstname;
            this.Mail = mail;
            this.Address = adress;
            this.DateOfBirth = dateofbirth;
        }

        private int _id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id
        {
            get { return _id; }
            set
            {
                if (value > 0 && value < 99999) _id = value;
                else { };//TODO Exception to do later
            }
        }
        private string _name;
        [Column]
        public string Name
        {
            get { return _name; }
            set { _name = value.ToUpper(); }
        }
       
        [Column]
        public string FirstName;

        private string _mail;
        [Column]
        public string Mail
        {
            get { return _mail; }
            set
            {
                Regex regMail = new Regex(@"^[a-zA-Z0-9.-_]+@{1,1}[a-zA-Z0-9.-_]{2,}\.[a-zA-Z0-9.]{2,6}$");
                if (regMail.IsMatch(value)) _mail = value;
                else { } //TODO THROW A EXCEPTION LATER
            }
        }

        [Column]
        public string Address { get; set; }

        [Column]
        public DateTime DateOfBirth { get; set; }


        public int CompareTo(Contact other)
        {
            return Id.CompareTo(other.Id) != 0 ? this.Id.CompareTo(other.Id) : this.Name.CompareTo(other.Name);
        }

        public override string ToString()
        {
            return $"{this.Id} {this.Name} {this.FirstName} {this.Mail} {this.Address} {this.DateOfBirth.ToString("dd/MM/yyyy")}";
        }
    }
}
