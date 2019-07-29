using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class DiskDriverTests1
    {
        [TestMethod]
        public void CreateOrGetFile_CanCreateFile()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                // act
                var file = driver.CreateOrGetFile(@"c:\temp\file.txt");

                // assert
                Assert.IsTrue(file.Exists);
            }
        }
    }
}
