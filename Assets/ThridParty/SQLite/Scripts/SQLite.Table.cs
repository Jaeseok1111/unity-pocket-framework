using System;
using System.Collections.Generic;

namespace ThridParty.SQLite
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }
    }

    public abstract class Table<T> where T : IRecord, new()
    {
        private readonly DatabaseContext _context;
        private readonly string _name;

        public Table(DatabaseContext context, string name)
        {
            _context = context;
            _name = name;
        }

        public Query<T> NewQuery()
        {
            return new Query<T>(_context, _name);
        }
    }
}