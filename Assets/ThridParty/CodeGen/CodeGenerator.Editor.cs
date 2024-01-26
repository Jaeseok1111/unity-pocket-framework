#if UNITY_EDITOR

using System;
using System.IO;
using System.Text;
using UnityEditor;

namespace ThridParty
{
    public static class CodeGeneratorEditor
    {
        const string MENU_GENERATE = "Tools/Code Generator/Generate";
        const string MENU_TOGGLE_AUTO_GENERATE = "Tools/Code Generator/Auto-generate on Compile";

        [InitializeOnLoadMethod]
        private static void Init()
        {
            Menu.SetChecked(MENU_TOGGLE_AUTO_GENERATE, CodeGeneratorSettings.autoGenerateOnCompile);

            if (CodeGeneratorSettings.autoGenerateOnCompile)
            {
                Generate();
            }
        }

        [MenuItem(MENU_GENERATE)]
        private static void Generate()
        {
            bool changed = false;

            foreach (var generatorType in TypeCache.GetTypesDerivedFrom<CodeGenerator>())
            {
                var generator = (CodeGenerator)Activator.CreateInstance(generatorType);
                var context = new CodeGeneratorContext();

                generator.Override(context);
                generator.GenerateCode(context);

                GenerateCodeContext(context);

                changed = true;
            }

            if (changed)
            {
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }

        private static void GenerateCodeContext(CodeGeneratorContext context)
        {
            string path = Path.Combine(context.FolderPath, context.FileName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (Directory.Exists(context.FolderPath) == false)
            {
                Directory.CreateDirectory(context.FolderPath);
            }

            File.WriteAllText(path, context.Code, Encoding.UTF8);
        }

        [MenuItem(MENU_TOGGLE_AUTO_GENERATE)]
        private static void ToggleAutoGenerate()
        {
            CodeGeneratorSettings.autoGenerateOnCompile = !CodeGeneratorSettings.autoGenerateOnCompile;
            Menu.SetChecked(MENU_TOGGLE_AUTO_GENERATE, CodeGeneratorSettings.autoGenerateOnCompile);
        }
    }
}

#endif