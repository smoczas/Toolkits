using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CSharpToolkit.IO
{
    public interface IDirectory : IFileSystemInfo
    {
        IDirectory Parent { get; }
        IDirectory Root { get; }

        void Create();
        IDirectory CreateSubdirectory(string path);
        
        void Delete(bool recursive);

        IEnumerable<IDirectory> EnumerateDirectories();
        IEnumerable<IDirectory> EnumerateDirectories(string searchPattern);
        IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption searchOption);
        IEnumerable<IFile> EnumerateFiles(string searchPattern, SearchOption searchOption);
        IEnumerable<IFile> EnumerateFiles(string searchPattern);
        IEnumerable<IFile> EnumerateFiles();
        IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption);
        IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos();
        IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern);
        IDirectory[] GetDirectories();
        IDirectory[] GetDirectories(string searchPattern, SearchOption searchOption);
        IDirectory[] GetDirectories(string searchPattern);
        IFile[] GetFiles();
        IFile[] GetFiles(string searchPattern, SearchOption searchOption);
        IFile[] GetFiles(string searchPattern);
        IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption);
        IFileSystemInfo[] GetFileSystemInfos();
        IFileSystemInfo[] GetFileSystemInfos(string searchPattern);
        void MoveTo(string destDirName);
    }
}
