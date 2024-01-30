using Zenject;

namespace ThePocket
{
    public class SceneInstaller<TInitializer> : MonoInstaller
        where TInitializer : SceneInitializer
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<TInitializer>()
                .AsSingle();

            Container.DeclareSignal<SendEventSignal>();
            Container.BindSignal<SendEventSignal>()
                .ToMethod<TInitializer>(x => x.SendEvent)
                .FromResolve();
        }
    }
}