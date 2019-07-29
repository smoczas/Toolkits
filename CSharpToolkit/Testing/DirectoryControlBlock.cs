using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpToolkit.Testing
{
    internal class DirectoryControlBlock : ControlBlock, IDisposable
    {
        public string Name { get => Idendifier.ToString(); }
        public List<FileIdentifier> Files { get; }
        public List<DirectoryIdentifier> Directories { get; }
        public DirectoryIdentifier Idendifier { get; }

        public DirectoryControlBlock(DirectoryIdentifier id)
        {
            Idendifier = id;
            Files = new List<FileIdentifier>();
            Directories = new List<DirectoryIdentifier>();
            LastWriteTime = CreationTime = LastAccessTime = DateTime.Now;
            Attributes = FileAttributes.Directory;
        }

        public DirectoryControlBlock Clone(DirectoryIdentifier newId)
        {
            var result = new DirectoryControlBlock(newId);
            result.Directories.AddRange(Directories);
            result.Files.AddRange(Files);
            return result;
        }

        public void Dispose()
        {
        }
    }
}
