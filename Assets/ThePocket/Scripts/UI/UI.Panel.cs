using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ThePocket
{
    public class UIPanel : IInitializable
    {
        private readonly Canvas _mainCanvas;
        private readonly Canvas _popupCanvas;
        private readonly DiContainer _container;

        private GameObject _rootPanel;
        private Stack<GameObject> _panels = new();

        public UIPanel(
            [Inject] DiContainer container,
            [Inject(Id = "Main")] Canvas mainCanvas,
            [Inject(Id = "Popup")] Canvas popupCanvas,
            [Inject] GameObject root)
        {
            _container = container;
            _mainCanvas = mainCanvas;
            _popupCanvas = popupCanvas;
            _rootPanel = root;
        }

        public void Initialize()
        {
            if (_rootPanel == null)
            {
                return;
            }

            GameObject root = _container.InstantiatePrefab(_rootPanel);
            Push(root);
        }

        public GameObject New(GameObject panel)
        {
            GameObject newPanel = _container.InstantiatePrefab(panel);

            Back();
            Push(newPanel);

            return newPanel;
        }

        public GameObject NewPopup(GameObject panel)
        {
            GameObject newPanel = _container.InstantiatePrefab(panel);

            Popup(newPanel);

            return newPanel;
        }

        public void Push(GameObject panel)
        {
            if (_panels.Count > 0)
            {
                SetLastPanelActive(false);
            }

            var instance = panel.GetComponent<RectTransform>();
            instance.transform.SetParent(_mainCanvas.transform);
            instance.offsetMin = Vector2.zero;
            instance.offsetMax = Vector2.zero;
            instance.transform.localScale = Vector3.one;
            instance.gameObject.SetActive(true);

            _panels.Push(instance.gameObject);
        }

        public void Popup(GameObject panel)
        {
            var instance = panel.GetComponent<RectTransform>();
            instance.transform.SetParent(_popupCanvas.transform);
            instance.offsetMin = Vector2.zero;
            instance.offsetMax = Vector2.zero;
            instance.transform.localScale = Vector3.one;
            instance.gameObject.SetActive(true);

            _panels.Push(instance.gameObject);
        }

        public void Back()
        {
            if (_panels.Count == 0)
            {
                return;
            }

            GameObject lastPanel = _panels.Pop();
            HideAndDestroy(lastPanel);

            if (_panels.Count == 0)
            {
                return;
            }

            SetLastPanelActive(true);
        }

        private void SetLastPanelActive(bool active)
        {
            GameObject panel = _panels.Pop();
            panel.SetActive(active);
            _panels.Push(panel);
        }

        private void HideAndDestroy(GameObject panel)
        {
            if (panel.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.blocksRaycasts = false;

                canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
                {
                    panel.SetActive(false);
                    GameObject.Destroy(panel, 1f);
                });
            }
        }
    }
}