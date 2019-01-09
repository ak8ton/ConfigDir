using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigDir;

namespace UnitTests
{
    [TestClass]
    public class Filename : TestBase
    {
        [TestMethod]
        public void Prior()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            Assert.AreEqual("HighPriority", cfg.Filename.Options.Value1);
            Assert.AreEqual("MidPriority", cfg.Filename.Options.Value2);
            Assert.AreEqual("LowPriority", cfg.Filename.Options.Value3);
        }
    }
}
