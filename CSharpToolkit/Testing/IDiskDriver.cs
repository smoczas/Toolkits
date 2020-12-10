using System;
using System.Collections.Generic;
using System.IO;

namespace CSharpToolkit.Testing
{
    public interface IDiskDriver
    {
        bool Exists(FileIdentifier file);
        bool Exists(DirectoryIdentifier dir);

        void SetIsReadOnly(FileIdentifier id, bool value);
        bool GetIsReadOnly(FileIdentifier id);
        long GetLength(FileIdentifier id);
        FileAttributes GetAttributes(FileIdentifier id);
        FileAttributes GetAttributes(DirectoryIdentifier id);
        void SetAttributes(FileIdentifier id, FileAttributes attr);
        void SetAttributes(DirectoryIdentifier id, FileAttributes attr);

        DateTime GetLastWriteTime(FileIdentifier id);
        void SetLastWriteTime(FileIdentifier id, DateTime time);
        DateTime GetCreationTime(FileIdentifier id);
        void SetCreationTime(FileIdentifier id, DateTime time);
        DateTime GetLastAccessTime(FileIdentifier id);
        void SetLastAccessTime(FileIdentifier id, DateTime time);

        DateTime GetLastWriteTime(DirectoryIdentifier id);
        void SetLastWriteTime(DirectoryIdentifier id, DateTime time);
        DateTime GetCreationTime(DirectoryIdentifier id);
        void SetCreationTime(DirectoryIdentifier id, DateTime time);
        DateTime GetLastAccessTime(DirectoryIdentifier id);
        void SetLastAccessTime(DirectoryIdentifier id, DateTime time);

        Dictionary<object, object> GetCustomData(FileIdentifier id);

        FakeDirectory GetDirectory(string path);

        void Delete(FileIdentifier id);
        void Delete(DirectoryIdentifier id, bool recurse);
        FakeFile CopyTo(FileIdentifier source, FileIdentifier target, bool overwrite);
        void MoveTo(FileIdentifier source, FileIdentifier target);
        void MoveTo(DirectoryIdentifier source, DirectoryIdentifier target);

        Stream OpenFile(FileIdentifier id, FileMode mode, FileAccess access, FileShare share);

        FakeFile[] GetFiles(DirectoryIdentifier id, string searchPattern, SearchOption searchOptions);
        FakeDirectory[] GetDirectories(DirectoryIdentifier id, string searchPattern, SearchOption searchOptions);
        FakeDirectory CreateOrGetDirectory(string path);
        FakeDirectory CreateSubdirectory(DirectoryIdentifier id, string path);

    }
}
