using System;
using System.Collections.Generic;
using System.IO;
using CSharpToolkit.IO;

namespace CSharpToolkit.Testing
{
    public class Directory : IDirectory
    {
        public Directory(string path, IDiskDriver driver)
        {
            _driver = driver;
            ChangePath(path);
        }

        public IDirectory Parent
        {
            get
            {
                var parentId = IdentifierHelper.Parent(_id);
                return parentId == null ? null : _driver.GetDirectory(parentId.FullName);
            }
        }

        public IDirectory Root
        {
            get
            {
                var rootId = IdentifierHelper.Root(_id);
                return _driver.GetDirectory(rootId.FullName);
            }
        }

        public string FullName => _displayedPath;

        public string Name => Path.GetFileName(_displayedPath);

        public string Extension => Path.GetExtension(Name);

        public bool Exists => _driver.Exists(_id);

        public FileAttributes Attributes { get => _driver.GetAttributes(_id); set => _driver.SetAttributes(_id, value); }

        public DateTime LastWriteTime { get => _driver.GetLastWriteTime(_id); set => _driver.SetLastWriteTime(_id, value); }
        public DateTime LastWriteTimeUtc { get => DateTime.SpecifyKind(LastWriteTime, DateTimeKind.Utc); set => LastWriteTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        public DateTime LastAccessTime { get => _driver.GetLastAccessTime(_id); set => _driver.SetLastAccessTime(_id, value); }
        public DateTime LastAccessTimeUtc { get => DateTime.SpecifyKind(LastAccessTime, DateTimeKind.Utc); set => LastAccessTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        public DateTime CreationTime { get => _driver.GetCreationTime(_id); set => _driver.SetCreationTime(_id, value); }
        public DateTime CreationTimeUtc { get => DateTime.SpecifyKind(CreationTime, DateTimeKind.Utc); set => CreationTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }

        public void Create()
        {
            _driver.CreateOrGetDirectory(FullName);
        }

        public IDirectory CreateSubdirectory(string path)
        {
            return _driver.CreateSubdirectory(_id, path);
        }

        public void Delete(bool recursive)
        {
            _driver.Delete(_id, recursive);
        }

        public void Delete()
        {
            Delete(false);
        }

        public IEnumerable<IDirectory> EnumerateDirectories()
        {
            return GetDirectories();
        }

        public IEnumerable<IDirectory> EnumerateDirectories(string searchPattern)
        {
            return GetDirectories(searchPattern);
        }

        public IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        {
            return GetDirectories(searchPattern, searchOption);
        }

        public IEnumerable<IFile> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            return GetFiles(searchPattern, searchOption);
        }

        public IEnumerable<IFile> EnumerateFiles(string searchPattern)
        {
            return GetFiles(searchPattern);
        }

        public IEnumerable<IFile> EnumerateFiles()
        {
            return GetFiles();
        }

        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public IDirectory[] GetDirectories()
        {
            return GetDirectories("*", SearchOption.TopDirectoryOnly);
        }

        public IDirectory[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return _driver.GetDirectories(_id, searchPattern, searchOption);
        }

        public IDirectory[] GetDirectories(string searchPattern)
        {
            return GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);
        }

        public IFile[] GetFiles()
        {
            return GetFiles("*", SearchOption.TopDirectoryOnly);
        }

        public IFile[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            return _driver.GetFiles(_id, searchPattern, searchOption);
        }

        public IFile[] GetFiles(string searchPattern)
        {
            return GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
        }

        public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public IFileSystemInfo[] GetFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public IFileSystemInfo[] GetFileSystemInfos(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(string destDirName)
        {
            var newId = new DirectoryIdentifier(destDirName);
            _driver.MoveTo(_id, newId);
        }

        public void Refresh()
        {
        }

        internal void ChangePath(string path)
        {
            _displayedPath = Path.GetFullPath(path);
            _id = new DirectoryIdentifier(path);
        }

        private string _displayedPath;
        private DirectoryIdentifier _id;
        private readonly IDiskDriver _driver;
    }
}
