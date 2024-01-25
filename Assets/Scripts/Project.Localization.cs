using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityFramework
{
    public class L10NString
    {
        public string Key;
        public Dictionary<string, object> Values = new();
    }

    public static class Localization
    {
        private static string _StringTableName = "Localization String Table";

        public static string GetSelectedLanguage()
        {
            return LocalizationSettings.SelectedLocale.Identifier.Code;
        }

        public static void SetLanguage(string languageIdentifier)
        {
            LocaleIdentifier localeCode = new LocaleIdentifier(languageIdentifier);

            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                Locale aLocale = LocalizationSettings.AvailableLocales.Locales[i];
                LocaleIdentifier anIdentifier = aLocale.Identifier;

                if (anIdentifier == localeCode)
                {
                    LocalizationSettings.SelectedLocale = aLocale;
                }
            }
        }

        public static string GetString(string key)
        {
            LocalizedString localizedString = new LocalizedString()
            {
                TableReference = _StringTableName,
                TableEntryReference = key
            };

            return Localized(localizedString);
        }

        public static string GetString(L10NString str)
        {
            LocalizedString localizedString = new LocalizedString()
            {
                TableReference = _StringTableName,
                TableEntryReference = str.Key
            };

            foreach (string key in str.Values.Keys)
            {
                switch (str.Values[key])
                {
                    case int intValue:
                        localizedString.Add(key, new IntVariable() { Value = intValue });
                        break;
                    case float floatValue:
                        localizedString.Add(key, new FloatVariable() { Value = floatValue });
                        break;
                    case string strValue:
                        localizedString.Add(key, new StringVariable() { Value = strValue });
                        break;
                }
            }

            return Localized(localizedString);
        }

        private static string Localized(LocalizedString localizedString)
        {
            var stringOperation = localizedString.GetLocalizedStringAsync();

            if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
            {
                return stringOperation.Result;
            }
            else
            {
                return null;
            }
        }
    }
}
