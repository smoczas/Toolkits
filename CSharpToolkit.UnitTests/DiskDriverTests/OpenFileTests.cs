using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class OpenFileTests
    {
        [DataTestMethod]
        [DataRow(FileMode.Append)]
        [DataRow(FileMode.Create)]
        [DataRow(FileMode.CreateNew)]
        [DataRow(FileMode.Open)]
        [DataRow(FileMode.OpenOrCreate)]
        [DataRow(FileMode.Truncate)]
        public void OpenFile_PathDoesntExist_Throws(FileMode mode)
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                // act/assert
                Assert.ThrowsException<DirectoryNotFoundException>(() =>
                {
                    driver.OpenFile(FileIdentifier.FromPath(@"c:\unexisting\file.txt"),
                        mode,
                        FileAccess.Write,
                        FileShare.Write);
                });
            }
        }

        [DataTestMethod]
        [DataRow(FileMode.Append, FileAccess.ReadWrite, FileShare.Read)]
        [DataRow(FileMode.Append, FileAccess.Read, FileShare.Read)]
        public void OpenFile_ConflictingArguments_Throws(FileMode mode, FileAccess access, FileShare share)
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                // act/assert
                var e = Assert.ThrowsException<ArgumentException>(() =>
                {
                    driver.OpenFile(FileIdentifier.FromPath(@"c:\unexisting\file.txt"),
                        mode,
                        access,
                        share);
                });
            }
        }

        [TestMethod]
        public void OpenFile_CreateMode_FileDoesntExist_CanCreateFile()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                // act
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");
                using (var writer = new StreamWriter(file.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    writer.Write("1");
                }

                // assert
                using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    Assert.AreEqual("1", reader.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void OpenFile_CreateMode_FileExists_CanOverwriteFile()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");
                using (var writer = new StreamWriter(file.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    writer.Write("1");
                }

                // act
                using (var writer = new StreamWriter(file.Open(FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    writer.Write("2");
                }

                // assert
                using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    Assert.AreEqual("2", reader.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void OpenFile_CreateNewMode_FileDoesntExist_CanCreateFile()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                // act
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");
                using (var writer = new StreamWriter(file.Open(FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    writer.Write("1");
                }

                // assert
                using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    Assert.AreEqual("1", reader.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void OpenFile_CreateNewMode_FileExists_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetFile(@"c:\temp\file.txt");

                // act/assert
                Assert.ThrowsException<IOException>(() =>
                {
                    driver.OpenFile(FileIdentifier.FromPath(@"c:\temp\file.txt"),
                        FileMode.CreateNew,
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);
                });
            }
        }

        [TestMethod]
        public void OpenFile_OpenMode_FileDoesntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");

                // act/assert
                Assert.ThrowsException<FileNotFoundException>(() =>
                {
                    driver.OpenFile(FileIdentifier.FromPath(@"c:\temp\file.txt"),
                        FileMode.Open,
                        FileAccess.ReadWrite,
                        FileShare.ReadWrite);
                });
            }
        }

        [TestMethod]
        public void OpenFile_OpenOrCreateMode_FileDoesntExist_CanCreateNewFile()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                // act
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");
                using (var writer = new StreamWriter(file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    writer.Write("1");
                }

                // assert
                using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
                {
                    Assert.AreEqual("1", reader.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void OpenFile_OpenOrCreateMode_FileEixsts_CanOpenFile()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                // act
                var file = driver.CreateOrGetFile(@"c:\temp\file.txt");
                using (var writer = new StreamWriter(file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    writer.Write("1");
                }

                // assert
                using (var reader = new StreamReader(file.Open(FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite)))
                {
                    Assert.AreEqual("1", reader.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void OpenFile_TruncateMode_FileExists_OverwritesFile()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                var file = driver.CreateOrGetFile(@"c:\temp\file.txt");
                using (var writer = new StreamWriter(file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    writer.Write("1");
                }

                // act
                using (var writer = new StreamWriter(file.Open(FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite)))
                {
                    writer.Write("2");
                }

                using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    Assert.AreEqual("2", reader.ReadToEnd());
                }
            }
        }

        [TestMethod]
        public void OpenFile_TruncateMode_FileDeosntExist_Throws()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act
                Assert.ThrowsException<FileNotFoundException>(() =>
                {
                    file.Open(FileMode.Truncate, FileAccess.Write, FileShare.ReadWrite);
                });
            }
        }

        [TestMethod]
        public void OpenFile_TruncateMode_ReadAcces_ThrowsOnReadAttempt()
        {
            Assert.IsTrue(false);
        }

        [TestMethod]
        public void OpenFile_AppendMode_FileDoesntExist_CanAppend()
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\temp");
                var file = driver.GetFile(@"c:\temp\file.txt");

                // act
                using (var writer = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                {
                    writer.Write("1");
                }
                using (var writer = new StreamWriter(file.Open(FileMode.Append, FileAccess.Write, FileShare.ReadWrite)))
                {
                    writer.Write("2");
                }

                // assert
                using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite)))
                {
                    Assert.AreEqual("12", reader.ReadToEnd());
                }
            }
        }

        [DataTestMethod]
        [DataRow(FileMode.Create)]
        [DataRow(FileMode.CreateNew)]
        [DataRow(FileMode.Open)]
        [DataRow(FileMode.OpenOrCreate)]
        public void OpenFile_DirectoryWithTheSameNameAlreadyExists_Throws(FileMode mode)
        {
            // arrange
            using (var driver = new DiskDriver())
            {
                driver.CreateOrGetDirectory(@"c:\file");
                var f = driver.GetFile(@"c:\file");

                // act/assert
                Assert.ThrowsException<UnauthorizedAccessException>(() => f.Open(mode));
            }
        }
    }
}
