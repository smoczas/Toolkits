using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSharpToolkit.IO
{
    public interface IFile : IFileSystemInfo
    {
        IDirectory Directory { get; }
        long Length { get; }
        bool IsReadOnly { get; set; }
        StreamWriter AppendText();
        IFile CopyTo(string destFileName);
        IFile CopyTo(string destFileName, bool overwrite);
        Stream Create();
        StreamWriter CreateText();
        void MoveTo(string destFileName);
        Stream Open(FileMode mode);
        Stream Open(FileMode mode, FileAccess access);
        Stream Open(FileMode mode, FileAccess access, FileShare share);
        Stream OpenRead();
        StreamReader OpenText();
        Stream OpenWrite();
    }
}
