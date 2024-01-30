using System;
using System.IO;
using UnityEngine;

namespace ThePocket.Utils
{
    public class File : IDisposable
    {
        protected string _path;
        protected string _text;

        public void Open(string path)
        {
            _path = Path.Combine(Application.persistentDataPath, path);

            if (System.IO.File.Exists(_path))
            {
                _text = System.IO.File.ReadAllText(_path);
            }
            else
            {
                System.IO.File.Create(_path).Close();
                _text = string.Empty;
            }
        }

        public StringReader GetReader()
        {
            return new StringReader(_text);
        }

        public virtual void Close()
        {
            _path = null;
            _text = null;
        }

        public virtual void Dispose()
        {
            Close();
        }
    }
}