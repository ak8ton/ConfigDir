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

            cfg.Finder.OnValueFound += (o) =>
            {
                Assert.AreEqual("Config/Stand/Name", o.Path);
                Assert.AreEqual("Stand1", o.Value);
                Assert.AreEqual("Stand1", o.RawValue);
                Assert.AreEqual(typeof(string), o.ExpectedType);
                Assert.AreEqual("XML файл: Config\\Stand.xml", o.Source.Description);

                throw new StopTestException();
            };

            var s = cfg.Stand.Name;
        }

        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void ValueNotFound_EndValue()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            cfg.Finder.OnValueNotFound += (o) =>
            {
                Assert.AreEqual("Config/Filename/Options/Value5", o.Path);
                Assert.AreEqual(null, o.Value);
                Assert.AreEqual(null, o.RawValue);
                Assert.AreEqual(null, o.ExpectedType);
                Assert.AreEqual(null, o.Source);

                throw new StopTestException();
            };

            var s = cfg.Filename.Options.Value5;
        }

        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void ValueNotFound_SubConfig()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            cfg.Finder.OnValueNotFound += (o) =>
            {
                Assert.AreEqual("Config/NotExsists/Value5", o.Path);
                Assert.AreEqual(null, o.Value);
                Assert.AreEqual(null, o.RawValue);
                Assert.AreEqual(null, o.ExpectedType);
                Assert.AreEqual(null, o.Source);

                throw new StopTestException();
            };

            var s = cfg.NotExsists.Value5;
        }

        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void ValueTypeError_EndValue()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            cfg.Finder.OnValueTypeError += (o) =>
            {
                Assert.AreEqual("Config/TypeError/NotIntegerValue", o.Path);
                Assert.AreEqual(null, o.Value);
                Assert.AreEqual("NotInteger", o.RawValue);
                Assert.AreEqual(typeof(int), o.ExpectedType);
                Assert.AreEqual("XML файл: Config\\TypeError.xml", o.Source.Description);

                throw new StopTestException();
            };

            var s = cfg.TypeError.NotIntegerValue;
        }

        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void ValueTypeError_SubConfig()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            cfg.Finder.OnValueTypeError += (o) =>
            {
                Assert.AreEqual("Config/TypeError/NotSubConfig", o.Path);
                Assert.AreEqual(null, o.Value);
                Assert.AreEqual("NotSubConfig", o.RawValue);
                Assert.AreEqual(typeof(Model.IOptions), o.ExpectedType);
                Assert.AreEqual("XML файл: Config\\TypeError.xml", o.Source.Description);

                throw new StopTestException();
            };

            var s = cfg.TypeError.NotSubConfig.Value1;
        }

        [TestMethod]
        [ExpectedException(typeof(StopTestException))]
        public void ValueTypeError_EmptyValue()
        {
            var cfg = Config.GetOrCreate<Model.Configuration>("Config");

            cfg.Finder.OnValueTypeError += (o) =>
            {
                Assert.AreEqual("Config/TypeError/NotSubConfig", o.Path);
                Assert.AreEqual(null, o.Value);
                Assert.AreEqual("NotSubConfig", o.RawValue);
                Assert.AreEqual(typeof(Model.IOptions), o.ExpectedType);
                Assert.AreEqual("XML файл: Config\\TypeError.xml", o.Source.Description);

                throw new StopTestException();
            };

            var s = cfg.TypeError.EmptyValueHolder.EmptyValue;
        }
    }
}
