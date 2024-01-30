#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ThePocket.Utils.SQLite
{
    public class RecordCodeGen : CodeGenerator
    {
        public override string FolderPath => "Assets/ThePocket/Scripts/Utils/SQLite";

        public override string Name => "SQLite.Record.Generated.cs";

        protected override void Generate()
        {
            WriteLine("using System;");
            WriteLine("using System.Data;");
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
            WriteLine($"public partial class {table.Name} : IRecord");
            WriteLine("{");
            PushIndent();
            {
                GenerateFetch(table);
            }
            PopIndent();
            WriteLine("}");
        }

        static Dictionary<Type, string> GetFieldSwitch = new()
        {
            { typeof(Byte), "GetByte" },
            { typeof(Char), "GetChar" },
            { typeof(String), "GetString" },
            { typeof(Boolean), "GetBoolean" },
            { typeof(Int16), "GetInt16" },
            { typeof(Int32), "GetInt32" },
            { typeof(Int64), "GetInt64" },
            { typeof(float), "GetFloat" },
            { typeof(double), "GetDouble" },
            { typeof(DateTime), "GetDateTime" },
            { typeof(Guid), "GetGuid" },
        };

        private void GenerateFetch(Type table)
        {

            WriteLine($"public void Fetch(IDataReader reader)");
            WriteLine("{");
            PushIndent();
            {
                int fieldIndex = 0;

                foreach (FieldInfo field in table.GetFields())
                {
                    FieldAttribute attribute = field.GetCustomAttribute<FieldAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    if (GetFieldSwitch.ContainsKey(field.FieldType) == false)
                    {
                        Debug.LogError($"Not Found GetField in IDataReader. " +
                            $"[" +
                            $"Name: {table.Name}, Field Name: {field.Name}, Field Type: {field.FieldType}" +
                            $"]");
                        continue;
                    }

                    WriteLine($"{field.Name} = reader.{GetFieldSwitch[field.FieldType]}({fieldIndex});");

                    ++fieldIndex;
                }
            }
            PopIndent();
            WriteLine("}");
        }
    }
}

#endif