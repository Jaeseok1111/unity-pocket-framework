using System;

namespace ThridParty.SQLite
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class FieldAttribute : Attribute
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public object DefaultValue { get; set; }
        public bool PrimaryKey { get; set; }
        public bool NotNull { get; set; }
    }
}
