#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ThePocket.Utils.SQLite
{
    public class SchemeCodeGenerator : CodeGenerator
    {
        public override string FolderPath => "Assets/ThePocket/Scripts/Utils/SQLite";
        public override string Name => "SQLite.Scheme.Generated.cs";

        protected override void Generate()
        {
            WriteLine("using ThePocket;");
            WriteLine("using ThePocket.Utils;");
            WriteLine("using ThePocket.Utils.SQLite;");
            WriteLine("using System;");
            WriteLine("using System.Data;");
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
                GenerateDefinitionClass(table, attribute);
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateDefinitionClass(Type table, ModelAttribute attribute)
        {
            WriteLine($"public class Scheme : IScheme");
            WriteLine("{");
            PushIndent();
            {
                WriteLine($"public string Name {{ get => \"{attribute.Name}\"; }}");
                WriteLine($"public int Version {{ get => 0; }}");
                WriteLine();

                GenerateFieldsClass(table);
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateFieldsClass(Type table)
        {
            WriteLine($"public static class Fields");
            WriteLine("{");
            PushIndent();
            {
                foreach (FieldInfo field in table.GetFields())
                {
                    FieldAttribute attribute = field.GetCustomAttribute<FieldAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    WriteLine($"public static FieldScheme {field.Name} {{ get; private set; }} = " +
                        $"new FieldScheme(\"{attribute.Name}\", typeof({field.FieldType.Name}));");
                }
            }
            PopIndent();
            WriteLine("}");
        }
    }
}

#endif