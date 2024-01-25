using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnityFramework.Internal
{
    public class ConfigFile : File
    {
        private class Section
        {
            private string _name;
            private Dictionary<string, string> _params = new();

            private StringBuilder _stringBuilder = new();
            private bool _modified = false;

            public Section(string name)
            {
                _name = name;
            }

            public void SetValue(string key, string value)
            {
                _params[key] = value;
                _modified = true;
            }

            public string GetValue(string key)
            {
                return _params.TryGetValue(key, out string value) ? value : null;
            }

            public void Write(StringWriter writer)
            {
                if (_modified)
                {
                    _stringBuilder = ToStringBuilder();
                    _modified = false;
                }

                writer.WriteLine(_stringBuilder.ToString());
            }

            public void Parse(StringReader reader)
            {
                string line;
                string key = string.Empty;
                string value = string.Empty;

                while ((line = reader.ReadLine()) != string.Empty)
                {
                    if (ParseKeyValue(line, ref key, ref value) == false)
                    {
                        continue;
                    }

                    if (_params.ContainsKey(key))
                    {
                        continue;
                    }

                    _params.Add(key, value);
                }

                _stringBuilder = ToStringBuilder();
            }

            private bool ParseKeyValue(string line, ref string key, ref string value)
            {
                int keyIndex = line.IndexOf('=');
                int valueIndex = line.Length - keyIndex - 1;

                if (keyIndex <= 0)
                {
                    return false;
                }

                key = line.Substring(0, keyIndex).Trim();
                if (key.Length <= 0)
                {
                    return false;
                }

                value = (valueIndex > 0) ? (line.Substring(keyIndex + 1, valueIndex).Trim()) : ("");
                return true;
            }

            private StringBuilder ToStringBuilder()
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine($"[{_name}]");

                foreach (string key in _params.Keys)
                {
                    builder.AppendLine($"{key}={_params[key]}");
                }

                builder.AppendLine();
                return builder;
            }
        }

        private Dictionary<string, Section> _sections = new();

        private bool _isAutoFlush = false;
        private bool _isCacheModified = false;

        public void Open(string path, bool autoFlush)
        {
            base.Open(path);

            Initialize(autoFlush);
        }

        public void Initialize(bool authFlush)
        {
            _isAutoFlush = authFlush;

            Refresh();
        }

        public override void Close()
        {
            Flush();

            base.Close();
        }

        public void Refresh()
        {
            StringReader reader = null;

            try
            {
                reader = GetReader();

                string line;
                string sectionName;
                Section section = null;

                _sections.Clear();

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    sectionName = ParseSectionName(line);
                    if (sectionName == null)
                    {
                        continue;
                    }

                    section = new(sectionName);
                    section.Parse(reader);

                    _sections.Add(sectionName, section);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }

                reader = null;
            }
        }

        public void Flush()
        {
            if (_isCacheModified == false)
            {
                return;
            }

            _isCacheModified = false;

            StringWriter writer = new();

            try
            {
                foreach (Section section in _sections.Values)
                {
                    section.Write(writer);
                }

                if (_path != null)
                {
                    System.IO.File.WriteAllText(_path, writer.ToString());
                }
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }

                writer = null;
            }
        }

        public void SetValue(string sectionName, string key, string value)
        {
            if (_sections.ContainsKey(sectionName) == false)
            {
                _sections.Add(sectionName, new Section(sectionName));
            }

            Section section = _sections[sectionName];
            section.SetValue(key, value);

            _isCacheModified = true;

            if (_isAutoFlush)
            {
                Flush();
            }
        }

        public string GetValue(string sectionName, string key)
        {
            if (_sections.ContainsKey(sectionName) == false)
            {
                return null;
            }

            Section section = _sections[sectionName];
            return section.GetValue(key);
        }

        private string ParseSectionName(string line)
        {
            if (line.StartsWith("[") == false ||
                line.EndsWith("]") == false ||
                line.Length < 3)
            {
                return null;
            }

            return line.Substring(1, line.Length - 2);
        }
    }
}
