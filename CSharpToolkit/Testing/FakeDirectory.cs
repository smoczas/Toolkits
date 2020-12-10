using System;
using System.Collections.Generic;
using System.IO;
using CSharpToolkit.IO;

namespace CSharpToolkit.Testing
{
    public class FakeDirectory : IDirectory
    {
        public FakeDirectory(string path, IDiskDriver driver)
        {
            _driver = driver;
            ChangePath(path);
        }

        public virtual IDirectory Parent
        {
            get
            {
                var parentId = IdentifierHelper.Parent(_id);
                return parentId == null ? null : _driver.GetDirectory(parentId.FullName);
            }
        }

        public virtual IDirectory Root
        {
            get
            {
                var rootId = IdentifierHelper.Root(_id);
                return _driver.GetDirectory(rootId.FullName);
            }
        }

        public virtual string FullName => _displayedPath;

        public virtual string Name => Path.GetFileName(_displayedPath);

        public virtual string Extension => Path.GetExtension(Name);

        public virtual bool Exists => _driver.Exists(_id);

        public virtual FileAttributes Attributes { get => _driver.GetAttributes(_id); set => _driver.SetAttributes(_id, value); }

        public virtual DateTime LastWriteTime { get => _driver.GetLastWriteTime(_id); set => _driver.SetLastWriteTime(_id, value); }
        public virtual DateTime LastWriteTimeUtc { get => DateTime.SpecifyKind(LastWriteTime, DateTimeKind.Utc); set => LastWriteTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        public virtual DateTime LastAccessTime { get => _driver.GetLastAccessTime(_id); set => _driver.SetLastAccessTime(_id, value); }
        public virtual DateTime LastAccessTimeUtc { get => DateTime.SpecifyKind(LastAccessTime, DateTimeKind.Utc); set => LastAccessTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        public virtual DateTime CreationTime { get => _driver.GetCreationTime(_id); set => _driver.SetCreationTime(_id, value); }
        public virtual DateTime CreationTimeUtc { get => DateTime.SpecifyKind(CreationTime, DateTimeKind.Utc); set => CreationTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }

        public virtual void Create()
        {
            _driver.CreateOrGetDirectory(FullName);
        }

        public virtual IDirectory CreateSubdirectory(string path)
        {
            return _driver.CreateSubdirectory(_id, path);
        }

        public virtual void Delete(bool recursive)
        {
            _driver.Delete(_id, recursive);
        }

        public virtual void Delete()
        {
            Delete(false);
        }

        public virtual IEnumerable<IDirectory> EnumerateDirectories()
        {
            return GetDirectories();
        }

        public virtual IEnumerable<IDirectory> EnumerateDirectories(string searchPattern)
        {
            return GetDirectories(searchPattern);
        }

        public virtual IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        {
            return GetDirectories(searchPattern, searchOption);
        }

        public virtual IEnumerable<IFile> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            return GetFiles(searchPattern, searchOption);
        }

        public virtual IEnumerable<IFile> EnumerateFiles(string searchPattern)
        {
            return GetFiles(searchPattern);
        }

        public virtual IEnumerable<IFile> EnumerateFiles()
        {
            return GetFiles();
        }

        public virtual IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public virtual IDirectory[] GetDirectories()
        {
            return GetDirectories("*", SearchOption.TopDirectoryOnly);
        }

        public virtual IDirectory[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            return _driver.GetDirectories(_id, searchPattern, searchOption);
        }

        public virtual IDirectory[] GetDirectories(string searchPattern)
        {
            return GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);
        }

        public virtual IFile[] GetFiles()
        {
            return GetFiles("*", SearchOption.TopDirectoryOnly);
        }

        public virtual IFile[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            return _driver.GetFiles(_id, searchPattern, searchOption);
        }

        public virtual IFile[] GetFiles(string searchPattern)
        {
            return GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
        }

        public virtual IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public virtual IFileSystemInfo[] GetFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public virtual IFileSystemInfo[] GetFileSystemInfos(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public virtual void MoveTo(string destDirName)
        {
            var newId = new DirectoryIdentifier(destDirName);
            _driver.MoveTo(_id, newId);
        }

        public virtual void Refresh()
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
