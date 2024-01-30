using BackEnd;

namespace ThePocket
{
    public interface IGameData
    {
    }

    public interface IGameDataForAutoGeneration
    {
        public string GetName();
        public Param ToServer();
        public void ToLocal(LitJson.JsonData gameDataJson);
    }
}