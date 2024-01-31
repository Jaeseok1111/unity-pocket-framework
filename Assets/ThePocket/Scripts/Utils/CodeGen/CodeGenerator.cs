using System;

namespace ThePocket.Utils
{
    public abstract class CodeGenerator
    {
        public abstract string FolderPath { get; }
        public abstract string Name { get; }

        private CodeBuilder _builder = new();

        public void Override(CodeGeneratorContext context)
        {
            context.OverrideFileName(Name);
            context.OverrideFolderPath("Assets/ThePocket/Scripts/Generated");
        }

        public void GenerateCode(CodeGeneratorContext context)
        {
            Generate();

            Flush(context);
        }

        protected abstract void Generate();

        protected void Flush(CodeGeneratorContext context)
        {
            string header =
                $"/// This file is auto-generated file with project,{Environment.NewLine}" +
                $"/// with {GetType().FullName} class.{Environment.NewLine}" +
                $"/// Please do not modify this file directly, or any modification at this file will lost at pre-build.{Environment.NewLine}" +
                $"/// @Jaeseok{Environment.NewLine}" +
                $"{Environment.NewLine}";

            string code = _builder.ToString();

            context.OverrideCode(
                $"{header}" +
                $"{Environment.NewLine}" +
                $"{code}" +
                $"{Environment.NewLine}");
        }

        #region Write/Indent
        protected void Write(string code)
        {
            _builder.Write(code);
        }

        protected void WriteLine()
        {
            _builder.WriteLine();
        }

        protected void WriteLine(string code)
        {
            _builder.WriteLine(code);
        }

        protected void PushIndent()
        {
            _builder.PushIndent();
        }

        protected void PopIndent()
        {
            _builder.PopIndent();
        }
        #endregion
    }
}