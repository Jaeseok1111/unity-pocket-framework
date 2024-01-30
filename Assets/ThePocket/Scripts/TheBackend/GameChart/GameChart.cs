namespace ThePocket
{
    public interface IGameChart
    {
    }

    public interface IGameChartForAutoGeneration
    {
        public string GetName();
        public void SaveToLocal(LitJson.JsonData gameChartJson);
    }
}