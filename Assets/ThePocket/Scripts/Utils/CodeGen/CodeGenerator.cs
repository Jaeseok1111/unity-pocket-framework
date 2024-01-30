using System;
using System.Collections.Generic;
using System.Linq;

namespace ThePocket.Utils
{
    public abstract class CodeGenerator
    {
        public abstract string FolderPath { get; }
        public abstract string Name { get; }

        private int _currentIndent;
        private List<string> _lines = new();

        private string _temporaryString;

        public void Override(CodeGeneratorContext context)
        {
            context.OverrideFileName(Name);
            context.OverrideFolderPath(FolderPath);
        }

        public void GenerateCode(CodeGeneratorContext context)
        {
            Generate();

            Flush(context);
        }

        protected abstract void Generate();

        protected void Flush(CodeGeneratorContext context)
        {
            if (_temporaryString != null)
            {
                PushTemporaryString();
            }

            string header =
                $"/// This file is auto-generated file with project,{Environment.NewLine}" +
                $"/// with {GetType().FullName} class.{Environment.NewLine}" +
                $"/// Please do not modify this file directly, or any modification at this file will lost at pre-build.{Environment.NewLine}" +
                $"/// @Jaeseok{Environment.NewLine}" +
                $"{Environment.NewLine}";

            string code = string.Join(Environment.NewLine, _lines.Select(x => x.TrimEnd()));

            context.OverrideCode(
                $"{header}" +
                $"{Environment.NewLine}" +
                $"{code}" +
                $"{Environment.NewLine}");
        }

        #region Write/Indent
        protected void Write(string code)
        {
            _temporaryString = _temporaryString + code ?? code;
        }

        protected void WriteLine()
        {
            WriteLine(string.Empty);
        }

        protected void WriteLine(string code)
        {
            Write(code);
            PushTemporaryString();
        }

        protected void PushIndent()
        {
            if (_temporaryString != null)
            {
                throw new InvalidOperationException("Unable to indent while line is not empty.");
            }

            _currentIndent++;
        }

        protected void PopIndent()
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
        #endregion
    }
}