using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSharpToolkit.IO
{
    public interface IFileSystemInfo
    {
        DateTime LastWriteTime { get; set; }
        DateTime LastWriteTimeUtc { get; set; }
        DateTime LastAccessTimeUtc { get; set; }
        DateTime LastAccessTime { get; set; }
        DateTime CreationTimeUtc { get; set; }
        DateTime CreationTime { get; set; }

        string FullName { get; }
        string Name { get; }
        string Extension { get; }

        bool Exists { get; }

        FileAttributes Attributes { get; set; }
        void Delete();
        void Refresh();
    }
}
