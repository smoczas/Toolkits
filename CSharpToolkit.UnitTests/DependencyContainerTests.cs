using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests
{
    [TestClass]
    public class DependencyContainerTests
    {
        [TestMethod]
        public void Register_CanGetRegisteredDependency()
        {
            // Arrange
            var dc = new DependencyContainer();

            // Act
            var registered = "comparable string";
            dc.For<IComparable>().Register(registered);
            var result = dc.For<IComparable>().Get(null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(registered, result);
        }
    }
}
