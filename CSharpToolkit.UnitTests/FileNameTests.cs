using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.IO;

namespace CSharpToolkit.UnitTests
{
    [TestClass]
    public class FileNameTests
    {
        [DataTestMethod]
        [DataRow(@"file.txt","file.txt")]
        [DataRow(@"file.txt", "FILE.TXT")]
        [DataRow(@"file.txt", @"c:\temp\file.txt")]
        public void SameFileName_AreEqual(string val1, string val2)
        {
            var lhs = new FileName(val1);
            var rhs = new FileName(val2);

            Assert.AreEqual(lhs, rhs);
            Assert.AreEqual(lhs.GetHashCode(), rhs.GetHashCode());
        }

        [DataTestMethod]
        [DataRow(@"file.txt", "other_file.txt")]
        [DataRow(@"file.txt", @"c:\temp\other_file.txt")]
        [DataRow(@"file.txt", @"c:\temp\")]
        public void DifferentFileName_ArentEqual(string val1, string val2)
        {
            var lhs = new FileName(val1);
            var rhs = new FileName(val2);

            Assert.AreNotEqual(lhs, rhs);
            Assert.AreNotEqual(lhs.GetHashCode(), rhs.GetHashCode());
        }
    }
}
