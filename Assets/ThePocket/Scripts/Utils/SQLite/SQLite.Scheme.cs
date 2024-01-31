using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePocket.Utils.SQLite
{
    public partial class Scheme
    {
        public string TableName { get; protected set; }

        public override bool Equals(object obj)
        {
            if (obj is Scheme)
            {
                Scheme other = (Scheme)obj;

                return TableName == other.TableName && Enumerable.SequenceEqual(_fields, other._fields);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return TableName.GetHashCode();
        }

        public override string ToString()
        {
            return TableName;
        }
    }

    #region Scheme Field

    public partial class Scheme
    {
        public enum FieldType
        {
            INTEGER,
            TEXT,
            REAL,
        }

        public class Field
        {
            public string Name { get; set; }
            public FieldType Type { get; set; }
            public bool NotNull { get; set; }
            public bool Unique { get; set; }
            public bool PrimaryKey { get; set; }

            public string ToQuery()
            {
                List<string> strings = new List<string>();

                strings.Add($"{Name} {Type}");

                if (NotNull) strings.Add("NOT NULL");
                if (Unique) strings.Add("UNIQUE");

                return string.Join(" ", strings);
            }

            public override bool Equals(object obj)
            {
                if (obj is Field)
                {
                    Field other = (Field)obj;
                    
                    return 
                        Name == other.Name && 
                        Type == other.Type &&
                        NotNull == other.NotNull &&
                        Unique == other.Unique &&
                        PrimaryKey == other.PrimaryKey;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private List<Field> _fields = new();

        protected void AddField(Field newField)
        {
            _fields.Add(newField);
        }

        public Field GetField(string name)
        {
            return _fields.Find(x => x.Name == name);
        }

        public List<Field> GetFields()
        {
            return _fields;
        }
    }

    #endregion

    #region Scheme Converter

    public partial class Scheme
    {
        public static Scheme Parse(string query)
        {
            string[][] lines = query
                .Replace(",", "\n")
                .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToOnlyWords())
                .Where(x => x.Length > 0)
                .ToArray();

            var scheme = new Scheme();
            scheme.TableName = lines[0].Last();

            for (int index = 1; index < lines.Length; ++index)
            {
                string[] words = lines[index];

                if (words.Length <= 0)
                {
                    continue;
                }

                switch (words[0])
                {
                    case "PRIMARY":
                        for (int keyIndex = 2; keyIndex < words.Length; ++keyIndex)
                        {
                            Field field = scheme.GetField(words[keyIndex]);
                            if (field != null)
                            {
                                field.PrimaryKey = true;
                            }
                        }
                        break;
                    default:
                        scheme.AddField(new Field()
                        {
                            Name = words[0],
                            Type = (FieldType)Enum.Parse(typeof(FieldType), words[1]),
                            NotNull = words.Contains("NOT") && words.Contains("NULL"),
                            Unique = words.Contains("UNIQUE"),
                        });
                        break;
                }
            }

            return scheme;
        }

        public string ToQuery()
        {
            CodeBuilder builder = new CodeBuilder();

            builder.WriteLine($"CREATE TABLE {TableName} (");
            builder.PushIndent();
            {
                foreach (var field in _fields)
                {
                    builder.WriteLine($"{field.ToQuery()},");
                }

                builder.WriteLine("PRIMARY KEY" +
                    "(" +
                    $"{string.Join(",", _fields.Where(field => field.PrimaryKey).Select(field => field.Name))}" +
                    ")");
            }
            builder.PopIndent();
            builder.WriteLine(")");

            return builder.ToString();
        }
    }

    #endregion
}
