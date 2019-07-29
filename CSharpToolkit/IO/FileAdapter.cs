using System.IO;

namespace CSharpToolkit.IO
{
    internal class FileAdapter : FileSystemInfoAdapter<FileInfo>, IFile
    {
        public FileAdapter(FileInfo fi)
            : base(fi)
        {
        }

        public IDirectory Directory => new DirectoryAdapter(SysInfo.Directory);

        public long Length => SysInfo.Length;

        public bool IsReadOnly
        {
            get => SysInfo.IsReadOnly;
            set => SysInfo.IsReadOnly = value;
        }

        public StreamWriter AppendText()
        {
            return SysInfo.AppendText();
        }

        public IFile CopyTo(string destFileName)
        {
            return new FileAdapter(SysInfo.CopyTo(destFileName));
        }

        public IFile CopyTo(string destFileName, bool overwrite)
        {
            return new FileAdapter(SysInfo.CopyTo(destFileName, overwrite));
        }

        public Stream Create()
        {
            return SysInfo.Create();
        }

        public StreamWriter CreateText()
        {
            return SysInfo.CreateText();
        }

        public void MoveTo(string destFileName)
        {
            SysInfo.MoveTo(destFileName);
        }

        public Stream Open(FileMode mode)
        {
            return SysInfo.Open(mode);
        }

        public Stream Open(FileMode mode, FileAccess access)
        {
            return SysInfo.Open(mode, access);
        }

        public Stream Open(FileMode mode, FileAccess access, FileShare share)
        {
            return SysInfo.Open(mode, access, share);
        }

        public Stream OpenRead()
        {
            return SysInfo.OpenRead();
        }

        public StreamReader OpenText()
        {
            return SysInfo.OpenText();
        }

        public Stream OpenWrite()
        {
            return SysInfo.OpenWrite();
        }
    }
}
