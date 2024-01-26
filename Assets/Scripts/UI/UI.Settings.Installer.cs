using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "UI Settings", menuName = "Project Settings/UI Settings")]
public class UISettingsInstaller : ScriptableObjectInstaller
{
    [Header("Loading")]
    [SerializeField] private string _loadingWithBar_Identifier;
    [SerializeField] private UILoadingBar _loadingWithBar;

    [Header("Logger")]
    [SerializeField] private GameObject _ok;
    [SerializeField] private GameObject _warning;
    [SerializeField] private GameObject _error;

    public override void InstallBindings()
    {
        Container
            .Bind<string>()
            .WithId("LoadingWithBar_Identifier")
            .FromInstance(_loadingWithBar_Identifier)
            .AsCached();

        Container
            .Bind<UILoadingBar>()
            .WithId("LoadingWithBar")
            .FromInstance(_loadingWithBar)
            .AsCached();

        Container
            .Bind<GameObject>()
            .WithId("LogOk")
            .FromInstance(_ok)
            .AsCached();

        Container
            .Bind<GameObject>()
            .WithId("LogWarning")
            .FromInstance(_warning)
            .AsCached();

        Container
            .Bind<GameObject>()
            .WithId("LogError")
            .FromInstance(_error)
            .AsCached();
    }
}