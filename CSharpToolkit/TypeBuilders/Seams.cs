using System;
using System.Threading;
using System.Diagnostics;
using CSharpToolkit.IO;

namespace CSharpToolkit.TypeBuilders
{
    public class Seams
    {
        public Seams Global { get; } = new Seams();

        public bool AssertCalls { get; set; }

        public virtual IFile GetFile(string path)
        {
            Assert();
            return FileSystem.GetFile(path);
        }

        public virtual IDirectory GetDirectory(string path)
        {
            Assert();
            return FileSystem.GetDirectory(path);
        }

        public virtual T GetDependency<T>(params object[] args) where T : class
        {
            Assert();
            return default;
        }

        public virtual DateTime GetNow()
        {
            return DateTime.Now; 
        }

        public virtual DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }

        public virtual void Sleep(int ms)
        {
            Thread.Sleep(ms);
        }

        protected Seams()
        {
        }

        private void Assert()
        {
            if(AssertCalls)
            {
                Debug.Assert(false, "Unintended call?", $"Probably not intended call to real dependency. Otherwise disable asserting by setting {nameof(AssertCalls)}");
            }
        }
    }
}
