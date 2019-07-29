using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class DirectoryDelete
    {
        [TestMethod]
        public void Delete_ForDirectory_Flatten_CanRemoveEmptyDir()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder_1 = driver.CreateOrGetDirectory(@"c:\folder_1");
                driver.CreateOrGetDirectory(@"c:\folder_2");

                // act
                folder_1.Delete(false);

                // assert
                folder_1.Refresh();
                Assert.IsFalse(folder_1.Exists);

                var root = driver.GetDirectory(@"c:\");
                var folders = root.GetDirectories();
                Assert.AreEqual(1, folders.Length);
                Assert.AreEqual(@"c:\folder_2", folders[0].FullName);
            }
        }

        [TestMethod]
        public void Delete_ForDirectory_Flatten_DirectoryNotEmpty_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder_1 = driver.CreateOrGetDirectory(@"c:\folder_1");
                driver.CreateOrGetFile(@"c:\folder_1\file.txt");

                // act
                Assert.ThrowsException<IOException>(() => folder_1.Delete(false));
            }
        }

        [TestMethod]
        public void Delete_ForDirectory_RemoveRoot_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var c = driver.CreateOrGetDirectory(@"c:\");

                // act/assert
                Assert.ThrowsException<InvalidOperationException>(() => c.Delete(true));
            }
        }

        [TestMethod]
        public void Delete_ForDirectory_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var folder = driver.GetDirectory(@"c:\folder");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => folder.Delete(true));
            }
        }

        [TestMethod]
        public void Delete_ForDirectory_Recurse_DirectoryNotEmpty_CanDelete()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\folder\subfolder\file.txt");
                driver.CreateOrGetFile(@"c:\folder\file.txt");
                driver.CreateOrGetFile(@"c:\other_folder\file.txt");

                // act
                var folder = driver.GetDirectory(@"c:\folder");
                folder.Delete(true);

                // assert
                folder.Refresh();
                Assert.IsFalse(folder.Exists);
                Assert.IsFalse(driver.GetDirectory(@"c:\folder\subfolder").Exists);
                Assert.IsFalse(driver.GetFile(@"c:\folder\subfolder\file.txt").Exists);

                var c = driver.GetDirectory(@"c:\");
                var filesAfterDelete = c.GetFiles("*", SearchOption.AllDirectories);
                Assert.AreEqual(1, filesAfterDelete.Length);
                Assert.AreEqual(@"c:\other_folder\file.txt", filesAfterDelete[0].FullName);

                var directoriesAfterDelete = c.GetDirectories("*", SearchOption.AllDirectories);
                Assert.AreEqual(1, directoriesAfterDelete.Length);
                Assert.AreEqual(@"c:\other_folder", directoriesAfterDelete[0].FullName);
            }
        }
    }
}
