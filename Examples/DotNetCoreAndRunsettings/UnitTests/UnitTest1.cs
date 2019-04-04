using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    public abstract class TestBase
    {
        public TestContext TestContext { get; set; }
    }

    [TestClass]
    public class UnitTest1 : TestBase
    {
        [TestMethod]
        public void TestMethod1()
        {
            Console.WriteLine(TestContext.Properties["ConfigDirName"]);
        }
    }
}
