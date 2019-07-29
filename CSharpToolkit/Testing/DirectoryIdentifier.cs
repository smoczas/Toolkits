using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpToolkit.Testing
{
    public class DirectoryIdentifier : IEquatable<DirectoryIdentifier>
    {
        public string Name { get => new DirectoryInfo(_id).Name; }

        public string FullName
        {
            get 
            {
                var di = new DirectoryInfo(_id);
                return Path.Combine(di?.Parent?.FullName ?? string.Empty, di.Name); // this is to take care of the drive names (e.g. c:\)
            }
        }

        public DirectoryIdentifier(string path)
        {
            if (path[path.Length - 1] != Path.DirectorySeparatorChar)
            {
                path += Path.DirectorySeparatorChar;
            }

            var key = Path.GetFullPath(path);
            _id = key.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as DirectoryIdentifier);
        }

        public bool Equals(DirectoryIdentifier other)
        {
            return other != null &&
                   StringComparer.OrdinalIgnoreCase.Equals(_id, other._id);
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(_id);
        }

        public override string ToString()
        {
            return _id;
        }

        public static List<DirectoryIdentifier> Split(DirectoryIdentifier instance)
        {
            var result = new List<DirectoryIdentifier>();
            int index = 0;
            while ((index = instance._id.IndexOf(Path.DirectorySeparatorChar, index)) >= 0)
            {
                var subKeyValue = instance._id.Substring(0, index);
                var subKey = new DirectoryIdentifier(subKeyValue);
                result.Add(subKey);

                // start searching after '/'
                ++index;
            }
            return result;
        }

        private readonly string _id;
    }
}
