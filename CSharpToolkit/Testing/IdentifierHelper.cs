using System.IO;
using CSharpToolkit.IO;

namespace CSharpToolkit.Testing
{
    static class IdentifierHelper
    {
        public static string ToPath(FileIdentifier id)
        {
            return Path.Combine(id.OwningDirectory.FullName, id.Name.Name);
        }

        public static DirectoryIdentifier Parent(DirectoryIdentifier id)
        {
            var di = new DirectoryInfo(id.FullName);
            if(di.Parent == null)
            {
                return null;
            }
            return new DirectoryIdentifier(di.Parent.FullName);
        }

        public static DirectoryIdentifier Root(DirectoryIdentifier id)
        {
            var di = new DirectoryInfo(id.FullName);
            return new DirectoryIdentifier(di.Root.FullName);
        }

        public static DirectoryIdentifier ConvertToDirId(FileIdentifier id)
        {
            var path = ToPath(id);
            return new DirectoryIdentifier(path);
        }

        public static FileIdentifier ConvertToFileId(DirectoryIdentifier id)
        {
            var path = id.FullName;
            return FileIdentifier.FromPath(path);
        }

        public static FileIdentifier CombineFileId(DirectoryIdentifier id, string path)
        {
            var newPath = Path.Combine(id.FullName, path);
            return FileIdentifier.FromPath(newPath);
        }

        public static DirectoryIdentifier CombineDirId(DirectoryIdentifier id, string path)
        {
            var newPath = Path.Combine(id.FullName, path);
            return new DirectoryIdentifier(newPath);
        }

        public static bool GetRelative(DirectoryIdentifier from, FileIdentifier to, out string relPath)
        {
            var f = from.FullName;
            var t = ToPath(to);

            return PathHelper.GetRelativePath(f, true, t, false, out relPath);
        }

        public static bool GetRelative(DirectoryIdentifier from, DirectoryIdentifier to, out string relPath)
        {
            var f = from.FullName;
            var t = to.FullName;

            return PathHelper.GetRelativePath(f, true, t, true, out relPath);
        }
    }
}
