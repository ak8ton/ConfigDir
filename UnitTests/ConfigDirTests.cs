using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigDir;

namespace UnitTests
{
    [TestClass]
    public class ConfigDirTests : TestBase
    {
        [TestMethod]
        public void TestMethod1()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");
            Assert.AreEqual("Stand1", cfg.Stand.Name);

            cfg.Stand.Changeable = "Hello";

            Assert.AreEqual("Hello", cfg.Stand.Changeable);
            cfg = Config.GetOrCreate<Model.Configuration>("Config");
            Assert.AreEqual("Hello", cfg.Stand.Changeable);
        }
    }
}
