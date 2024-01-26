using TMPro;
using UnityEngine;
using UnityFramework;
using Zenject;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UITextField : MonoBehaviour
{
    [SerializeField] private string _identifier;

    private DiContainer _container;
    private StringValue _string;

    private TextMeshProUGUI _textField;
    private string _lastUpdateValue;

    public string Identifier { get => _identifier; set => _identifier = value; }

    [Inject]
    public void Construct(
        DiContainer container,
        [Inject(Optional = true)] string identifier)
    {
        _container = container;

        if (string.IsNullOrEmpty(identifier) == false)
        {
            _identifier = identifier;
        }
    }

    private void Awake()
    {
        _textField = GetComponent<TextMeshProUGUI>();
    }

    private void LateUpdate()
    {
        if (_string == null || _string.Identifier != _identifier)
        {
            _string = _container.Resolve(new BindingId()
            {
                Type = typeof(StringValue),
                Identifier = _identifier
            }) as StringValue;
        }

        if (_lastUpdateValue == _string.Value)
        {
            return;
        }

        _textField.text = _string?.Value ?? string.Empty;
        _lastUpdateValue = _string.Value;
    }
}