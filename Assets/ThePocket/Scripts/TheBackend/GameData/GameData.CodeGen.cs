#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using ThePocket.Utils;
using UnityEditor;

namespace ThePocket
{
    public class GameDataCodeGen : CodeGenerator
    {
        public override string FolderPath => "Assets/ThePocket/Scripts/TheBackend/GameData";
        public override string Name => "GameData.Generated.cs";

        protected override void Generate()
        {
            WriteLine("using BackEnd;");
            WriteLine("using ThePocket;");
            WriteLine("using ThePocket.Utils;");
            WriteLine("using System;");
            WriteLine("using System.Globalization;");
            WriteLine("using UnityEngine;");
            WriteLine();

            GenerateClasses();
        }

        private void GenerateClasses()
        {
            var query = TypeCache
                .GetTypesWithAttribute<GameDataAttribute>()
                .GroupBy(chart => chart.Namespace);

            foreach (IGrouping<string, Type> group in query)
            {
                string namespaceName = group.Key;

                GenerateNamespaceBegin(namespaceName);

                foreach (Type gameData in group)
                {
                    GameDataAttribute attribute = gameData.GetCustomAttribute<GameDataAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    GenerateClass(gameData, attribute);
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

        private void GenerateClass(Type gameData, GameDataAttribute attribute)
        {
            WriteLine($"public partial class {gameData.Name} : IGameDataForAutoGeneration");
            WriteLine("{");
            PushIndent();
            {
                CreateGetNameFunction(attribute);
                WriteLine();

                CreateToServerFunction(gameData);
                WriteLine();

                CreateToLocalFunction(gameData);
                WriteLine();

                CreateQueryFunction();
            }
            PopIndent();
            WriteLine("}");
        }

        private void CreateGetNameFunction(GameDataAttribute attribute)
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

        private void CreateToServerFunction(Type gameData)
        {
            WriteLine($"public Param ToServer()");
            WriteLine("{");
            PushIndent();
            {
                WriteLine("Param param = new();");
                WriteLine();
                {
                    foreach (FieldInfo field in gameData.GetFields())
                    {
                        GameDataColumnAttribute column = field.GetCustomAttribute<GameDataColumnAttribute>();
                        if (column == null)
                        {
                            continue;
                        }

                        if (field.FieldType == typeof(DateTime))
                        {
                            WriteLine($"param.Add(\"{column.Name}\", string.Format(\"{{0:MM-DD:HH:mm:ss.fffZ}}\", {field.Name}.ToString(CultureInfo.InvariantCulture)));");
                            continue;
                        }

                        WriteLine($"param.Add(\"{column.Name}\", {field.Name});");
                    }
                }
                WriteLine();
                WriteLine($"return param;");
            }
            PopIndent();
            WriteLine("}");
        }

        private void CreateToLocalFunction(Type gameData)
        {
            WriteLine($"public void ToLocal(LitJson.JsonData gameDataJson)");
            WriteLine("{");
            PushIndent();
            {
                foreach (FieldInfo field in gameData.GetFields())
                {
                    GameDataColumnAttribute column = field.GetCustomAttribute<GameDataColumnAttribute>();
                    if (column == null)
                    {
                        continue;
                    }


                    if (field.FieldType == typeof(string))
                    {
                        WriteLine($"{field.Name} = gameDataJson[\"{column.Name}\"].ToString();");
                        continue;
                    }

                    Write($"{field.Name} = ");
                    Write($"{field.FieldType.Name}.Parse(gameDataJson[\"{column.Name}\"].ToString());");
                    WriteLine();
                }
            }
            PopIndent();
            WriteLine("}");
        }

        private void CreateQueryFunction()
        {
            WriteLine($"public GameDataQuery CreateQuery()");
            WriteLine("{");
            PushIndent();
            {
                WriteLine($"return new GameDataQuery(this);");
            }
            PopIndent();
            WriteLine("}");
        }
    }
}

#endif