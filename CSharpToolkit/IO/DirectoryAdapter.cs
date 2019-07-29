using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CSharpToolkit.IO
{
    internal class DirectoryAdapter : FileSystemInfoAdapter<DirectoryInfo>, IDirectory
    {
        public DirectoryAdapter(DirectoryInfo di)
            : base(di)
        {
        }

        public IDirectory Parent => FileSystem.WrapToDirectory(SysInfo.Parent);

        public IDirectory Root => FileSystem.WrapToDirectory(SysInfo.Root);

        public void Create()
        {
            SysInfo.Create();
        }

        public IDirectory CreateSubdirectory(string path)
        {
            return FileSystem.WrapToDirectory(SysInfo.CreateSubdirectory(path));
        }

        public void Delete(bool recursive)
        {
            SysInfo.Delete(recursive);
        }


        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            return SysInfo.EnumerateDirectories()
                .Select(FileSystem.WrapToDirectory);
        }

        public IEnumerable<IDirectory> EnumerateDirectories(string searchPattern)
        {
            return SysInfo.EnumerateDirectories(searchPattern)
                .Select(FileSystem.WrapToDirectory);
        }

        public IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        {
            return SysInfo.EnumerateDirectories(searchPattern, searchOption)
                .Select(FileSystem.WrapToDirectory);
        }


        public IEnumerable<IFile> EnumerateFiles()
        {
            return SysInfo.EnumerateFiles()
                .Select(FileSystem.WrapToFile);
        }
        public IEnumerable<IFile> EnumerateFiles(string searchPattern)
        {
            return SysInfo.EnumerateFiles(searchPattern)
                .Select(FileSystem.WrapToFile);
        }

        public IEnumerable<IFile> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            return SysInfo.EnumerateFiles(searchPattern, searchOption)
                .Select(FileSystem.WrapToFile);
        }


        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
        {
            return SysInfo.EnumerateFileSystemInfos()
                .Select(FileSystem.WrapToFileSystemInfo);
        }

        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
        {
            return SysInfo.EnumerateFileSystemInfos(searchPattern)
                .Select(FileSystem.WrapToFileSystemInfo);
        }

        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            return SysInfo.EnumerateFileSystemInfos(searchPattern, searchOption)
                .Select(FileSystem.WrapToFileSystemInfo);
        }


        public IDirectory[] GetDirectories()
        {
            return SysInfo.GetDirectories()
                .Select(FileSystem.WrapToDirectory)
                .ToArray();
        }

        public IDirectory[] GetDirectories(string searchPattern)
        {
            return SysInfo.GetDirectories(searchPattern)
                .Select(FileSystem.WrapToDirectory)
                .ToArray();
        }

        public IDirectory[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return SysInfo.GetDirectories(searchPattern, searchOption)
                .Select(FileSystem.WrapToDirectory)
                .ToArray();
        }

        

        public IFile[] GetFiles()
        {
            return SysInfo.GetFiles()
                .Select(FileSystem.WrapToFile)
                .ToArray();
        }

        public IFile[] GetFiles(string searchPattern)
        {
            return SysInfo.GetFiles(searchPattern)
                .Select(FileSystem.WrapToFile)
                .ToArray();
        }

        public IFile[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            return SysInfo.GetFiles(searchPattern, searchOption)
                .Select(FileSystem.WrapToFile)
                .ToArray();
        }

        
        public IFileSystemInfo[] GetFileSystemInfos()
        {
            return SysInfo.GetFileSystemInfos()
                .Select(FileSystem.WrapToFileSystemInfo)
                .ToArray();
        }

        public IFileSystemInfo[] GetFileSystemInfos(string searchPattern)
        {
            return SysInfo.GetFileSystemInfos(searchPattern)
                .Select(FileSystem.WrapToFileSystemInfo)
                .ToArray();
        }

        public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            return SysInfo.GetFileSystemInfos(searchPattern, searchOption)
                .Select(FileSystem.WrapToFileSystemInfo)
                .ToArray();
        }


        public void MoveTo(string destDirName)
        {
            SysInfo.MoveTo(destDirName);
        }
    }
}
