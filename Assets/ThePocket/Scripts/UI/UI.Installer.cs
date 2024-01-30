using UnityEngine;
using Zenject;

namespace ThePocket
{
    public class UIInstaller : MonoInstaller
    {
        [Header("Canvas")]
        [SerializeField] private Canvas _main;
        [SerializeField] private Canvas _popup;

        [Header("Panel")]
        [SerializeField] private GameObject _root;

        public override void InstallBindings()
        {
            Container
                .Bind<Canvas>()
                .WithId("Main")
                .FromInstance(_main)
                .AsCached();

            Container
                .Bind<Canvas>()
                .WithId("Popup")
                .FromInstance(_popup)
                .AsCached();

            Container
                .Bind<UILogger>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<UIPanel>()
                .AsSingle()
                .WithArguments(_root);
        }
    }
}