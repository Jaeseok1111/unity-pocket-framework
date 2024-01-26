using Internal;
using UnityEngine;
using Zenject;

public class SceneInitializer : IInitializable
{
    protected DiContainer _container;

    private EventListener _listener = new();
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

    public void Listen(string name, global::System.Action listener)
    {
        _listener.Listen(name, listener);
    }

    public void LoadScene(string sceneName, params SceneJob[] jobs)
    {
        _sceneManager.LoadSceneAsync(sceneName, jobs);
    }

    public void SendEvent(SendEventSignal signal)
    {
        _listener.Invoke(signal.Name);
    }
}