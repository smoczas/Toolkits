using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class DirectoryMoveTests
    {
        [TestMethod]
        public void Move_TargetDirAlreadyExists_Throws()
        {
            // arrange
            using(var driver = new DiskDriver())
            {
                var from = driver.CreateOrGetDirectory(@"c:\dir1");
                driver.CreateOrGetDirectory(@"c:\dir2");

                // act/assert
                Assert.ThrowsException<IOException>(() => from.MoveTo(@"c:\dir2"));
            }
        }

        [TestMethod]
        public void Move_FileWithTheSameNameAlreadyExists_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var from = driver.CreateOrGetDirectory(@"c:\dir1");
                driver.CreateOrGetFile(@"c:\dir2");

                // act/assert
                Assert.ThrowsException<IOException>(() => from.MoveTo(@"c:\dir2"));
            }
        }

        [TestMethod]
        public void Move_SourceDoesntExists_Throws()
        {
            // arrange
            using(var driver = new DiskDriver())
            {
                var dir = driver.GetDirectory(@"c:\dir");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => dir.MoveTo(@"c:\temp\dir"));
            }
        }

        [TestMethod]
        public void Move_PartOfTargetDoesntExists_Throws()
        {
            // arrange
            using(var driver = new DiskDriver())
            {
                var dir = driver.CreateOrGetDirectory(@"c:\dir");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => dir.MoveTo(@"c:\temp\dir"));
            }
        }

        [TestMethod]
        public void Move_CanMoveOwnership()
        {
            // arrange
            using(var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_2\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_2\file_2.txt");

                driver.CreateOrGetDirectory(@"c:\targ");

                var folder_1 = driver.GetDirectory(@"c:\src\folder_1");

                // act
                folder_1.MoveTo(@"c:\targ\folder_1");

                // assert
                var c = driver.GetDirectory(@"c:\");
                var resultFile = c.GetFiles("*", SearchOption.AllDirectories);
                Assert.AreEqual(8, resultFile.Length);
                Assert.AreEqual(1, resultFile.Where(i => i.FullName.Equals(@"c:\targ\folder_1\subfolder_1\file_1.txt")).Count());
                Assert.AreEqual(1, resultFile.Where(i => i.FullName.Equals(@"c:\targ\folder_1\subfolder_1\file_2.txt")).Count());
                Assert.AreEqual(1, resultFile.Where(i => i.FullName.Equals(@"c:\targ\folder_1\subfolder_2\file_1.txt")).Count());
                Assert.AreEqual(1, resultFile.Where(i => i.FullName.Equals(@"c:\targ\folder_1\subfolder_2\file_2.txt")).Count());
                Assert.AreEqual(1, resultFile.Where(i => i.FullName.Equals(@"c:\src\folder_2\subfolder_1\file_1.txt")).Count());
                Assert.AreEqual(1, resultFile.Where(i => i.FullName.Equals(@"c:\src\folder_2\subfolder_1\file_2.txt")).Count());
                Assert.AreEqual(1, resultFile.Where(i => i.FullName.Equals(@"c:\src\folder_2\subfolder_2\file_1.txt")).Count());
                Assert.AreEqual(1, resultFile.Where(i => i.FullName.Equals(@"c:\src\folder_2\subfolder_2\file_2.txt")).Count());

                var resultDir = c.GetDirectories("*", SearchOption.AllDirectories);
                Assert.AreEqual(8, resultDir.Length);
                Assert.AreEqual(1, resultDir.Where(i => i.FullName.Equals(@"c:\targ")).Count());
                Assert.AreEqual(1, resultDir.Where(i => i.FullName.Equals(@"c:\targ\folder_1")).Count());
                Assert.AreEqual(1, resultDir.Where(i => i.FullName.Equals(@"c:\targ\folder_1\subfolder_1")).Count());
                Assert.AreEqual(1, resultDir.Where(i => i.FullName.Equals(@"c:\targ\folder_1\subfolder_2")).Count());
                Assert.AreEqual(1, resultDir.Where(i => i.FullName.Equals(@"c:\src")).Count());
                Assert.AreEqual(1, resultDir.Where(i => i.FullName.Equals(@"c:\src\folder_2")).Count());
                Assert.AreEqual(1, resultDir.Where(i => i.FullName.Equals(@"c:\src\folder_2\subfolder_1")).Count());
                Assert.AreEqual(1, resultDir.Where(i => i.FullName.Equals(@"c:\src\folder_2\subfolder_2")).Count());
            }
        }

        [TestMethod]
        public void Move_CanUpdateStubs()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_2\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_2\file_2.txt");

                driver.CreateOrGetDirectory(@"c:\targ");

                var folder_1 = driver.GetDirectory(@"c:\src\folder_1");

                // act
                folder_1.MoveTo(@"c:\targ\folder_1");

                // assert
                var fileStub = driver.GetFile(@"c:\targ\folder_1\subfolder_1\file_1.txt");
                Assert.AreEqual(@"c:\targ\folder_1\subfolder_1\file_1.txt", fileStub.FullName);
                fileStub = driver.GetFile(@"c:\targ\folder_1\subfolder_1\file_2.txt");
                Assert.AreEqual(@"c:\targ\folder_1\subfolder_1\file_2.txt", fileStub.FullName);
                fileStub = driver.GetFile(@"c:\targ\folder_1\subfolder_2\file_1.txt");
                Assert.AreEqual(@"c:\targ\folder_1\subfolder_2\file_1.txt", fileStub.FullName);
                fileStub = driver.GetFile(@"c:\targ\folder_1\subfolder_2\file_2.txt");
                Assert.AreEqual(@"c:\targ\folder_1\subfolder_2\file_2.txt", fileStub.FullName);

                fileStub = driver.GetFile(@"c:\src\folder_2\subfolder_1\file_1.txt");
                Assert.AreEqual(@"c:\src\folder_2\subfolder_1\file_1.txt", fileStub.FullName);
                fileStub = driver.GetFile(@"c:\src\folder_2\subfolder_1\file_2.txt");
                Assert.AreEqual(@"c:\src\folder_2\subfolder_1\file_2.txt", fileStub.FullName);
                fileStub = driver.GetFile(@"c:\src\folder_2\subfolder_2\file_1.txt");
                Assert.AreEqual(@"c:\src\folder_2\subfolder_2\file_1.txt", fileStub.FullName);
                fileStub = driver.GetFile(@"c:\src\folder_2\subfolder_2\file_2.txt");
                Assert.AreEqual(@"c:\src\folder_2\subfolder_2\file_2.txt", fileStub.FullName);

                var dirStub = driver.GetDirectory(@"c:\targ");
                Assert.AreEqual(@"c:\targ", dirStub.FullName);
                dirStub = driver.GetDirectory(@"c:\targ\folder_1");
                Assert.AreEqual(@"c:\targ\folder_1", dirStub.FullName);
                dirStub = driver.GetDirectory(@"c:\targ\folder_1\subfolder_1");
                Assert.AreEqual(@"c:\targ\folder_1\subfolder_1", dirStub.FullName);
                dirStub = driver.GetDirectory(@"c:\targ\folder_1\subfolder_2");
                Assert.AreEqual(@"c:\targ\folder_1\subfolder_2", dirStub.FullName);

                dirStub = driver.GetDirectory(@"c:\src");
                Assert.AreEqual(@"c:\src", dirStub.FullName);
                dirStub = driver.GetDirectory(@"c:\src\folder_2");
                Assert.AreEqual(@"c:\src\folder_2", dirStub.FullName);
                dirStub = driver.GetDirectory(@"c:\src\folder_2\subfolder_1");
                Assert.AreEqual(@"c:\src\folder_2\subfolder_1", dirStub.FullName);
                dirStub = driver.GetDirectory(@"c:\src\folder_2\subfolder_2");
                Assert.AreEqual(@"c:\src\folder_2\subfolder_2", dirStub.FullName);
            }
        }

        [TestMethod]
        public void Move_CanUpdateControlBlockCollections()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_1\subfolder_2\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\src\folder_2\subfolder_2\file_2.txt");

                driver.CreateOrGetDirectory(@"c:\targ");

                var folder_1 = driver.GetDirectory(@"c:\src\folder_1");

                // act
                folder_1.MoveTo(@"c:\targ\folder_1");

                // assert
                var fileStub = driver.GetFile(@"c:\targ\folder_1\subfolder_1\file_1.txt");
                Assert.IsTrue(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\targ\folder_1\subfolder_1\file_2.txt");
                Assert.IsTrue(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\targ\folder_1\subfolder_2\file_1.txt");
                Assert.IsTrue(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\targ\folder_1\subfolder_2\file_2.txt");
                Assert.IsTrue(fileStub.Exists);

                fileStub = driver.GetFile(@"c:\src\folder_1\subfolder_1\file_1.txt");
                Assert.IsFalse(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\src\folder_1\subfolder_1\file_2.txt");
                Assert.IsFalse(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\src\folder_1\subfolder_2\file_1.txt");
                Assert.IsFalse(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\src\folder_1\subfolder_2\file_2.txt");
                Assert.IsFalse(fileStub.Exists);

                fileStub = driver.GetFile(@"c:\src\folder_2\subfolder_1\file_1.txt");
                Assert.IsTrue(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\src\folder_2\subfolder_1\file_2.txt");
                Assert.IsTrue(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\src\folder_2\subfolder_2\file_1.txt");
                Assert.IsTrue(fileStub.Exists);
                fileStub = driver.GetFile(@"c:\src\folder_2\subfolder_2\file_2.txt");
                Assert.IsTrue(fileStub.Exists);

                var dirStub = driver.GetDirectory(@"c:\targ");
                Assert.IsTrue(dirStub.Exists);
                dirStub = driver.GetDirectory(@"c:\targ\folder_1");
                Assert.IsTrue(dirStub.Exists);
                dirStub = driver.GetDirectory(@"c:\targ\folder_1\subfolder_1");
                Assert.IsTrue(dirStub.Exists);
                dirStub = driver.GetDirectory(@"c:\targ\folder_1\subfolder_2");
                Assert.IsTrue(dirStub.Exists);

                dirStub = driver.GetDirectory(@"c:\src\folder_1");
                Assert.IsFalse(dirStub.Exists);
                dirStub = driver.GetDirectory(@"c:\src\folder_1\subfolder_1");
                Assert.IsFalse(dirStub.Exists);
                dirStub = driver.GetDirectory(@"c:\src\folder_1\subfolder_2");
                Assert.IsFalse(dirStub.Exists);

                dirStub = driver.GetDirectory(@"c:\src");
                Assert.IsTrue(dirStub.Exists);
                dirStub = driver.GetDirectory(@"c:\src\folder_2");
                Assert.IsTrue(dirStub.Exists);
                dirStub = driver.GetDirectory(@"c:\src\folder_2\subfolder_1");
                Assert.IsTrue(dirStub.Exists);
                dirStub = driver.GetDirectory(@"c:\src\folder_2\subfolder_2");
                Assert.IsTrue(dirStub.Exists);
            }
        }
    }
}
