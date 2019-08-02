namespace CSharpToolkit.Testing
{
    public static class DiskHelpers
    {
        public static void Attach(this SeamsExt seams, Disk disk)
        {
            seams.DirectoryGetter = disk;
            seams.FileGetter = disk;
        }
    }
}
