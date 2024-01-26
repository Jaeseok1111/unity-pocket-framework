using System;

/* ¿¹½Ã 
    [GameData(Name = "UserData")]
    public partial class UserData : IGameData
    {
        [GameDataColumn(Name = "name")]
        public string Name;
    }
*/

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