using UnityEngine;

public class UIPagination : MonoBehaviour
{
    [Header("View")]
    [SerializeField] private GameObject _prev;
    [SerializeField] private GameObject _selected;
    [SerializeField] private GameObject _next;

    private int _currentPage = 0;
    private int _maxPage;

    protected virtual void Start()
    {
        _maxPage = _selected.transform.childCount - 1;
    }

    protected virtual void OnSelected(int page)
    {
    }

    protected void SetPage(int page)
    {
        _currentPage = page;

        SetActiveChild(_prev, _currentPage - 1);
        SetActiveChild(_selected, _currentPage);
        SetActiveChild(_next, _currentPage + 1);

        OnSelected(_currentPage);
    }

    public void Prev()
    {
        if (_currentPage == 0)
        {
            return;
        }

        SetPage(_currentPage - 1);
    }

    public void Next()
    {
        if (_currentPage == _maxPage)
        {
            return;
        }

        SetPage(_currentPage + 1);
    }

    private void SetActiveChild(GameObject gameObject, int childIndex)
    {
        for (int index = 0; index < gameObject.transform.childCount; ++index)
        {
            Transform child = gameObject.transform.GetChild(index);

            child.gameObject.SetActive(index == childIndex);
        }
    }
}