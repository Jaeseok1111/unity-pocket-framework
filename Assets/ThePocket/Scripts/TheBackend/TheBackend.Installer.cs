using BackEnd;
using UnityEngine;
using Zenject;

namespace ThePocket
{
    public class TheBackendInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            if (Initialize() == false)
            {
                Debug.LogError("Error: Initialize()");
            }

            Debug.Log("Start Server...");
        }

        private bool Initialize()
        {
            Debug.Log("Try: Initialize()");

            var backend = Backend.Initialize(useAsyncPoll: true);

            if (backend.IsSuccess())
            {
                Debug.Log("Success: Initialize()");
                return true;
            }
            else
            {
                Debug.Log($"Failed: Initialize(). {backend}");
                return false;
            }
        }
    }
}