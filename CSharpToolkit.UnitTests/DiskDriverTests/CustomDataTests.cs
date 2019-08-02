using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSharpToolkit.Testing;

namespace CSharpToolkit.UnitTests.DiskDriverTests
{
    [TestClass]
    public class CustomDataTests
    {
        [TestMethod]
        public void GetCustomData_CanStoreCustomData()
        {
            // arrange
            using(var driver = new DiskDriver())
            {
                var file = driver.CreateOrGetFile(@"c:\file.txt");
                file.CustomData.Add("my-data-key", "my-data-value");

                // assert
                file = driver.GetFile(@"c:\file.txt");
                Assert.AreEqual("my-data-value", file.CustomData["my-data-key"]);
            }
        }
    }
}
