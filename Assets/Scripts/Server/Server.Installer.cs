using BackEnd;
using UnityEngine;
using Zenject;

namespace UnityFramework
{
    public class ServerInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            if (InitializeServer() == false)
            {
                Debug.LogError("Error: InitializeServer()");
            }

            Debug.Log("Start Server...");
        }

        private bool InitializeServer()
        {
            Debug.Log("Try: InitializeServer()");

            var backend = Backend.Initialize(useAsyncPoll: true);

            if (backend.IsSuccess())
            {
                Debug.Log("Success: InitializeServer()");
                return true;
            }
            else
            {
                Debug.Log($"Failed: InitializeServer(). {backend}");
                return false;
            }
        }
    }
}
