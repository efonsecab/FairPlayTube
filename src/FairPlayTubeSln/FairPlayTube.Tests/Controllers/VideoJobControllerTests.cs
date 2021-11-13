using FairPlayTube.ClientServices;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
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
    public class VideoJobControllerTests : TestsBase
    {
        private const string TestVideoName = "AUTOMATESTESTVIDEONAME";
        private static VideoInfo CreateTestVideoEntity()
        {
            return new()
            {
                Description = "AUTOMATED TEST VIDEO DESC",
                FileName = "AUTOMATEDTESTVIDEO.MP4",
                Name = TestVideoName,
                VideoIndexStatusId = (int)Common.Global.Enums.VideoIndexStatus.Processed,
                VideoBloblUrl = TestsBase.TestVideoBloblUrl,
                Location = TestsBase.AzureVideoIndexerConfiguration!.Location,
                AccountId = Guid.Parse(TestsBase.AzureVideoIndexerConfiguration.AccountId),
                Price = 5,
                VideoId = Guid.NewGuid().ToString()
            };
        }

        [TestInitialize]
        public async Task TestInitialize()
        {
            await CleanTestData();
        }

        [TestCleanup]
        public async Task TestCleanup()
        {
            await CleanTestData();
        }

        private static async Task CleanTestData()
        {
            var dbContext = TestsBase.CreateDbContext();
            foreach (var singleJob in dbContext.VideoJob)
            {
                dbContext.VideoJob.Remove(singleJob);
            }
            await dbContext.SaveChangesAsync();
            foreach (var singleVideo in dbContext.VideoInfo)
            {
                dbContext.VideoInfo.Remove(singleVideo);
            }
            await dbContext.SaveChangesAsync();
        }

        [TestMethod()]
        public async Task AddVideoJobTest()
        {
            await base.SignIn(Role.User);
            var dbContext = TestsBase.CreateDbContext();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            userEntity.AvailableFunds = 10;
            await dbContext.SaveChangesAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            VideoJobClientService videoJobClientService = CreateVideoJobClientService();
            VideoJobModel videoJobModel = new()
            {
                Budget = 5,
                Description = "Automated Test Description",
                Title = "Automated Test Job",
                VideoId = testVideoEntity.VideoId
            };
            await videoJobClientService.AddVideoJobAsync(videoJobModel);
            var result = await dbContext.VideoJob.SingleOrDefaultAsync();
            Assert.IsNotNull(result);
        }

        
        [TestMethod]
        public async Task GetVideosJobsTest()
        {
            await base.SignIn(Role.User);
            var dbContext = TestsBase.CreateDbContext();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            userEntity.AvailableFunds = 10;
            await dbContext.SaveChangesAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            VideoJobClientService videoJobClientService = CreateVideoJobClientService();
            VideoJobModel videoJobModel = new()
            {
                Budget = 5,
                Description = "Automated Test Description",
                Title = "Automated Test Job",
                VideoId = testVideoEntity.VideoId
            };
            await videoJobClientService.AddVideoJobAsync(videoJobModel);

            var allVideoJobs = await videoJobClientService.GetVideosJobs();
            Assert.AreEqual(expected:1, allVideoJobs.Length);
        }
    }
}
