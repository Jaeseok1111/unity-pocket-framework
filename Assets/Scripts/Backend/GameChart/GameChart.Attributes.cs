using System;

/* ¿¹½Ã
    [GameChart(Name = "ItemData")]
    public partial class Item : IGameChart
    {
        [GameChartColumn(Name = "itemId")]
        public int ItemId;

        [GameChartColumn(Name = "name")]
        public string Name;
    }
*/

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
