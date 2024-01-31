using System;
using System.Collections.Generic;
using System.Linq;

namespace ThePocket.Utils
{
    public class CodeBuilder
    {
        private int _currentIndent;
        private List<string> _lines = new();

        private string _temporaryString;

        public override string ToString()
        {
            if (_temporaryString != null)
            {
                PushTemporaryString();
            }

            return string.Join(Environment.NewLine, _lines.Select(x => x.TrimEnd()));
        }

        public void Write(string code)
        {
            _temporaryString = _temporaryString + code ?? code;
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public void WriteLine(string code)
        {
            Write(code);
            PushTemporaryString();
        }

        public void PushIndent()
        {
            if (_temporaryString != null)
            {
                throw new InvalidOperationException("Unable to indent while line is not empty.");
            }

            _currentIndent++;
        }

        public void PopIndent()
        {
            _currentIndent--;
            if (_currentIndent < 0)
            {
                throw new InvalidOperationException("Unable to unindent!");
            }
        }

        private void PushTemporaryString()
        {
            if (_temporaryString == null)
            {
                throw new ArgumentNullException();
            }

            string indentString = new(' ', 4 * _currentIndent);

            _lines.Add(indentString + _temporaryString);
            _temporaryString = null;
        }
    }
}