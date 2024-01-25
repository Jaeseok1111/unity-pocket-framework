using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UnityFramework.UI
{
    public class UISettingsLanguage : UIPagination
    {
        [Header("Langauges")]
        [SerializeField] private List<string> _languages = new();

        private StringValue _language;
        private GameConfig _config;

        [Inject]
        public void Construct(
            [Inject(Id = "Language")] StringValue language,
            [Inject] GameConfig config)
        {
            _language = language;
            _config = config;
        }

        protected override void Start()
        {
            base.Start();

            int currentPage = _languages.IndexOf(_language.Value);
            SetPage(currentPage);
        }

        protected override void OnSelected(int page)
        {
            _config.Language = _languages[page];
        }
    }
}