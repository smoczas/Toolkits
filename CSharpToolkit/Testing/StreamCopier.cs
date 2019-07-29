using System.IO;

namespace CSharpToolkit.Testing
{
    internal class StreamCopier
    {
        public bool ResetTarget { get; set; } = false;

        public void Copy(Stream source, Stream target)
        {
            var srcPos = source.Position;
            var targPos = target.Position;
            source.Seek(0, SeekOrigin.Begin);
            source.CopyTo(target);
            source.Seek(srcPos, SeekOrigin.Begin);

            if (ResetTarget)
            {
                target.Seek(targPos, SeekOrigin.Begin);
            }
        }
    }
}
