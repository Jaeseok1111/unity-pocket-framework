using UnityFramework.Internal;
using Zenject;

namespace UnityFramework.Scenes
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