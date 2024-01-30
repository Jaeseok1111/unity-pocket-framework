#if UNITY_EDITOR

using ThePocket.Utils;

namespace ThePocket
{
    public class GameDataContextCodeGen : CodeGenerator
    {
        public override string FolderPath => "Assets/ThePocket/Scripts/GameData";
        public override string Name => "GameDataContext.Generated.cs";

        protected override void Generate()
        {
        }
    }
}

#endif