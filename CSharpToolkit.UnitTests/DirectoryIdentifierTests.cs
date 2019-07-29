using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests
{
    [TestClass]
    public class DirectoryIdentifierTests
    {
        [DataTestMethod]
        [DataRow(@"c:\",@"C:\")]
        [DataRow(@"c:\temp", @"c:\temp")]
        [DataRow(@"c:\temp", @"c:\temp\")]
        [DataRow(@"c:\temp", @"C:\TEMP")]
        public void SamePaths_AreEqual(string lhsVal, string rhsVal)
        {
            var lhs = new DirectoryIdentifier(lhsVal);
            var rhs = new DirectoryIdentifier(rhsVal);

            Assert.AreEqual(lhs, rhs);
            Assert.AreEqual(lhs.GetHashCode(), rhs.GetHashCode());
        }

        [DataTestMethod]
        [DataRow(@"c:\", @"d:\")]
        [DataRow(@"c:\temp1", @"C:\temp2")]
        [DataRow(@"c:\temp\1", @"C:\temp\2")]
        public void DifferentPaths_ArentEqual(string lhsVal, string rhsVal)
        {
            var lhs = new DirectoryIdentifier(lhsVal);
            var rhs = new DirectoryIdentifier(rhsVal);

            Assert.AreNotEqual(lhs, rhs);
            Assert.AreNotEqual(lhs.GetHashCode(), rhs.GetHashCode());
        }
    }
}
