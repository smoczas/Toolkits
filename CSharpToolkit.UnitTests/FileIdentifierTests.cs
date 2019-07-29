using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests
{
    [TestClass]
    public class FileIdentifierTests
    {
        [DataTestMethod]
        [DataRow(@"file.txt", @"file.txt")]
        [DataRow(@"file.txt", @"FILE.TXT")]
        [DataRow(@"c:\file.txt", @"c:\file.txt")]
        [DataRow(@"c:\file.txt", @"C:\FILE.TXT")]
        [DataRow(@"c:\temp\file.txt", @"c:\temp\file.txt")]
        public void SamePaths_AreEqual(string lhsVal, string rhsVal)
        {
            var lhs = FileIdentifier.FromPath(lhsVal);
            var rhs = FileIdentifier.FromPath(rhsVal);

            Assert.AreEqual(lhs, rhs);
            Assert.AreEqual(lhs.GetHashCode(), rhs.GetHashCode());
        }

        [DataTestMethod]
        [DataRow(@"file.txt", @"file2.txt")]
        [DataRow(@"c:\file.txt", @"d:\file.txt")]
        [DataRow(@"c:\temp\file.txt", @"c:\file.txt")]
        [DataRow(@"c:\temp\file1.txt", @"c:\temp\file2.txt")]
        public void DifferentPaths_ArentEqual(string lhsVal, string rhsVal)
        {
            var lhs = FileIdentifier.FromPath(lhsVal);
            var rhs = FileIdentifier.FromPath(rhsVal);

            Assert.AreNotEqual(lhs, rhs);
            Assert.AreNotEqual(lhs.GetHashCode(), rhs.GetHashCode());
        }
    }
}
