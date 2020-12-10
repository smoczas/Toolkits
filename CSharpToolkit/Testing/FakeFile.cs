using System;
using System.Collections.Generic;
using System.IO;
using CSharpToolkit.IO;

namespace CSharpToolkit.Testing
{
    public class FakeFile : IFile
    {
        public FakeFile(string path, IDiskDriver diskDriver)
        {
            ChangePath(path);
            _driver = diskDriver;
        }

        public virtual IDirectory Directory => _driver.GetDirectory(Path.GetDirectoryName(FullName));

        public virtual long Length => _driver.GetLength(_identifier);

        public virtual bool IsReadOnly { get => _driver.GetIsReadOnly(_identifier); set => _driver.SetIsReadOnly(_identifier, value); }

        public virtual string FullName => _fullName;

        public virtual string Name => _identifier.Name.Name;

        public virtual string Extension => _identifier.Name.Extension;

        public virtual bool Exists => _driver.Exists(_identifier);

        public virtual FileAttributes Attributes { get => _driver.GetAttributes(_identifier); set => _driver.SetAttributes(_identifier, value); }

        public virtual DateTime LastWriteTime { get => _driver.GetLastWriteTime(_identifier); set => _driver.SetLastWriteTime(_identifier, value); }
        public virtual DateTime LastWriteTimeUtc { get => DateTime.SpecifyKind(LastWriteTime, DateTimeKind.Utc); set => LastWriteTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        public virtual DateTime LastAccessTime { get => _driver.GetLastAccessTime(_identifier); set => _driver.SetLastAccessTime(_identifier, value); }
        public virtual DateTime LastAccessTimeUtc { get => DateTime.SpecifyKind(LastAccessTime, DateTimeKind.Utc); set => LastAccessTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }
        public virtual DateTime CreationTime { get => _driver.GetCreationTime(_identifier); set => _driver.SetCreationTime(_identifier, value); }
        public virtual DateTime CreationTimeUtc { get => DateTime.SpecifyKind(CreationTime, DateTimeKind.Utc); set => CreationTime = DateTime.SpecifyKind(value, DateTimeKind.Local); }

        public virtual Dictionary<object, object> CustomData { get => _driver.GetCustomData(_identifier); }

        public virtual StreamWriter AppendText()
        {
            return new StreamWriter(Open(FileMode.Append));
        }

        public virtual IFile CopyTo(string destFileName)
        {
            return CopyTo(destFileName, false);
        }

        public virtual IFile CopyTo(string destFileName, bool overwrite)
        {
            return _driver.CopyTo(_identifier, FileIdentifier.FromPath(destFileName), overwrite);
        }

        public virtual Stream Create()
        {
            return Open(FileMode.Create);
        }

        public virtual StreamWriter CreateText()
        {
            return new StreamWriter(Create());
        }

        public virtual void Delete()
        {
            _driver.Delete(_identifier);
        }

        public virtual void MoveTo(string destFileName)
        {
            _driver.MoveTo(_identifier, FileIdentifier.FromPath(destFileName));
        }

        public virtual Stream Open(FileMode mode)
        {
            return Open(mode, FileAccess.ReadWrite);
        }

        public virtual Stream Open(FileMode mode, FileAccess access)
        {
            return Open(mode, access, FileShare.None);
        }

        public virtual Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return _driver.OpenFile(_identifier, mode, access, share);
        }

        public virtual Stream OpenRead()
        {
            return Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public virtual StreamReader OpenText()
        {
            return new StreamReader(OpenRead());
        }

        public virtual Stream OpenWrite()
        {
            return Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public virtual void Refresh()
        {
        }

        public void ChangePath(string path)
        {
            _fullName = Path.GetFullPath(path);
            _identifier = FileIdentifier.FromPath(FullName);
        }


        private FileIdentifier _identifier;
        private readonly IDiskDriver _driver;
        private string _fullName;
    }
}
