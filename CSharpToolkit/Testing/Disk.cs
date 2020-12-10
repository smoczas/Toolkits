using System;
using CSharpToolkit.IO;
using CSharpToolkit.TypeBuilders;

namespace CSharpToolkit.Testing
{
    public class Disk : 
        IDisposable, 
        IGetter<string, IFile>, 
        IGetter<string, FakeFile>,
        IGetter<string, IDirectory>,
        IGetter<string, FakeDirectory>
    {
        public Func<string, IDiskDriver, FakeFile> FileFactoryMethod 
        {
            get => _driver.FileFactoryMethod;
            set => _driver.FileFactoryMethod = value; 
        }

        public Func<string, IDiskDriver, FakeDirectory> DirectoryFactoryMethod 
        {
            get => _driver.DirectoryFactoryMethod;
            set => _driver.DirectoryFactoryMethod = value;
        }

        public Disk()
        {
            _driver = new DiskDriver();
        }

        public FakeFile CreateFile(string path)
        {
            var result = _driver.CreateOrGetFile(path);
            return result;
        }

        public FakeFile GetFile(string path)
        {
            var result = _driver.GetFile(path);
            return result;
        }

        public FakeDirectory CreateDirectory(string path)
        {
            var result = _driver.CreateOrGetDirectory(path);
            return result;
        }

        public FakeDirectory GetDirectory(string path)
        {
            var result = _driver.GetDirectory(path);
            return result;
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        FakeFile IGetter<string, FakeFile>.Get(string path)
        {
            return _driver.GetFile(path);
        }

        IFile IGetter<string, IFile>.Get(string path)
        {
            return _driver.GetFile(path);
        }

        IDirectory IGetter<string, IDirectory>.Get(string path)
        {
            return _driver.GetDirectory(path);
        }

        FakeDirectory IGetter<string, FakeDirectory>.Get(string path)
        {
            return _driver.GetDirectory(path);
        }

        private readonly DiskDriver _driver;
    }
}
