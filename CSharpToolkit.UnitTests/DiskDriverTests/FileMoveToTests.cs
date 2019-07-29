using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class FileMoveToTests
    {
        [TestMethod]
        public void MoveTo_TargetIsDirectory_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");
                driver.CreateOrGetDirectory(@"c:\targ");

                // act/assert
                Assert.ThrowsException<IOException>(() => src.MoveTo(@"c:\targ"));
            }
        }

        [TestMethod]
        public void MoveTo_TargetIsNewFile_CanMoveAndChangeName()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");
                driver.CreateOrGetDirectory(@"c:\targ");
                using (var t = src.CreateText())
                {
                    t.Write("CONTENT");
                }

                // act
                src.MoveTo(@"c:\targ\moved.txt");
                src.Refresh();

                // assert
                var target = driver.GetFile(@"c:\targ\moved.txt");
                Assert.AreEqual(@"c:\targ\moved.txt", src.FullName);
                Assert.IsFalse(driver.GetFile(@"c:\src\file.txt").Exists);
                Assert.IsTrue(target.Exists);
                using (var t = target.OpenText())
                {
                    Assert.AreEqual("CONTENT", t.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void MoveTo_SourceFileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\src");
                var src = driver.GetFile(@"c:\src\file.txt");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() => src.MoveTo(@"c:\targ"));
            }
        }

        [TestMethod]
        public void MoveTo_SourceDirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.GetFile(@"c:\src\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => src.MoveTo(@"c:\targ"));
            }
        }

        [TestMethod]
        public void MoveTo_TargetFileExists_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");
                driver.CreateOrGetFile(@"c:\targ\file.txt");

                // act/assert
                Assert.ThrowsException<IOException>(() => src.MoveTo(@"c:\targ"));
            }
        }

        [TestMethod]
        public void MoveTo_TargetDirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => src.MoveTo(@"c:\targ\file.txt"));
            }
        }

        [TestMethod]
        public void MoveTo_CanKeepTimes()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");
                driver.CreateOrGetDirectory(@"c:\targ");
                var time = DateTime.Now.AddMinutes(-5);

                src.LastAccessTime =
                    src.LastWriteTime =
                    src.CreationTime =
                    time;

                // act
                src.MoveTo(@"c:\targ\file.txt");

                // assert
                var result = driver.GetFile(@"c:\targ\file.txt");
                Assert.AreEqual(time, result.LastWriteTime);
                Assert.AreEqual(time, result.LastAccessTime);
                Assert.AreEqual(time, result.CreationTime);
            }
        }

        [TestMethod]
        public void MoveTo_CanMoveOwnership()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var srcDir = driver.CreateOrGetDirectory(@"c:\src");
                var srcFile = driver.CreateOrGetFile(@"c:\src\file.txt");
                driver.CreateOrGetFile(@"c:\src\file_2.txt");

                var targetDir = driver.CreateOrGetDirectory(@"c:\targ");

                // act
                srcFile.MoveTo(@"c:\targ\file.txt");

                // assert
                var targetFiles = targetDir.GetFiles();
                var srcFiles = srcDir.GetFiles();
                Assert.AreEqual(1, srcFiles.Length);
                Assert.AreEqual(@"c:\src\file_2.txt", srcFiles[0].FullName);

                Assert.AreEqual(1, targetFiles.Length);
                Assert.AreEqual(@"c:\targ\file.txt", targetFiles[0].FullName);
            }
        }
    }
}
