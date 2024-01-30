using System;

namespace ThePocket.Utils.SQLite
{
    public abstract class TableScheme
    {
        public abstract string Name { get; protected set; }
        public abstract int Version { get; protected set; }
    }

    public class FieldScheme
    {
        public string Name { get; private set; }
        public Type Type { get; private set; }

        public FieldScheme(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
