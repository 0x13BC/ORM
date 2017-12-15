using System;

namespace ORM
{
    internal class ColumnAttribute : Attribute
    {
        public bool IsPrimaryKey { get; set; }
        public bool IsDbGenerated { get; set; }
    }
}