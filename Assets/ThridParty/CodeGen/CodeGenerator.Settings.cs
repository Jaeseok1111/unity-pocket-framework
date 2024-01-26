#if UNITY_EDITOR

using UnityEditor;

namespace ThridParty
{
    public class CodeGeneratorSettings
    {
        const string KEY_GENERATE_ON_COMPILE = "CodeGen-AutoGenerateOnCompile";

        public static bool autoGenerateOnCompile
        {
            get
            {
                if (bool.TryParse(EditorUserSettings.GetConfigValue(KEY_GENERATE_ON_COMPILE), out var result))
                {
                    return result;
                }
                return false;
            }
            set
            {
                EditorUserSettings.SetConfigValue(KEY_GENERATE_ON_COMPILE, value.ToString());
            }
        }
    }
}

#endif