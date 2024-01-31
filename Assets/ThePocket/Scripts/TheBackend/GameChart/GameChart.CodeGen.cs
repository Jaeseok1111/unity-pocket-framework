#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using ThePocket.Utils;
using UnityEditor;

namespace ThePocket
{
    public class GameChartCodeGen : CodeGenerator
    {
        public override string FolderPath => "Assets/ThePocket/Scripts/TheBackend/GameChart";
        public override string Name => "GameChart.Generated.cs";

        protected override void Generate()
        {
            WriteLine("using System;");
            WriteLine("using ThePocket;");
            WriteLine("using ThePocket.Utils;");
            WriteLine();

            GenerateClasses();
        }

        private void GenerateClasses()
        {
            var query = TypeCache
                .GetTypesWithAttribute<ModelAttribute>()
                .Where(type => type.GetCustomAttribute<ModelAttribute>()?.Usage == ModelUsageTargets.GameChart)
                .GroupBy(chart => chart.Namespace);

            foreach (IGrouping<string, Type> group in query)
            {
                string namespaceName = group.Key;

                GenerateNamespaceBegin(namespaceName);

                foreach (Type gameChart in group)
                {
                    GenerateClass(gameChart);
                }

                GenerateNamespaceEnd(namespaceName);
            }
        }

        private void GenerateNamespaceBegin(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                return;
            }

            WriteLine($"namespace {namespaceName}");
            WriteLine("{");
            PushIndent();
        }

        private void GenerateNamespaceEnd(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                return;
            }

            PopIndent();
            WriteLine("}");
        }

        private void GenerateClass(Type gameChart)
        {
            WriteLine($"public partial class {gameChart.Name} : IGameChartForAutoGeneration");
            WriteLine("{");
            PushIndent();
            {
                ModelAttribute attribute = gameChart.GetCustomAttribute<ModelAttribute>();
                
                CreateGetNameFunction(attribute);
                WriteLine();

                CreateSaveToLocal(gameChart);
                WriteLine();

                CreateQueryFunction();
            }
            PopIndent();
            WriteLine("}");
        }

        private void CreateGetNameFunction(ModelAttribute attribute)
        {
            WriteLine($"public string GetName()");
            WriteLine("{");
            PushIndent();
            {
                WriteLine($"return \"{attribute.Name}\";");
            }
            PopIndent();
            WriteLine("}");
        }

        private void CreateSaveToLocal(Type gameData)
        {
            WriteLine($"public void SaveToLocal(LitJson.JsonData gameChartJson)");
            WriteLine("{");
            PushIndent();
            {
                foreach (FieldInfo field in gameData.GetFields())
                {
                    FieldAttribute attr = field.GetCustomAttribute<FieldAttribute>();
                    if (attr == null)
                    {
                        continue;
                    }

                    if (field.FieldType == typeof(string))
                    {
                        WriteLine($"{field.Name} = gameChartJson[\"{attr.Name}\"].ToString();");
                        continue;
                    }

                    Write($"{field.Name} = ");
                    Write($"{field.FieldType.Name}.Parse(gameChartJson[\"{attr.Name}\"].ToString());");
                    WriteLine();
                }
            }
            PopIndent();
            WriteLine("}");
        }

        private void CreateQueryFunction()
        {
            WriteLine($"public GameChartQuery CreateQuery()");
            WriteLine("{");
            PushIndent();
            {
                WriteLine($"return new GameChartQuery(this);");
            }
            PopIndent();
            WriteLine("}");
        }
    }
}

#endif