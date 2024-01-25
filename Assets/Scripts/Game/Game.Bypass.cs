using Zenject;

namespace UnityFramework.Game
{
    public class Bypass : Installer<Bypass>
    {
        public static Bypass Instance { get; private set; }

        public override void InstallBindings()
        {
            Instance = this;
        }

        public ConcreteIdBinderGeneric<TContract> Bind<TContract>()
        {
            return Container.Bind<TContract>();
        }
    }
}