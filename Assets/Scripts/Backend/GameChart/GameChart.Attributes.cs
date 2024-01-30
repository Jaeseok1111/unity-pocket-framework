using System;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class GameChartAttribute : Attribute
{
    public string Name { get; set; }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class GameChartColumnAttribute : Attribute
{
    public string Name { get; set; }
}
