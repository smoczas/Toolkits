using System;
using CSharpToolkit.IO;
using CSharpToolkit.TypeBuilders;

namespace CSharpToolkit.Testing
{
    public class EditableSeams : Seams
    {
        public EditableSeams()
        {
            DependencyContainer = new DependencyContainer();
            var emptyGetter = new EmptyGetter();
            FileGetter = emptyGetter;
            DirectoryGetter = emptyGetter;
        }

        public DependencyContainer DependencyContainer { get; set; }

        public override IDirectory GetDirectory(string path)
        {
            return DirectoryGetter.Get(path);
        }

        public IGetter<string, IDirectory> DirectoryGetter { get; set; }


        public override IFile GetFile(string path)
        {
            return FileGetter.Get(path);
        }

        public IGetter<string, IFile> FileGetter { get; set; }


        public override void Sleep(int ms)
        {
            SleepAction(ms);
        }

        public Action<int> SleepAction { get; set; }


        public override DateTime GetNow()
        {
            return Now();
        }

        public Func<DateTime> Now { get; set; }


        public override DateTime GetUtcNow()
        {
            return UtcNow();
        }

        public Func<DateTime> UtcNow { get; set; }
    }
}
