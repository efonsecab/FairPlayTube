using FairPlayTube.Common.Global;
using FairPlayTube.Models.UserProfile;
using FairPlayTube.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests : TestsBase
    {
        [TestMethod()]
        public async Task GetMyRoleTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            var result = await authorizedHttpClient.GetStringAsync(Constants.ApiRoutes.UserController.GetMyRole);
            Assert.IsNotNull(result);
            Assert.AreEqual(result, Role.User.ToString());

            //authorizedHttpClient = await base.CreateAuthorizedClientAsync(Role.Admin);
            //var result = await authorizedHttpClient.GetStringAsync(Constants.ApiRoutes.UserController.GetMyRole);
            //Assert.IsNotNull(result);
            //Assert.AreEqual(result, Role.Admin);
        }

        [TestMethod()]
        public async Task ListUsersTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            var result = await authorizedHttpClient.GetFromJsonAsync<UserModel[]>(Constants.ApiRoutes.UserController.ListUsers);
            Assert.IsNotNull(result);
        }
    }
}