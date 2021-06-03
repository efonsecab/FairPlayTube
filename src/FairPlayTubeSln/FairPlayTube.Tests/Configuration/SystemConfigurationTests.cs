using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Tests.Configuration
{
    [TestClass]
    public class SystemConfigurationTests: TestsBase
    {
        [TestMethod]
        public void DefaultConnectionStringTests()
        {
            var result = TestsBase.Configuration.GetConnectionString(Common.Global.Constants.ConfigurationKeysNames.DefaultConnectionString);
            Assert.IsNotNull(result, "No connection string found");
        }
    }
}
