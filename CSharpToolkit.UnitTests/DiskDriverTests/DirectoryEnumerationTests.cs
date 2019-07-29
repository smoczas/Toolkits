using System;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class DirectoryEnumerationTests
    {
        [TestMethod]
        public void GetDirectories_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var root = driver.GetDirectory(@"c:\root");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => root.GetDirectories(@"subfolder", SearchOption.AllDirectories));
            }
        }

        [TestMethod]
        public void GetDirectories_EmptyDirectory_ReturnsEmpy()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var root = driver.CreateOrGetDirectory(@"c:\root");

                // act
                var result = root.GetDirectories(@"*", SearchOption.AllDirectories);

                // assert
                Assert.IsFalse(result.Any());
            }
        }

        [TestMethod]
        public void GetDirectories_Recurse_NoWilchars_CanFind()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_1");
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_2");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\subfolder_3");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\subfolder_4");

                var root = driver.GetDirectory(@"c:\root");

                // act
                var result = root.GetDirectories(@"subfolder_4", SearchOption.AllDirectories);

                // assert
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual(@"c:\root\folder_2\subfolder_4", result[0].FullName);
            }
        }

        [TestMethod]
        public void GetDirectories_Recurse_Wilchars_CanFind()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_1");
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_2");
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder1"); // to check if they are excluded from result
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder1"); // to check if they are excluded from result


                driver.CreateOrGetDirectory(@"c:\root\folder_2\subfolder_3");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\subfolder_4");
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder3"); // to check if they are excluded from result
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder4"); // to check if they are excluded from result

                var root = driver.GetDirectory(@"c:\root");

                // act
                var result = root.GetDirectories(@"subfolder_*", SearchOption.AllDirectories);

                // assert
                Assert.AreEqual(4, result.Length);
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_1\subfolder_1").Count());
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_1\subfolder_2").Count());
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_2\subfolder_3").Count());
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_2\subfolder_4").Count());
            }
        }

        [TestMethod]
        public void GetDirectories_TopDirOnly_NoWildchars_CanFind()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\root\folder_1\folder_1");
                driver.CreateOrGetDirectory(@"c:\root\folder_1\folder_2");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\folder_1");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\folder_2");

                var root = driver.GetDirectory(@"c:\root");

                // act
                var result = root.GetDirectories(@"folder_2", SearchOption.TopDirectoryOnly);

                // assert
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual(@"c:\root\folder_2", result[0].FullName);
            }
        }

        [TestMethod]
        public void GetDirectories_ExplicitSearchPath_CanFind()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_1");
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_2");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\subfolder_3");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\subfolder_4");

                var root = driver.GetDirectory(@"c:\root");

                // act
                var result = root.GetDirectories(@"folder_1\subfolder_*", SearchOption.AllDirectories);

                // assert
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_1\subfolder_1").Count());
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_1\subfolder_2").Count());
            }
        }

        [TestMethod]
        public void GetDirectories_ExplicitSearchPath_IllegalWildchar_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_1");
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_2");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\subfolder_3");
                driver.CreateOrGetDirectory(@"c:\root\folder_2\subfolder_4");

                var root = driver.GetDirectory(@"c:\root");

                // act/assert
                Assert.ThrowsException<ArgumentException>(() => root.GetDirectories(@"folder_*\subfolder_*", SearchOption.AllDirectories));
            }
        }

        [TestMethod]
        public void GetDirectories_ExplicitSearchPath_PartOfPathNotExists_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\root\folder_1\subfolder_1");

                var root = driver.GetDirectory(@"c:\root");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => root.GetDirectories(@"folder_2\subfolder_*", SearchOption.AllDirectories));
            }
        }

        [TestMethod]
        public void GetFiles_DirectoryDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var root = driver.GetDirectory(@"c:\root");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => root.GetFiles(@"file.txt", SearchOption.AllDirectories));
            }
        }

        [TestMethod]
        public void GetFiles_EmptyDirectory_ReturnsEmpty()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var root = driver.CreateOrGetDirectory(@"c:\root");

                // act
                var result = root.GetFiles(@"*", SearchOption.AllDirectories);

                // assert
                Assert.IsFalse(result.Any());
            }
        }

        [TestMethod]
        public void GetFiles_Recurse_NoWilchars_CanFind()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\root\folder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\root\folder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\root\folder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\root\folder_2\file_2.txt");

                var root = driver.GetDirectory(@"c:\root");

                // act
                var result = root.GetFiles(@"file_1.txt", SearchOption.AllDirectories);

                // assert
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_1\file_1.txt").Count());
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_2\file_1.txt").Count());
            }
        }

        [TestMethod]
        public void GetFiles_Recurse_Wilchars_CanFind()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\root\folder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\root\folder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\root\folder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\root\folder_2\file_2.txt");

                var root = driver.GetDirectory(@"c:\root");

                // act
                var result = root.GetFiles(@"*_1.txt", SearchOption.AllDirectories);

                // assert
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_1\file_1.txt").Count());
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_2\file_1.txt").Count());
            }
        }

        [TestMethod]
        public void GetFiles_TopDirOnly_NoWildchars_CanFind()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\root\folder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\root\file_1.txt");

                var root = driver.GetDirectory(@"c:\root");

                // act
                var result = root.GetFiles(@"file_1.txt", SearchOption.TopDirectoryOnly);

                // assert
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual(@"c:\root\file_1.txt", result[0].FullName);
            }
        }

        [TestMethod]
        public void GetFiles_ExplicitSearchPath_CanFind()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\root\folder_1\file_1.txt");
                driver.CreateOrGetFile(@"c:\root\folder_1\file_2.txt");
                driver.CreateOrGetFile(@"c:\root\folder_2\file_1.txt");
                driver.CreateOrGetFile(@"c:\root\folder_2\file_2.txt");

                var root = driver.GetDirectory(@"c:\root");

                // act
                var result = root.GetFiles(@"folder_1\file_*.txt", SearchOption.AllDirectories);

                // assert
                Assert.AreEqual(2, result.Length);
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_1\file_1.txt").Count());
                Assert.AreEqual(1, result.Where(i => i.FullName == @"c:\root\folder_1\file_2.txt").Count());
            }
        }

        [TestMethod]
        public void GetFiles_ExplicitSearchPath_IllegalWildchar_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\root\folder_1\subfolder_1");
                driver.CreateOrGetFile(@"c:\root\folder_1\subfolder_2");
                driver.CreateOrGetFile(@"c:\root\folder_2\subfolder_3");
                driver.CreateOrGetFile(@"c:\root\folder_2\subfolder_4");

                var root = driver.GetDirectory(@"c:\root");

                // act/assert
                Assert.ThrowsException<ArgumentException>(() => root.GetFiles(@"folder_*\file_*.txt", SearchOption.AllDirectories));
            }
        }

        [TestMethod]
        public void GetFiles_ExplicitSearchPath_PartOfPathNotExists_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\root\folder_1\file_1.txt");

                var root = driver.GetDirectory(@"c:\root");

                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() => root.GetFiles(@"folder_2\subfolder_*", SearchOption.AllDirectories));
            }
        }
    }
}
