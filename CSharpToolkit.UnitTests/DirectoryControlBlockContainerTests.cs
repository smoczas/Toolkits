using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests
{
    [TestClass]
    public class DirectoryControlBlockContainerTests
    {
        [TestMethod]
        public void AddOrGet_NewDir_WholePathIsNew_ReturnsAllAddedItems()
        {
            // arrange
            var container = new DirectoryControlBlockContainer();
            var id = new DirectoryIdentifier(@"c:\temp\folder\subfolder");

            // act
            var added = container.AddOrGet(id, out var result);

            // assert
            Assert.IsTrue(added);
            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(new DirectoryIdentifier(@"c:\"), result[0].Idendifier);
            Assert.AreEqual(new DirectoryIdentifier(@"c:\temp"), result[1].Idendifier);
            Assert.AreEqual(new DirectoryIdentifier(@"c:\temp\folder"), result[2].Idendifier);
            Assert.AreEqual(new DirectoryIdentifier(@"c:\temp\folder\subfolder"), result[3].Idendifier);
        }

        [TestMethod]
        public void AddOrGet_NewDir_PartOfPathIsNew_ReturnsParentAndAddedItems()
        {
            // arrange
            var container = new DirectoryControlBlockContainer();
            container.AddOrGet(new DirectoryIdentifier(@"c:\temp\folder"), out _);

            // act
            var added = container.AddOrGet(new DirectoryIdentifier(@"c:\temp\folder\subfolder\deep_subfolder"), out var result);

            // assert
            Assert.IsTrue(added);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(new DirectoryIdentifier(@"c:\temp\folder"), result[0].Idendifier);
            Assert.AreEqual(new DirectoryIdentifier(@"c:\temp\folder\subfolder"), result[1].Idendifier);
            Assert.AreEqual(new DirectoryIdentifier(@"c:\temp\folder\subfolder\deep_subfolder"), result[2].Idendifier);
        }

        [TestMethod]
        public void AddOrGet_NewDir_CreatesSubdirs()
        {
            // arrange
            var container = new DirectoryControlBlockContainer();
            var id = new DirectoryIdentifier(@"c:\temp\folder\subfolder");

            // act
            container.AddOrGet(id, out _);

            // assert
            Assert.IsTrue(container.TryGet(new DirectoryIdentifier(@"c:\"), out var c));
            Assert.IsNotNull(c);
            Assert.AreEqual(@"c:\", c.Name);

            Assert.IsTrue(container.TryGet(new DirectoryIdentifier(@"c:\temp"), out var temp));
            Assert.IsNotNull(temp);
            Assert.AreEqual(@"c:\temp\", temp.Name);

            Assert.IsTrue(container.TryGet(new DirectoryIdentifier(@"c:\temp\folder"), out var folder));
            Assert.IsNotNull(folder);
            Assert.AreEqual(@"c:\temp\folder\", folder.Name);

            Assert.IsTrue(container.TryGet(new DirectoryIdentifier(@"c:\temp\folder\subfolder"), out var subfolder));
            Assert.IsNotNull(subfolder);
            Assert.AreEqual(@"c:\temp\folder\subfolder\", subfolder.Name);
        }

        [TestMethod]
        public void AddOrGet_ExistingDir_ReturnsOnlyExistingItem()
        {
            // arrange
            var container = new DirectoryControlBlockContainer();
            container.AddOrGet(new DirectoryIdentifier(@"c:\temp"), out _);

            // act
            var added = container.AddOrGet(new DirectoryIdentifier(@"c:\temp"), out var result);

            // assert
            Assert.IsFalse(added);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(new DirectoryIdentifier(@"c:\temp"), result[0].Idendifier);
        }

        [TestMethod]
        public void AddOrGet_ExistingDir_DoesntCreateNew()
        {
            // arrange
            var container = new DirectoryControlBlockContainer();

            // act
            container.AddOrGet(new DirectoryIdentifier(@"c:\temp"), out var expected);
            container.AddOrGet(new DirectoryIdentifier(@"c:\temp"), out var result);

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Last(), result.First());
        }

        [TestMethod]
        public void TryGet_NonExistingDir_ReturnsFalse()
        {
            // arrange
            var id = new DirectoryIdentifier(@"c:\temp\folder\subfolder");
            var container = new DirectoryControlBlockContainer();
            container.AddOrGet(id, out _);


            // assert
            Assert.IsFalse(container.TryGet(new DirectoryIdentifier(@"c:\othertemp"), out var _));
            Assert.IsFalse(container.TryGet(new DirectoryIdentifier(@"c:\temp\subtemp"), out _));
        }
    }
}
