using FairPlayTube.ClientServices;
using FairPlayTube.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Tests.Controllers
{
    [TestClass]
    public class UserMessageControllerTests: TestsBase
    {
        [TestMethod]
        public async Task GetMyConversationsUsers()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            var dbContext = TestsBase.CreateDbContext();
            ApplicationUser destinataryUser = new ApplicationUser()
            {
                ApplicationUserStatusId = 1,
                EmailAddress = "test@test.local",
                FullName = "TST Cnv USER",
            };
            await dbContext.ApplicationUser.AddAsync(destinataryUser);
            await dbContext.SaveChangesAsync();
            var currentUser = dbContext.ApplicationUser.First();
            UserClientService userClientService = CreateUserClientService();
            await userClientService.SendMessageAsync(new Models.UserMessage.UserMessageModel() 
            {
                Message="Test Message",
                ToApplicationUserId=destinataryUser.ApplicationUserId
            });
            UserMessageClientService userMessageClientService = base.CreateUserMessageClientService();
            var result = await userMessageClientService.GetMyConversationsUsersAsync();
            Assert.IsNotNull(result);
        }
    }
}
