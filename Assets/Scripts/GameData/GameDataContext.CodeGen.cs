#if UNITY_EDITOR

using ThridParty;

public class GameDataContextCodeGen : CodeGenerator
{
    public override string FolderPath => "Assets/Scripts/GameData";
    public override string Name => "GameDataContext.Generated.cs";

    protected override void Generate()
    {
    }
}

#endif