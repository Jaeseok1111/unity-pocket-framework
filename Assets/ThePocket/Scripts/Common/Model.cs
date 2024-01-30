using System;

namespace ThePocket
{
    public enum ModelUsageTargets
    {
        GameChart,
        GameData,
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModelAttribute : Attribute
    {
        public string Name { get; set; }
        public ModelUsageTargets Usage { get; set; }
    }

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