using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ThePocket
{
    public class SceneInitializer : IInitializable
    {
        protected DiContainer _container;

        private Dictionary<string, System.Action> _eventCallback = new();
        private SceneManager _sceneManager;

        public SceneInitializer([Inject] DiContainer container)
        {
            _container = container;
        }

        public virtual void Initialize()
        {
            GameObject root = _container.Resolve<Context>().gameObject;

            _sceneManager = _container.InstantiateComponent<SceneManager>(root);
            _container
                .BindInterfacesAndSelfTo<SceneManager>()
                .FromInstance(_sceneManager)
                .AsSingle();
        }

        public void Listen(string name, System.Action listener)
        {
            if (_eventCallback.ContainsKey(name))
            {
                return;
            }

            _eventCallback[name] = listener;
        }

        public void LoadScene(string sceneName, params SceneJob[] jobs)
        {
            _sceneManager.LoadSceneAsync(sceneName, jobs);
        }

        public void SendEvent(SendEventSignal signal)
        {
            if (_eventCallback.ContainsKey(signal.Name) == false)
            {
                return;
            }

            _eventCallback[signal.Name].Invoke();
        }
    }
}