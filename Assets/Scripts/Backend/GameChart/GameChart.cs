public interface IGameChart
{
}

public interface IGameChartForAutoGeneration
{
    public string GetName();
    public void ToLocal(LitJson.JsonData gameChartJson);
}