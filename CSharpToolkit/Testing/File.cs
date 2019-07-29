using System;
using System.IO;
using CSharpToolkit.IO;

namespace CSharpToolkit.Testing
{
    public class File : IFile
    {
        public File(string path, IDiskDriver diskDriver)
        {
            ChangePath(path);
            _driver = diskDriver;
        }

        public IDirectory Directory => _driver.GetDirectory(Path.GetDirectoryName(FullName));

        public long Length => _driver.GetLength(_identifier);

        public bool IsReadOnly { get => _driver.GetIsReadOnly(_identifier); set => _driver.SetIsReadOnly(_identifier, value); }

        public string FullName { get; private set; }

        public string Name => _identifier.Name.Name;

        public string Extension => _identifier.Name.Extension;

        public bool Exists => _driver.Exists(_identifier);

        public FileAttributes Attributes { get => _driver.GetAttributes(_identifier); set => _driver.SetAttributes(_identifier, value); }

        public DateTime LastWriteTime { get => _driver.GetLastWriteTime(_identifier); set => _driver.SetLastWriteTime(_identifier, value); }
        public DateTime LastWriteTimeUtc { get => DateTime.SpecifyKind(LastWriteTime, DateTimeKind.Utc); set => LastWriteTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        public DateTime LastAccessTime { get => _driver.GetLastAccessTime(_identifier); set => _driver.SetLastAccessTime(_identifier, value); }
        public DateTime LastAccessTimeUtc { get => DateTime.SpecifyKind(LastAccessTime, DateTimeKind.Utc); set => LastAccessTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        public DateTime CreationTime { get => _driver.GetCreationTime(_identifier); set => _driver.SetCreationTime(_identifier, value); }
        public DateTime CreationTimeUtc { get => DateTime.SpecifyKind(CreationTime, DateTimeKind.Utc); set => CreationTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }

        public StreamWriter AppendText()
        {
            return new StreamWriter(Open(FileMode.Append));
        }

        public IFile CopyTo(string destFileName)
        {
            return CopyTo(destFileName, false);
        }

        public IFile CopyTo(string destFileName, bool overwrite)
        {
            return _driver.CopyTo(_identifier, FileIdentifier.FromPath(destFileName), overwrite);
        }

        public Stream Create()
        {
            return Open(FileMode.Create);
        }

        public StreamWriter CreateText()
        {
            return new StreamWriter(OpenWrite());
        }

        public void Delete()
        {
            _driver.Delete(_identifier);
        }

        public void MoveTo(string destFileName)
        {
            _driver.MoveTo(_identifier, FileIdentifier.FromPath(destFileName));
        }

        public Stream Open(FileMode mode)
        {
            return Open(mode, FileAccess.ReadWrite);
        }

        public Stream Open(FileMode mode, FileAccess access)
        {
            return Open(mode, access, FileShare.None);
        }

        public Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return _driver.OpenFile(_identifier, mode, access, share);
        }

        public Stream OpenRead()
        {
            return Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public StreamReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        public Stream OpenWrite()
        {
            return Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public void Refresh()
        {
        }

        public void ChangePath(string path)
        {
            FullName = Path.GetFullPath(path);
            _identifier = FileIdentifier.FromPath(FullName);
        }

        private FileIdentifier _identifier;
        private readonly IDiskDriver _driver;
    }
}
