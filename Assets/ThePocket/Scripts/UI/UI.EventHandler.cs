using UnityEngine;
using Zenject;

namespace ThePocket
{
    public class UIEventHandler : MonoBehaviour
    {
        private DiContainer _container;
        private SignalBus _signalBus;
        private UIPanel _panel;

        [Inject]
        public void Construct(
            DiContainer container,
            SignalBus signalBus,
            UIPanel panel)
        {
            _container = container;
            _signalBus = signalBus;
            _panel = panel;
        }

        public void Push(GameObject panel)
        {
            GameObject instance = _container.InstantiatePrefab(panel);

            _panel.Push(instance);
        }

        public void Popup(GameObject panel)
        {
            GameObject instance = _container.InstantiatePrefab(panel);

            _panel.Popup(instance);
        }

        public void Back()
        {
            _panel.Back();
        }

        public void SendEvent(string name)
        {
            _signalBus.Fire(new SendEventSignal() { Name = name });
        }
    }
}