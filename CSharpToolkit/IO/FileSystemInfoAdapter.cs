using System;
using System.IO;

namespace CSharpToolkit.IO
{
    internal class FileSystemInfoAdapter<T> : IFileSystemInfo where T : FileSystemInfo
        
    {
        public FileSystemInfoAdapter(T sysInfo)
        {
            SysInfo = sysInfo;
        }

        public string FullName => SysInfo.FullName;

        public string Name => SysInfo.Name;

        public string Extension => SysInfo.Extension;

        public bool Exists => SysInfo.Exists;

        public FileAttributes Attributes
        {
            get => SysInfo.Attributes;
            set => SysInfo.Attributes = value;
        }

        public DateTime LastWriteTime { get => SysInfo.LastWriteTime; set => SysInfo.LastWriteTime = value; }
        public DateTime LastWriteTimeUtc { get => SysInfo.LastWriteTimeUtc; set => SysInfo.LastWriteTimeUtc = value; }
        public DateTime LastAccessTimeUtc { get => SysInfo.LastAccessTime; set => SysInfo.LastAccessTime = value; }
        public DateTime LastAccessTime { get => SysInfo.LastAccessTimeUtc; set => SysInfo.LastAccessTimeUtc = value; }
        public DateTime CreationTimeUtc { get => SysInfo.CreationTimeUtc; set => SysInfo.CreationTimeUtc = value; }
        public DateTime CreationTime { get => SysInfo.CreationTime; set => SysInfo.CreationTime = value; }

        public void Delete()
        {
            SysInfo.Delete();
        }

        public void Refresh()
        {
            SysInfo.Refresh();
        }

        public override string ToString()
        {
            return SysInfo.ToString();
        }

        protected T SysInfo { get; }
        
    }
}
