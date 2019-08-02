using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpToolkit.Testing
{
    internal class FileControlBlock : ControlBlock, IDisposable
    {
        public MemoryStream Stream { get; set; }
        public FileIdentifier Identifier { get; }
        public bool IsReadOnly { get; set; }
        public Dictionary<object, object> CustomData { get; set; }


        public FileControlBlock(FileIdentifier id)
        {
            Identifier = id;
            Stream = new MemoryStream();
            LastWriteTime = CreationTime = LastAccessTime = DateTime.Now;
            CustomData = new Dictionary<object, object>();
        }

        public FileControlBlock Clone(FileIdentifier newId)
        {
            var result = new FileControlBlock(newId)
            {
                IsReadOnly = IsReadOnly,
                Attributes = Attributes,
                LastWriteTime = LastWriteTime,
                CreationTime = CreationTime,
                LastAccessTime = LastAccessTime,
            };

            var copier = new StreamCopier { ResetTarget = true };
            copier.Copy(Stream, result.Stream);

            return result;
        }

        public void Dispose()
        {
            Stream.Dispose();
        }
    }
}
