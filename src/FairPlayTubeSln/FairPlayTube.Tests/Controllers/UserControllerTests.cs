using FairPlayTube.Controllers;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.UserProfile;
using FairPlayTube.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FairPlayTube.ClientServices;

namespace FairPlayTube.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests : TestsBase
    {
        [TestMethod()]
        public async Task GetMyRoleTest()
        {
            await base.SignIn(Role.User);
            UserClientService userClientService = base.CreateUserClientService();
            var result = await userClientService.GetMyRoleAsync();
            Assert.IsNotNull(result);
            Assert.AreEqual(result, Role.User.ToString());
        }

        [TestMethod()]
        public async Task ListUsersTest()
        {
            await base.SignIn(Role.User);
            UserClientService userClientService = base.CreateUserClientService();
            var result = await userClientService.ListUsers();
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void InviteUserTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public void SendMessageTest()
        {
            Assert.Inconclusive();
        }
    }
}