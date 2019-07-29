using System;
using System.IO;

namespace CSharpToolkit.Testing
{
    internal class ControlBlock
    {
        public FileAttributes Attributes { get; set; }

        public DateTime LastWriteTime { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastAccessTime { get; set; }
    }
}
