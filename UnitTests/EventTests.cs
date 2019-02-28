using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConfigDir;
using System;

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

            cfg.Finder.OnConfigError += (o) =>
            {
                throw new Exception();
            };

            cfg.Finder.OnValueFound += (o) =>
            {
                Assert.AreEqual("Config/Stand/Name", o.Path);
                Assert.AreEqual("Stand1", o.Value);
                Assert.AreEqual("Stand1", o.RawValue);
                Assert.AreEqual("XML файл: Config\\Stand.xml", o.Source.ToString());

                throw new StopTestException();
            };

            var s = cfg.Stand.Name;
        }

        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void ConfigError()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            cfg.Finder.OnValueFound += (_) =>
            {
                throw new Exception();
            };

            cfg.Finder.OnConfigError += (_) =>
            {
                throw new StopTestException();
            };

            var s = cfg.Filename.Options.Value5;
        }

        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void Validate()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");
            var s = cfg.Validated.Value1;
        }
    }
}
