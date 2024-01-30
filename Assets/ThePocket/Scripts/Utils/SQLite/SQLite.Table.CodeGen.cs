#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace ThePocket.Utils.SQLite
{
    public class TableCodeGen : CodeGenerator
    {
        public override string FolderPath => "Assets/ThePocket/Scripts/Utils/SQLite";
        public override string Name => "SQLite.Table.Generated.cs";

        protected override void Generate()
        {
            WriteLine("using System;");
            WriteLine("using ThePocket;");
            WriteLine("using ThePocket.Utils;");
            WriteLine("using ThePocket.Utils.SQLite;");
            WriteLine();

            GenerateClasses();
        }

        private void GenerateClasses()
        {
            var query = TypeCache
                .GetTypesWithAttribute<ModelAttribute>()
                .GroupBy(chart => chart.Namespace);

            foreach (IGrouping<string, Type> group in query)
            {
                string namespaceName = group.Key;

                GenerateNamespaceBegin(namespaceName);
                {
                    foreach (Type table in group)
                    {
                        ModelAttribute attribute = table.GetCustomAttribute<ModelAttribute>();
                        if (attribute == null)
                        {
                            continue;
                        }

                        GenerateClass(table, attribute);
                    }
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

        private void GenerateClass(Type table, ModelAttribute attribute)
        {
            WriteLine($"public partial class {table.Name}");
            WriteLine("{");
            PushIndent();
            {
                GenerateTableClass(table, attribute);
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateTableClass(Type table, ModelAttribute attribute)
        {
            WriteLine($"public class Table : Table<{table.Name}>");
            WriteLine("{");
            PushIndent();
            {
                // Constructor
                WriteLine($"public Table(DatabaseContext context)");
                PushIndent();
                {
                    WriteLine($": base(context, \"{attribute.Name}\")");
                }
                PopIndent();
                WriteLine("{");
                WriteLine("}");
            }
            PopIndent();
            WriteLine("}");
        }
    }
}


#endif