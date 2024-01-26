using UnityEngine;

public class UISafeArea : MonoBehaviour
{
    private RectTransform _panel;

    private Rect _lastSafeArea;
    private Vector2Int _lastScreenSize;
    private ScreenOrientation _lastScreenOrientation = ScreenOrientation.LandscapeLeft;

    private void Awake()
    {
        _panel = GetComponent<RectTransform>();

        Refresh();
    }

    private void LateUpdate()
    {
        Refresh();
    }

    private void Refresh()
    {
        Rect safeArea = Screen.safeArea;

        if (safeArea != _lastSafeArea
            || Screen.width != _lastScreenSize.x
            || Screen.height != _lastScreenSize.y
            || Screen.orientation != _lastScreenOrientation)
        {
            _lastScreenSize.x = Screen.width;
            _lastScreenSize.y = Screen.height;
            _lastScreenOrientation = Screen.orientation;

            Apply(safeArea);
        }
    }

    private void Apply(Rect safeArea)
    {
        _lastSafeArea = safeArea;

        if (Screen.width > 0 && Screen.height > 0)
        {
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            if (anchorMin.x >= 0 && anchorMin.y >= 0 && anchorMax.x >= 0 && anchorMax.y >= 0)
            {
                _panel.anchorMin = anchorMin;
                _panel.anchorMax = anchorMax;
            }
        }
    }
}