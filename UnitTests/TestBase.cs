using ConfigDir;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    public class TestBase
    {
        [TestInitialize]
        public void Init()
        {
            Config.ResetAll();
        }
    }
}
