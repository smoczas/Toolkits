using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class DirectoryPropertiesTests
    {
        [TestMethod]
        public void Parent_DirectoryIsRoot_ReturnsNull()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var c = driver.CreateOrGetDirectory(@"c:\");

                // assert
                Assert.IsNull(c.Parent);
            }
        }

        [TestMethod]
        public void Parent_ReturnsParent()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var f = driver.CreateOrGetDirectory(@"c:\folder");

                // assert
                Assert.AreEqual(@"c:\", f.Parent.FullName);
            }
        }

        [TestMethod]
        public void Root_ReturnsRoot()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var f = driver.CreateOrGetDirectory(@"c:\folder\subfolder");

                // assert
                Assert.AreEqual(@"c:\", f.Root.FullName);
            }
        }

        [TestMethod]
        public void TimeStamps_NonexistingFile_ReturnsMinValue()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var expected = DateTime.MinValue;
                var f = driver.GetDirectory(@"c:\folder");

                // Assert
                Assert.IsTrue(f.LastAccessTime == expected);
                Assert.IsTrue(f.LastWriteTime == expected);
                Assert.IsTrue(f.CreationTime == expected);
            }
        }

        [TestMethod]
        public void TimeStamps_NewFile_ReturnsCorrectTimestamps()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var beforeCreate = DateTime.Now;

                var f = driver.CreateOrGetDirectory(@"c:\folder");

                // Assert
                Assert.IsTrue(f.LastAccessTime >= beforeCreate);
                Assert.IsTrue(f.LastWriteTime >= beforeCreate);
                Assert.IsTrue(f.CreationTime >= beforeCreate);
            }
        }

        [TestMethod]
        public void TimeStamps_CreateOrGetDirectory_ReturnsCorrectTimestamps()
        {
            // arrange
            using (var driver = new DiskDriver())
            {

                var beforeCreate = DateTime.Now;
                var folder = driver.CreateOrGetDirectory(@"c:\folder");

                var old = beforeCreate.AddMinutes(-5);
                folder.CreationTime = folder.LastAccessTime = folder.LastWriteTime = old;

                Assert.IsTrue(folder.LastAccessTime == old);
                Assert.IsTrue(folder.LastWriteTime == old);
                Assert.IsTrue(folder.CreationTime == old);

                var subfolder = driver.CreateOrGetDirectory(@"c:\folder\subfolder");

                // Assert
                Assert.IsTrue(folder.LastAccessTime >= beforeCreate);
                Assert.IsTrue(folder.LastWriteTime >= beforeCreate);
                Assert.IsTrue(folder.CreationTime >= beforeCreate);
            }
        }

        [TestMethod]
        public void TimeStamps_CreateFileStream_CanUpdateTimestamps()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var beforeCreate = DateTime.Now;

                var f = driver.CreateOrGetDirectory(@"c:\folder");

                var old = beforeCreate.AddMinutes(-5);
                f.LastAccessTime = f.LastWriteTime = f.CreationTime = old;

                Assert.IsTrue(f.LastAccessTime == old);
                Assert.IsTrue(f.LastWriteTime == old);
                Assert.IsTrue(f.CreationTime == old);

                // act
                var file = driver.GetFile(@"c:\folder\file.txt");
                using (file.Create()) { }

                f.Refresh();

                // Assert
                Assert.IsTrue(f.LastAccessTime >= beforeCreate);
                Assert.IsTrue(f.LastWriteTime >= beforeCreate);
                Assert.IsTrue(f.CreationTime == old);
            }
        }

        [TestMethod]
        public void Timestamps_CreateOrGetFile_CanUpdatestamps()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var beforeCreate = DateTime.Now;

                var f = driver.CreateOrGetDirectory(@"c:\folder");

                var old = beforeCreate.AddMinutes(-5);
                f.LastAccessTime = f.LastWriteTime = f.CreationTime = old;

                Assert.IsTrue(f.LastAccessTime == old);
                Assert.IsTrue(f.LastWriteTime == old);
                Assert.IsTrue(f.CreationTime == old);

                // act
                var file = driver.CreateOrGetFile(@"c:\folder\file.txt");

                f.Refresh();

                // Assert
                Assert.IsTrue(f.LastAccessTime >= beforeCreate);
                Assert.IsTrue(f.LastWriteTime >= beforeCreate);
                Assert.IsTrue(f.CreationTime == old);
            }
        }

        [TestMethod]
        public void Timestamps_CreateFileStream_GrandparentDirectoryIsntUpdated()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var beforeCreate = DateTime.Now;

                var f = driver.CreateOrGetDirectory(@"c:\parent\subfolder");
                var parent = driver.GetDirectory(@"c:\parent");

                var expected = beforeCreate.AddMinutes(-5);
                parent.LastAccessTime = parent.LastWriteTime = parent.CreationTime = expected;

                Assert.IsTrue(parent.LastAccessTime == expected);
                Assert.IsTrue(parent.LastWriteTime == expected);
                Assert.IsTrue(parent.CreationTime == expected);

                // act
                var file = driver.GetFile(@"c:\parent\subfolder\file.txt");
                using (file.Create()) { }

                // Assert
                Assert.IsTrue(parent.LastAccessTime == expected);
                Assert.IsTrue(parent.LastWriteTime == expected);
                Assert.IsTrue(parent.CreationTime == expected);
            }
        }

        [TestMethod]
        public void Timestamps_CreateOrGetFile_GrandparentDirectoryIsntUpdated()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var beforeCreate = DateTime.Now;

                var f = driver.CreateOrGetDirectory(@"c:\parent\subfolder");
                var parent = driver.GetDirectory(@"c:\parent");

                var expected = beforeCreate.AddMinutes(-5);
                parent.LastAccessTime = parent.LastWriteTime = parent.CreationTime = expected;

                Assert.IsTrue(parent.LastAccessTime == expected);
                Assert.IsTrue(parent.LastWriteTime == expected);
                Assert.IsTrue(parent.CreationTime == expected);

                // act
                var file = driver.CreateOrGetFile(@"c:\parent\subfolder\file.txt");

                // Assert
                Assert.IsTrue(parent.LastAccessTime == expected);
                Assert.IsTrue(parent.LastWriteTime == expected);
                Assert.IsTrue(parent.CreationTime == expected);
            }
        }

        [TestMethod]
        public void Timestamps_DeleteFile_UpdateTimestamps()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\folder\file.txt");

                var folder = driver.GetDirectory(@"c:\folder");

                var beforeDelete = DateTime.Now;
                var old = beforeDelete.AddMinutes(-5);
                folder.LastAccessTime = folder.LastWriteTime = folder.CreationTime = old;

                Assert.IsTrue(folder.LastAccessTime == old);
                Assert.IsTrue(folder.LastWriteTime == old);
                Assert.IsTrue(folder.CreationTime == old);

                // act
                var file = driver.GetFile(@"c:\folder\file.txt");
                file.Delete();

                folder.Refresh();

                // Assert
                Assert.IsTrue(folder.LastAccessTime >= beforeDelete);
                Assert.IsTrue(folder.LastWriteTime >= beforeDelete);
                Assert.IsTrue(folder.CreationTime == old);
            }
        }

        [TestMethod]
        public void Timestamps_DeleteFile_GrandparentDirectoryIsntUpdated()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\parent\folder\file.txt");

                var parent = driver.GetDirectory(@"c:\parent");

                var beforeDelete = DateTime.Now;
                var old = beforeDelete.AddMinutes(-5);
                parent.LastAccessTime = parent.LastWriteTime = parent.CreationTime = old;

                Assert.IsTrue(parent.LastAccessTime == old);
                Assert.IsTrue(parent.LastWriteTime == old);
                Assert.IsTrue(parent.CreationTime == old);

                // act
                var file = driver.GetFile(@"c:\parent\folder\file.txt");
                file.Delete();

                parent.Refresh();

                // Assert
                Assert.IsTrue(parent.LastAccessTime == old);
                Assert.IsTrue(parent.LastWriteTime == old);
                Assert.IsTrue(parent.CreationTime == old);
            }
        }


        [TestMethod]
        public void Timestamps_DeleteDirectory_UpdateTimestamps()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var subdir = driver.CreateOrGetDirectory(@"c:\folder\subdir");

                var folder = driver.GetDirectory(@"c:\folder");

                var beforeDelete = DateTime.Now;
                var old = beforeDelete.AddMinutes(-5);
                folder.LastAccessTime = folder.LastWriteTime = folder.CreationTime = old;

                Assert.IsTrue(folder.LastAccessTime == old);
                Assert.IsTrue(folder.LastWriteTime == old);
                Assert.IsTrue(folder.CreationTime == old);

                // act
                subdir.Delete();

                folder.Refresh();

                // Assert
                Assert.IsTrue(folder.LastAccessTime >= beforeDelete);
                Assert.IsTrue(folder.LastWriteTime >= beforeDelete);
                Assert.IsTrue(folder.CreationTime == old);
            }
        }

        [TestMethod]
        public void Timestamp_DeleteDirectory_GrandparentDirectoryIsntUpdated()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var beforeCreate = DateTime.Now;

                var f = driver.CreateOrGetDirectory(@"c:\parent\subfolder\folder");
                var parent = driver.GetDirectory(@"c:\parent");

                var expected = beforeCreate.AddMinutes(-5);
                parent.LastAccessTime = parent.LastWriteTime = parent.CreationTime = expected;

                Assert.IsTrue(parent.LastAccessTime == expected);
                Assert.IsTrue(parent.LastWriteTime == expected);
                Assert.IsTrue(parent.CreationTime == expected);

                // act
                var folder = driver.GetDirectory(@"c:\parent\subfolder\folder");
                folder.Delete();

                // Assert
                Assert.IsTrue(parent.LastAccessTime == expected);
                Assert.IsTrue(parent.LastWriteTime == expected);
                Assert.IsTrue(parent.CreationTime == expected);
            }
        }

        [TestMethod]
        public void SetAttributes_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder = driver.GetDirectory(@"c:\folder");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => folder.Attributes = FileAttributes.Hidden);
            }
        }

        [TestMethod]
        public void SetAttributes_DirExists_CanSetAttributes()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder = driver.CreateOrGetFile(@"c:\folder");
                folder.Attributes = FileAttributes.Hidden;

                // assert
                Assert.AreEqual(FileAttributes.Hidden, folder.Attributes);
            }
        }
    }
}
