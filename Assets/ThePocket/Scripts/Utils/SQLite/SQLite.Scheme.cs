using System;

namespace ThePocket.Utils.SQLite
{
    public interface IScheme
    {
        public string Name { get; }
        public int Version { get; }
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
