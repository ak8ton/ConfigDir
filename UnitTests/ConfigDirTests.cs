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
        }
    }
}
