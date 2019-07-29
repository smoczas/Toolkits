using System;
using System.IO;
using CSharpToolkit.IO;
using CSharpToolkit.TypeBuilders;

namespace CSharpToolkit.Testing
{
    public class Disk : IDisposable, IGetter<string, IFile>, IGetter<string, IDirectory>
    {
        public Func<string, Stream, File> FileFactoryMethod { get; set; }

        public Func<string, Directory> DirectoryFactoryMethod { get; set; }

        public Disk()
        {
            _driver = new DiskDriver();
        }

        public File CreateFile(string path)
        {
            var result = _driver.CreateOrGetFile(path);
            return result;
        }

        public Directory CreateDirectory(string path)
        {
            var result = _driver.CreateOrGetDirectory(path);
            return result;
        }

        public void Dispose()
        {
            _driver.Dispose();
        }

        IFile IGetter<string, IFile>.Get(string path)
        {
            return _driver.GetFile(path);
        }

        IDirectory IGetter<string, IDirectory>.Get(string path)
        {
            return _driver.GetDirectory(path);
        }

        private readonly DiskDriver _driver;
    }
}
