using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class FileDeleteTests
    {
        [TestMethod]
        public void Delete_ForFile_DirDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => file.Delete());
            }
        }

        [TestMethod]
        public void Delete_ForFile_FileDoesntExist_DoesNothing()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act
                file.Delete();

                // assert
                file.Refresh();
                Assert.IsFalse(file.Exists);
            }
        }

        [TestMethod]
        public void Delete_ForFile_FileExists_CanRemoveFile()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.CreateOrGetFile(@"c:\temp\file.txt");
                Assert.IsTrue(file.Exists);

                // act
                file.Delete();

                // assert
                file.Refresh();
                Assert.IsFalse(file.Exists);
            }
        }

        [TestMethod]
        public void Delete_ForFile_DirectoryWithTheSameNameAlreadyExists_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\file");
                var f = driver.GetFile(@"c:\file");

                // assert
                Assert.ThrowsException<UnauthorizedAccessException>(() => f.Delete());
            }
        }
    }
}
