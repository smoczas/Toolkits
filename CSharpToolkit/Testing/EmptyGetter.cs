using System;
using CSharpToolkit.IO;
using CSharpToolkit.TypeBuilders;

namespace CSharpToolkit.Testing
{
    class EmptyGetter : IGetter<string, IFile>, IGetter<string, IDirectory>
    {
        IFile IGetter<string, IFile>.Get(string a1)
        {
            throw new NotSupportedException("sorry, no IO operations unless your register getter");
        }

        IDirectory IGetter<string, IDirectory>.Get(string a1)
        {
            throw new NotSupportedException("sorry, no IO operations unless your register getter");
        }
    }
}
