using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FairPlayTube.Tests.Configuration
{
    [TestClass]
    public class SystemConfigurationTests : TestsBase
    {
        [TestMethod]
        public void DefaultConnectionStringTests()
        {
            var result = TestsBase.Configuration.GetConnectionString(Common.Global.Constants.ConfigurationKeysNames.DefaultConnectionString);
            Assert.IsNotNull(result, "No connection string found");
        }
    }
}
