using System;

namespace ThePocket
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GameDataAttribute : Attribute
    {
        public string Name { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class GameDataColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}