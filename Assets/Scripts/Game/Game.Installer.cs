using Zenject;

namespace UnityFramework.Game
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Bypass.Install(Container);


        }
    }
}
