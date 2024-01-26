using Internal;
using UnityEngine;
using Zenject;

public class ProjectConfig : ILateDisposable
{
    private ConfigFile _file = new();

    private StringValue _language = new StringValue() { Identifier = "Language" };
    private StringValue _user = new StringValue() { Identifier = "UserId" };

    [Inject]
    public ProjectConfig(DiContainer container, string fileName, bool autoFlush)
    {
        container
            .Bind<StringValue>()
            .WithId(_language.Identifier)
            .FromInstance(_language)
            .AsCached();

        container
            .Bind<StringValue>()
            .WithId(_user.Identifier)
            .FromInstance(_user)
            .AsCached();

        _file.Open(fileName, autoFlush);
        {
            _language.Value = _file.GetValue("Language", "Identifier");
            _user.Value = _file.GetValue("User", "Identifier");

            if (_language.Value != null)
            {
                Localization.SetLanguage(_language.Value);
            }
            else
            {
                switch (Application.systemLanguage)
                {
                    case SystemLanguage.English:
                        Language = "en";
                        break;
                    case SystemLanguage.Japanese:
                        Language = "ja";
                        break;
                    case SystemLanguage.Korean:
                        Language = "ko";
                        break;
                }
            }
        }
    }

    public string Language
    {
        get => _language.Value;
        set
        {
            Localization.SetLanguage(value);

            _language.Value = value;
            _file.SetValue("Language", "Identifier", _language.Value);
        }
    }

    public string User
    {
        get => _user.Value;
        set
        {
            _user.Value = value;
            _file.SetValue("User", "Identifier", _user.Value);
        }
    }

    public void LateDispose()
    {
        _file.Dispose();
    }
}