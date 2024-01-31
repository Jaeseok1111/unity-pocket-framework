using Sample;
using ThePocket;

public class SampleSceneInstaller : SceneInstaller<SampleSceneInitializer>
{
    public override void InstallBindings()
    {
        base.InstallBindings();

        GameDataInstaller.Install(Container, (context) =>
        {
            context.AddTable<ItemData, ItemData.Table, ItemData.Scheme>();
        });
    }
}