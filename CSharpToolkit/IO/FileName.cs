using System;
using System.IO;

namespace CSharpToolkit.IO
{
    public class FileName : IEquatable<FileName>
    {
        public string NameWithoutExtension { get => Path.GetFileNameWithoutExtension(_name); }

        public string Name { get => _name; }

        public string Extension { get => Path.GetExtension(_name); }

        public FileName(string name)
        {
            _name = Path.GetFileName(name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileName);
        }

        public bool Equals(FileName other)
        {
            return other != null &&
                   StringComparer.OrdinalIgnoreCase.Equals(_name, other._name);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(_name);
        }

        public override string ToString()
        {
            return _name;
        }

        private readonly string _name;
    }
}
