using UnityEngine;
using Zenject;

public class UILogger
{
    private readonly DiContainer _container;

    private readonly Canvas _mainCanvas;
    private readonly UIPanel _panel;

    private readonly GameObject _ok;
    private readonly GameObject _logWarning;
    private readonly GameObject _logError;

    public UILogger(
        [Inject] DiContainer container,
        [Inject(Id = "Main")] Canvas mainCanvas,
        [Inject] UIPanel panel,
        [Inject(Id = "LogOk", Optional = true)] GameObject ok,
        [Inject(Id = "LogWarning", Optional = true)] GameObject logWarning,
        [Inject(Id = "LogError", Optional = true)] GameObject logError)
    {
        _container = container;
        _mainCanvas = mainCanvas;
        _panel = panel;
        _ok = ok;
        _logWarning = logWarning;
        _logError = logError;
    }

    public void Ok(string message)
    {
        Debug.Log(message);

        Popup(_ok, "Log_Message", message);
    }

    public void LogWarning(string message)
    {
        Debug.LogWarning(message);

        Popup(_logWarning, "Log_WarningMessage", message);
    }

    public void LogError(string message)
    {
        Debug.LogError(message);

        Popup(_logError, "Log_ErrorMessage", message);
    }

    private void Popup(GameObject prefab, string identifier, string message)
    {
        if (prefab == null)
        {
            return;
        }

        if (_container.HasBindingId<StringValue>(identifier))
        {
            StringValue value = _container.Resolve(new BindingId()
            {
                Type = typeof(StringValue),
                Identifier = identifier
            }) as StringValue;

            value.Value = message;
        }
        else
        {
            _container
                .Bind<StringValue>()
                .WithId(identifier)
                .FromInstance(new StringValue()
                {
                    Identifier = identifier,
                    Value = message
                })
                .AsCached();
        }

        GameObject newPopup = _container.InstantiatePrefab(prefab, _mainCanvas.transform);
        newPopup.GetComponentInChildren<UITextField>().Identifier = identifier;

        _panel.Popup(newPopup);
    }
}