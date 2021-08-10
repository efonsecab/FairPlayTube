using FairPlayTube.Controllers;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.UserProfile;
using FairPlayTube.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FairPlayTube.ClientServices;
using System;
using FairPlayTube.DataAccess.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FairPlayTube.Models.UserMessage;

namespace FairPlayTube.Controllers.Tests
{
    [TestClass()]
    public class UserControllerTests : TestsBase
    {

        [TestInitialize()]
        public async Task TestInitialize()
        {
            await CleanData();
        }

        [ClassCleanup()]
        public static async Task ClassCleanup()
        {
            await CleanData();
        }

        public static async Task CleanData()
        {
            var dbContext = TestsBase.CreateDbContext();
            foreach (var singleUserFollower in dbContext.UserFollower)
            {
                dbContext.UserFollower.Remove(singleUserFollower);
            }
            await dbContext.SaveChangesAsync();
        }

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
        public async Task InviteUserTest()
        {
            await base.SignIn(Role.User);
            UserClientService userClientService = base.CreateUserClientService();
            await userClientService.InviteUserAsync(inviteUserModel: new Models.Invites.InviteUserModel()
            {
                CustomMessage = "This is a message from FairPlayTube Automated Tests",
                ToEmailAddress = "test@fairplaytube.local"
            });
        }

        [TestMethod()]
        public async Task SendMessageTest()
        {
            await base.SignIn(Role.User);
            UserClientService userClientService = base.CreateUserClientService();
            await userClientService.SendMessageAsync(new UserMessageModel()
            {
                Message = "PRUEBA", 
                ToApplicationUserId = 1
            });
        }

        [TestMethod()]
        public async Task AddUserFollowerTest()
        {
            var followedApplicationUserId = Guid.NewGuid();
            var dbContext = TestsBase.CreateDbContext();
            await dbContext.ApplicationUser.AddAsync(new ApplicationUser()
            {
                AzureAdB2cobjectId = followedApplicationUserId,
                EmailAddress = "test@test.test",
                FullName = "AUTOMATED TEST USER"
            });
            await dbContext.SaveChangesAsync();
            await base.SignIn(Role.User);
            UserClientService userClientService = base.CreateUserClientService();
            await userClientService.AddUserFollowerAsync(followedApplicationUserId: followedApplicationUserId);
            var followerEntity = await dbContext.UserFollower
                .SingleOrDefaultAsync(p => p.FollowedApplicationUser.AzureAdB2cobjectId == followedApplicationUserId);
            Assert.IsNotNull(followerEntity);
        }
    }
}