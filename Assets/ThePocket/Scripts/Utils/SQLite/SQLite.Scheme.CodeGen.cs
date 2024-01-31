#if UNITY_EDITOR

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using static UnityEngine.CompositeCollider2D;

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

        private void GenerateClass(Type table, ModelAttribute tableAttribute)
        {
            WriteLine($"public partial class {table.Name}");
            WriteLine("{");
            PushIndent();
            {
                GenerateDefinitionClass(table, tableAttribute);
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateDefinitionClass(Type table, ModelAttribute tableAttribute)
        {
            WriteLine($"public class Scheme : ThePocket.Utils.SQLite.Scheme");
            WriteLine("{");
            PushIndent();
            {
                GenerateConstructor(table, tableAttribute);
                WriteLine();
                GenerateFieldsClass(table);
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateConstructor(Type table, ModelAttribute tableAttribute)
        {
            WriteLine("public Scheme()");
            WriteLine("{");
            PushIndent();
            {
                WriteLine($"TableName = \"{tableAttribute.Name}\";");

                foreach (FieldInfo field in table.GetFields())
                {
                    FieldAttribute attribute = field.GetCustomAttribute<FieldAttribute>();
                    if (attribute == null)
                    {
                        continue;
                    }

                    WriteLine($"AddField({field.Name});");
                }
            }
            PopIndent();
            WriteLine("}");
        }

        private void GenerateFieldsClass(Type table)
        {
            foreach (FieldInfo field in table.GetFields())
            {
                FieldAttribute attribute = field.GetCustomAttribute<FieldAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                Scheme.FieldType? fieldType = field.FieldType.ToString() switch
                {
                    "System.Boolean" => Scheme.FieldType.TEXT,
                    "System.Int16" => Scheme.FieldType.INTEGER,
                    "System.Int32" => Scheme.FieldType.INTEGER,
                    "System.Int64" => Scheme.FieldType.INTEGER,
                    "float" => Scheme.FieldType.REAL,
                    "double" => Scheme.FieldType.REAL,
                    "System.Decimal" => Scheme.FieldType.REAL,
                    "System.String" => Scheme.FieldType.TEXT,
                    "System.Enum" => Scheme.FieldType.TEXT,
                    "System.DateTime" => Scheme.FieldType.TEXT,
                    _ => null,
                };

                if (fieldType == null)
                {
                    continue;
                }

                WriteLine($"public static Scheme.Field {field.Name}");
                WriteLine("{");
                PushIndent();
                {
                    WriteLine("get");
                    WriteLine("{");
                    PushIndent();
                    {
                        WriteLine($"return new Scheme.Field()");
                        WriteLine("{");
                        PushIndent();
                        {
                            WriteLine($"Name = \"{attribute.Name}\",");
                            WriteLine($"Type = Scheme.FieldType.{fieldType},");
                            WriteLine($"NotNull = {(attribute.NotNull ? "true" : "false")},");
                            WriteLine($"Unique = {(attribute.Unique ? "true" : "false")},");
                            WriteLine($"PrimaryKey = {(attribute.PrimaryKey ? "true" : "false")},");
                        }
                        PopIndent();
                        WriteLine("};");
                    }
                    PopIndent();
                    WriteLine("}");
                }
                PopIndent();
                WriteLine("}");
            }
        }
    }
}

#endif