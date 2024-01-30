#if UNITY_EDITOR

namespace ThePocket.Utils
{
    public class CodeGeneratorContext
    {
        public string FileName { get; private set; } = null;

        public string FolderPath { get; private set; } = null;

        public string Code { get; private set; } = null;

        public void OverrideFileName(string name)
        {
            FileName = name;
        }

        public void OverrideFolderPath(string path)
        {
            FolderPath = path;
        }

        public void OverrideCode(string code)
        {
            Code = code;
        }
    }
}

#endif