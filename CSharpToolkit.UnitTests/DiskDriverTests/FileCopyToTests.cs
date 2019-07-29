using System;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class FileCopyToTests
    {
        [TestMethod]
        public void CopyTo_TargetIsExistingDir_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");
                driver.CreateOrGetDirectory(@"c:\targ");

                // act/assert
                Assert.ThrowsException<IOException>(() => src.CopyTo(@"c:\targ", false));
            }
        }

        [TestMethod]
        public void CopyTo_TargetIsNewFile_CanCopy()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");
                driver.CreateOrGetDirectory(@"c:\targ");
                driver.GetFile(@"c:\targ\new_file.txt");
                using (var t = src.CreateText())
                {
                    t.Write("CONTENT");
                }

                // act
                var result = src.CopyTo(@"c:\targ\new_file.txt", false);

                // assert
                using (var t = result.OpenText())
                {
                    Assert.AreEqual("CONTENT", t.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void CopyTo_TargetIsExistingFile_NoOverwrite_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");
                var targ = driver.CreateOrGetFile(@"c:\targ\existing_file.txt");

                // act/assert
                Assert.ThrowsException<IOException>(() => src.CopyTo(@"c:\targ\existing_file.txt", false));
            }
        }

        [TestMethod]
        public void CopyTo_TargetIsExistingFile_Overwrite_CanCopy()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");
                var targ = driver.CreateOrGetFile(@"c:\targ\existing_file.txt");
                using (var t = src.CreateText())
                {
                    t.Write("CONTENT");
                }
                using (var t = targ.CreateText())
                {
                    t.Write("OLD_CONTENT");
                }

                // act
                var result = src.CopyTo(@"c:\targ\existing_file.txt", true);

                // assert
                using (var t = result.OpenText())
                {
                    Assert.AreEqual("CONTENT", t.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void CopyTo_TargetDirDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => src.CopyTo(@"c:\targ\file.txt", true));
            }
        }

        [TestMethod]
        public void CopyTo_SourceDirDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var src = driver.GetFile(@"c:\src\file.txt");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => src.CopyTo(@"c:\targ\file.txt", true));
            }
        }

        [TestMethod]
        public void CopyTo_SourceFileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\src");
                var src = driver.GetFile(@"c:\src\file.txt");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() => src.CopyTo(@"c:\targ\file.txt", true));
            }
        }

        [TestMethod]
        public void CopyTo_CanUpdateTimes()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\targ");
                var src = driver.CreateOrGetFile(@"c:\src\file.txt");

                var time = DateTime.Now.AddMinutes(-5);
                src.LastAccessTime =
                    src.LastWriteTime =
                    src.CreationTime =
                    time;


                // act
                var result = src.CopyTo(@"c:\targ\file.txt", true);

                // assert
                Assert.IsTrue(result.CreationTime > time);
                Assert.IsTrue(result.LastWriteTime > time);
                Assert.IsTrue(result.LastAccessTime > time);
            }
        }

        [TestMethod]
        public void CopyTo_CanCreateNewOnwership()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var srcDir = driver.CreateOrGetDirectory(@"c:\src");
                var srcFile = driver.CreateOrGetFile(@"c:\src\file.txt");
                driver.CreateOrGetFile(@"c:\src\file_2.txt");

                var targetDir = driver.CreateOrGetDirectory(@"c:\targ");

                // act
                srcFile.CopyTo(@"c:\targ\file.txt", true);

                // assert
                var targetFiles = targetDir.GetFiles();
                var srcFiles = srcDir.GetFiles();
                Assert.AreEqual(2, srcFiles.Length);
                Assert.AreEqual(1, srcFiles.Where(i => i.FullName == @"c:\src\file.txt").Count());
                Assert.AreEqual(1, srcFiles.Where(i => i.FullName == @"c:\src\file_2.txt").Count());

                Assert.AreEqual(1, targetFiles.Length);
                Assert.AreEqual(@"c:\targ\file.txt", targetFiles[0].FullName);
            }
        }
    }
}
