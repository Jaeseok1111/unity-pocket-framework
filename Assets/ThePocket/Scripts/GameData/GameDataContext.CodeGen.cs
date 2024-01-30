#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThePocket.Utils;
using UnityEditor;

namespace ThePocket
{
    public class GameDataContextCodeGen : CodeGenerator
    {
        public override string FolderPath => "Assets/ThePocket/Scripts/GameData";
        public override string Name => "GameDataContext.Generated.cs";

        protected override void Generate()
        {
            var query = TypeCache
                .GetTypesWithAttribute<ModelAttribute>()
                .GroupBy(chart => chart.Namespace);

            GenerateUsingNamespaces(query);
            GenerateClass(query);
        }

        private void GenerateUsingNamespaces(IEnumerable<IGrouping<string, Type>> query)
        {
            WriteLine("using System;");
            WriteLine("using ThePocket;");
            WriteLine("using ThePocket.Utils;");
            WriteLine("using ThePocket.Utils.SQLite;");

            foreach (IGrouping<string, Type> group in query)
            {
                WriteLine($"using {group.Key};");
            }

            WriteLine();
        }

        private void GenerateClass(IEnumerable<IGrouping<string, Type>> query)
        {
            WriteLine($"namespace ThePocket");
            WriteLine("{");
            PushIndent();
            {
                WriteLine($"public partial class GameDataContext");
                WriteLine("{");
                PushIndent();
                {
                    GenerateFields(query);
                    WriteLine();
                    GenerateConstructor(query);
                }
                PopIndent();
                WriteLine("}");
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateFields(IEnumerable<IGrouping<string, Type>> query)
        {
            foreach (IGrouping<string, Type> group in query)
            {
                foreach (Type table in group)
                {
                    ModelAttribute attribute = table.GetCustomAttribute<ModelAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    WriteLine($"public {table.Name}.Table {attribute.Name.ToPascalCase()} {{ get; private set; }}");
                }
            }
        }

        private void GenerateConstructor(IEnumerable<IGrouping<string, Type>> query)
        {
            WriteLine($"partial void Construct()");
            WriteLine("{");
            PushIndent();
            {
                foreach (IGrouping<string, Type> group in query)
                {
                    foreach (Type table in group)
                    {
                        ModelAttribute attribute = table.GetCustomAttribute<ModelAttribute>();
                        if (attribute == null)
                        {
                            continue;
                        }

                        WriteLine($"{attribute.Name.ToPascalCase()} = new(_database);");
                    }
                }
            }
            PopIndent();
            WriteLine("}");
        }
    }
}

#endif