using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;
using NSubstitute;

namespace CSharpToolkit.UnitTests
{
    [TestClass]
    public class MemoryStreamTests
    {
        [TestMethod]
        public void Dispose_CanWriteToSink()
        {
            // arrange
            using (var sink = new MemoryStream())
            using (var reader = new StreamReader(sink))
            {
                using (var msStub = new MemoryStreamProxy(sink, FileIdentifier.FromPath(@"c:\somefile.txt"), Substitute.For<IDiskDriver>()))
                using (var writer = new StreamWriter(msStub))
                {
                    writer.WriteLine("TEST");
                }

                // assert
                var result = reader.ReadLine();
                Assert.AreEqual("TEST", result);
            }
        }

        [TestMethod]
        public void Dispose_CanUpdateTimes()
        {
            // arrange
            var driver = Substitute.For<IDiskDriver>();
            var id = FileIdentifier.FromPath(@"c:\somefile.txt");
            using (var sink = new MemoryStream())
            {
                using (var msStub = new MemoryStreamProxy(sink, id, driver))
                using (var writer = new StreamWriter(msStub))
                {
                    writer.WriteLine("TEST");
                }
                
                // assert
                driver.Received().SetLastAccessTime(id, Arg.Any<DateTime>());
                driver.Received().SetLastWriteTime(id, Arg.Any<DateTime>());
            }
        }
    }
}
