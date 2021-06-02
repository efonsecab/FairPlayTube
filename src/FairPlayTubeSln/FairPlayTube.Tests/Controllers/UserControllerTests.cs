using Microsoft.VisualStudio.TestTools.UnitTesting;
using FairPlayTube.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairPlayTube.Tests;
using FairPlayTube.Common.Global;

namespace FairPlayTube.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests : TestsBase
    {
        [TestMethod()]
        public async Task GetMyRoleTest()
        {
            var authorizedHttpClient = await base.CreateAuthorizedClientAsync(Role.User);
            var result = await authorizedHttpClient.GetStringAsync(Constants.ApiRoutes.UserController.GetMyRole);
            Assert.IsNotNull(result);
            Assert.AreEqual(result, Role.User.ToString());

            //authorizedHttpClient = await base.CreateAuthorizedClientAsync(Role.Admin);
            //var result = await authorizedHttpClient.GetStringAsync(Constants.ApiRoutes.UserController.GetMyRole);
            //Assert.IsNotNull(result);
            //Assert.AreEqual(result, Role.Admin);
        }
    }
}