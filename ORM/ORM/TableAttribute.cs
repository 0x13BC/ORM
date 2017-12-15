using System;

namespace ORM
{
    internal class TableAttribute : Attribute
    {
        public string Name { get; set; }
    }
}