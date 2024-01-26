using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityFramework;
using Zenject;

public class UILoadingBar : MonoBehaviour
{
    private DiContainer _container;
    private IntValue _percent;

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Slider _slider;

    private int _lastUpdatePercent = -1;

    [Inject]
    public void Construct(DiContainer container)
    {
        _container = container;
    }

    private void Start()
    {
        _percent = new IntValue() { Identifier = _container.ResolveId<string>("LoadingWithBar_Identifier") };

        SetPercent(0);
    }

    private void LateUpdate()
    {
        if (_lastUpdatePercent == _percent.Value)
        {
            return;
        }

        _slider.value = _percent.Value;
        _lastUpdatePercent = _percent.Value;

        L10NString l10n = new L10NString()
        {
            Key = "LoadingBar_Percent",
            Values = new()
                {
                    { "Percent", Mathf.FloorToInt(_percent.Value) }
                }
        };

        _text.text = Localization.GetString(l10n);
    }

    public void SetPercent(int value)
    {
        _percent.Value = value;
    }
}