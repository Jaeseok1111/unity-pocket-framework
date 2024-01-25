using Zenject;

namespace UnityFramework
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
        }
    }
}
