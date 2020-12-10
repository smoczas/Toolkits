using System;
using System.Collections.Generic;

namespace CSharpToolkit.Testing
{
    class IOFactory 
    {
        public Func<string, IDiskDriver, FakeFile> FileFactoryMethod { get; set; }

        public Func<string, IDiskDriver, FakeDirectory> DirectoryFactoryMethod { get; set; }

        public IOFactory()
        {
            DirectoryFactoryMethod = (p, d) => new FakeDirectory(p, d);
            FileFactoryMethod = (p, d) => new FakeFile(p, d);
            _files = new Dictionary<FileIdentifier, FakeFile>();
            _directories = new Dictionary<DirectoryIdentifier, FakeDirectory>();
        }

        public FakeFile CreateFile(string path, IDiskDriver driver)
        {
            var id = FileIdentifier.FromPath(path);
            if (_files.TryGetValue(id, out var result))
            {
                return result;
            }

            result = FileFactoryMethod(path, driver);
            _files.Add(id, result);
            return result;
        }

        public FakeDirectory CreateDirectory(string path, IDiskDriver driver)
        {
            var id = new DirectoryIdentifier(path);
            if (_directories.TryGetValue(id, out var result))
            {
                return result;
            }

            result = DirectoryFactoryMethod(path, driver);
            _directories.Add(id, result);
            return result;
        }

        public void OnFileMoved(FileIdentifier from, FileIdentifier to)
        {
            if(_files.TryGetValue(from, out var found))
            {
                _files.Remove(from);
                found.ChangePath(IdentifierHelper.ToPath(to));
                _files.Add(to, found);
            }
        }

        public void OnDirectoryMoved(DirectoryIdentifier from, DirectoryIdentifier to)
        {
            if(_directories.TryGetValue(from, out var found))
            {
                _directories.Remove(from);
                found.ChangePath(to.FullName);
                _directories.Add(to, found);
            }
        }

        private readonly Dictionary<FileIdentifier, FakeFile> _files;
        private readonly Dictionary<DirectoryIdentifier, FakeDirectory> _directories;
    }
}
