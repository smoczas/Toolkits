using System;
using System.Collections.Generic;
using System.IO;
using CSharpToolkit.IO;

namespace CSharpToolkit.Testing
{
    public class FileIdentifier : IEquatable<FileIdentifier>
    {
        public DirectoryIdentifier OwningDirectory { get; }
        public FileName Name { get; }

        public FileIdentifier(DirectoryIdentifier dirId, FileName name)
        {
            OwningDirectory = dirId;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FileIdentifier);
        }

        public bool Equals(FileIdentifier other)
        {
            return other != null &&
                   EqualityComparer<DirectoryIdentifier>.Default.Equals(OwningDirectory, other.OwningDirectory) &&
                   EqualityComparer<FileName>.Default.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            var hashCode = -765475078;
            hashCode = hashCode * -1521134295 + EqualityComparer<DirectoryIdentifier>.Default.GetHashCode(OwningDirectory);
            hashCode = hashCode * -1521134295 + EqualityComparer<FileName>.Default.GetHashCode(Name);
            return hashCode;
        }

        public override string ToString()
        {
            return Path.Combine(OwningDirectory.FullName, Name.Name);
        }

        public static FileIdentifier FromPath(string path)
        {
            var fi = new FileInfo(path);

            if(fi.Directory == null)
            {
                return new FileIdentifier(new DirectoryIdentifier(path), new FileName(string.Empty));
            }

            return new FileIdentifier(new DirectoryIdentifier(fi.Directory.FullName), new FileName(fi.Name));
        }
    }
}
