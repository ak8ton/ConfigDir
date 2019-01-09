using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigDir;

namespace UnitTests
{
    [TestClass]
    public class EventTests : TestBase
    {
        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void ValueFound()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            cfg.Data.OnValueFound += (o) => 
            {
                Assert.AreEqual("Config/Stend/Name", o.Path);
                Assert.AreEqual("Stend1", o.Value);
                Assert.AreEqual("Stend1", o.RawValue);
                Assert.AreEqual(typeof(string), o.ExpectedType);
                Assert.AreEqual(null, o.RequiredPath);
                Assert.AreEqual("XML файл: Config\\Stend.xml", o.Source.Description);

                throw new StopTestException();
            };

            var s = cfg.Stend.Name;
        }

        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void ValueNotFound()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            cfg.Data.OnValueNotFound += (o) =>
            {
                throw new StopTestException();
            };

            var s = cfg.Filename.Options.Value5;
        }
    }
}
