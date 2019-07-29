using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class FilePropertiesTests
    {
        [TestMethod]
        public void SetIsReadOnly_FileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() => file.IsReadOnly = true);
            }
        }

        [TestMethod]
        public void SetIsReadOnly_DirDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => file.IsReadOnly = true);
            }
        }

        [TestMethod]
        public void SetIsReadOnly_FileExist_CanSet()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file1 = driver.CreateOrGetFile(@"c:\temp\file.txt");
                file1.IsReadOnly = false;


                var file2 = driver.GetFile(@"c:\temp\file.txt");
                Assert.IsFalse(file2.IsReadOnly);

                file1.IsReadOnly = true;
                file2.Refresh();
                Assert.IsTrue(file2.IsReadOnly);
            }
        }

        [TestMethod]
        public void GetIsReadOnly_FileDoesntExist_ReturnsTrue()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.IsTrue(file.IsReadOnly);
            }
        }

        [TestMethod]
        public void GetLength_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => file.Length);
            }
        }

        [TestMethod]
        public void GetLength_FileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() => file.Length);
            }
        }

        [TestMethod]
        public void GetLength_FileExist_CanReturnLength()
        {
            // arrange
            using (var driver = new DiskDriver())
            using (var ms = new MemoryStream(new byte[] { 1, 2, 3, 4 }))
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                using (var fs = file.OpenWrite())
                {
                    ms.CopyTo(fs);
                }

                file.Refresh();

                // assert
                Assert.AreEqual(ms.Length, file.Length);
            }
        }

        [TestMethod]
        public void SetAttributes_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => file.Attributes = FileAttributes.Hidden);
            }
        }

        [TestMethod]
        public void SetAttributes_FileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() => file.Attributes = FileAttributes.Hidden);
            }
        }

        [TestMethod]
        public void SetAttributes_FileExists_CanSetAttributes()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.CreateOrGetFile(@"c:\temp\file.txt");

                // act
                file = driver.GetFile(@"c:\temp\file.txt");
                file.Attributes = FileAttributes.Hidden;

                // assert
                Assert.AreEqual(FileAttributes.Hidden, file.Attributes);
            }
        }

        [TestMethod]
        public void SetLastAccessTime_FileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() => file.LastAccessTime = DateTime.Now);
            }
        }

        [TestMethod]
        public void SetLastAccessTime_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => file.LastAccessTime = DateTime.Now);
            }
        }

        [TestMethod]
        public void SetLastAccessTime_CanSet()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.CreateOrGetFile(@"c:\temp\file.txt");
                var time = DateTime.Now.AddMinutes(-5);

                // act
                file.LastAccessTime = time;

                // assert
                Assert.AreEqual(time, file.LastAccessTime);
            }
        }

        [TestMethod]
        public void SetLastWriteTime_FileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() => file.LastWriteTime = DateTime.Now);
            }
        }

        [TestMethod]
        public void SetLastWriteTime_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => file.LastWriteTime = DateTime.Now);
            }
        }

        [TestMethod]
        public void SetLastWriteTime_CanSet()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.CreateOrGetFile(@"c:\temp\file.txt");
                var time = DateTime.Now.AddMinutes(-5);

                // act
                file.LastWriteTime = time;

                // assert
                Assert.AreEqual(time, file.LastWriteTime);
            }
        }

        [TestMethod]
        public void SetCreationTime_FileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() => file.CreationTime = DateTime.Now);
            }
        }

        [TestMethod]
        public void SetCreationTime_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => file.CreationTime = DateTime.Now);
            }
        }

        [TestMethod]
        public void SetCreationTime_CanSet()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.CreateOrGetFile(@"c:\temp\file.txt");
                var time = DateTime.Now.AddMinutes(-5);

                // act
                file.CreationTime = time;

                // assert
                Assert.AreEqual(time, file.CreationTime);
            }
        }
    }
}
