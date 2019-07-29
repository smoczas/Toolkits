using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class DirectoryCreateTests
    {
        [TestMethod]
        public void Create_CanCreate()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var f = driver.GetDirectory(@"c:\temp");

                // act
                f.Create();

                // assert
                f.Refresh();
                Assert.IsTrue(f.Exists);
            }
        }

        [TestMethod]
        public void Create_FileWithTheSameNameAlreadyExists_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\folder");
                var folder = driver.GetDirectory(@"c:\folder");

                // act/assert
                Assert.ThrowsException<IOException>(() => folder.Create());
            }
        }

        [TestMethod]
        public void CreateSubdirectory_CanCreate()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder = driver.CreateOrGetDirectory(@"c:\folder");

                // act
                var subfolder = folder.CreateSubdirectory(@"subfolder");

                // assert
                Assert.IsTrue(subfolder.Exists);
                Assert.AreEqual(@"c:\folder\subfolder", subfolder.FullName);
            }
        }

        [TestMethod]
        public void CreateSubdirectory_PathContainsDots_CanCraeate()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder = driver.CreateOrGetDirectory(@"c:\folder");

                // act
                var subfolder = folder.CreateSubdirectory(@".\..\folder\.\subfolder");

                // assert
                Assert.IsTrue(subfolder.Exists);
                Assert.AreEqual(@"c:\folder\subfolder", subfolder.FullName);
            }
        }

        [TestMethod]
        public void CreateSubdirectory_PathIsNotSubdir_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder = driver.CreateOrGetDirectory(@"c:\folder");

                // act/assert
                Assert.ThrowsException<ArgumentException>(() => folder.CreateSubdirectory(@"..\subfolder"));
            }
        }

        [TestMethod]
        public void CreateSubdirectory_PathIsAbsolute_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder = driver.CreateOrGetDirectory(@"c:\folder");

                // act/assert
                Assert.ThrowsException<ArgumentException>(() => folder.CreateSubdirectory(@"c:\folder\subfolder"));
            }
        }

        [TestMethod]
        public void CreateSubdirectory_FileWithTheSameNameAlreadyExists_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\folder\subfolder");
                var folder = driver.GetDirectory(@"c:\folder");

                // act/assert
                Assert.ThrowsException<IOException>(() => folder.CreateSubdirectory(@"subfolder"));
            }
        }
    }
}
