using Zenject;

public class SampleSceneInitializer : SceneInitializer
{
    public SampleSceneInitializer([Inject] DiContainer container)
        : base(container)
    {
    }
}