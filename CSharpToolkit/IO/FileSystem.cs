using System.IO;

namespace CSharpToolkit.IO
{
    public static class FileSystem
    {
        public static IDirectory GetDirectory(string path)
        {
            return new DirectoryAdapter(new DirectoryInfo(path));
        }

        public static IFile GetFile(string path)
        {
            return new FileAdapter(new FileInfo(path));
        }

        internal static IDirectory WrapToDirectory(DirectoryInfo di)
        {
            return new DirectoryAdapter(di);
        }

        internal static IFile WrapToFile(FileInfo fi)
        {
            return new FileAdapter(fi);
        }

        internal static IFileSystemInfo WrapToFileSystemInfo(FileSystemInfo fsi)
        {
            return new FileSystemInfoAdapter<FileSystemInfo>(fsi);
        }
    }
}
