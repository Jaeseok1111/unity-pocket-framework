using ThePocket;

public class SampleSceneInstaller : SceneInstaller<SampleSceneInitializer>
{
    public override void InstallBindings()
    {
        base.InstallBindings();

        Container
            .BindInterfacesAndSelfTo<GameDataContext>()
            .AsSingle();
    }
}