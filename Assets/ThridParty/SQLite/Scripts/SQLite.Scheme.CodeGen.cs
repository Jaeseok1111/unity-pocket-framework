#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ThridParty.SQLite
{
    public class SchemeCodeGenerator : CodeGenerator
    {
        public override string FolderPath => "Assets/ThridParty/SQLite/Scripts";
        public override string Name => "SQLite.Scheme.Generated.cs";

        protected override void Generate()
        {
            WriteLine("using System;");
            WriteLine("using System.Data;");
            WriteLine("using ThridParty.SQLite;");
            WriteLine();

            GenerateClasses();
        }

        private void GenerateClasses()
        {
            var query = TypeCache
                .GetTypesWithAttribute<TableAttribute>()
                .GroupBy(chart => chart.Namespace);

            foreach (IGrouping<string, Type> group in query)
            {
                string namespaceName = group.Key;
                GenerateNamespaceBegin(namespaceName);

                foreach (Type table in group)
                {
                    TableAttribute attribute = table.GetCustomAttribute<TableAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    GenerateClass(table, attribute);
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

        private void GenerateClass(Type table, TableAttribute attribute)
        {
            WriteLine($"public partial class {table.Name} : IRecord");
            WriteLine("{");
            PushIndent();
            {
                GenerateFetch(table);
                WriteLine();
                GenerateDefinitionClass(table, attribute);
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

        private void GenerateDefinitionClass(Type table, TableAttribute tableAttribute)
        {
            WriteLine($"public class Scheme : TableScheme");
            WriteLine("{");
            PushIndent();
            {
                WriteLine($"public override string Name {{ get; protected set; }} = \"{tableAttribute.Name}\";");
                WriteLine($"public override int Version {{ get; protected set; }} = 0;");
                WriteLine();

                GenerateFieldsClass(table);
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateFieldsClass(Type table)
        {
            WriteLine($"public class Fields");
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

                    WriteLine($"public FieldScheme {field.Name} {{ get; private set; }} = " +
                        $"new FieldScheme(\"{attribute.Name}\", typeof({field.FieldType.Name}));");
                }
            }
            PopIndent();
            WriteLine("}");
        }
    }
}

#endif