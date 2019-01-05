using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigDir;

namespace UnitTests
{
    [TestClass]
    public class ConfigDirTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");
            Assert.AreEqual("Stend1", cfg.Stend.Name);

            cfg.Stend.Changeable = "Hello";

            Assert.AreEqual("Hello", cfg.Stend.Changeable);
            cfg = Config.GetOrCreate<Model.Configuration>("Config");
            Assert.AreEqual("Hello", cfg.Stend.Changeable);
        }
    }
}
