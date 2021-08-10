using FairPlayTube.Controllers;
using FairPlayTube.Common.Global;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using FairPlayTube.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FairPlayTube.ClientServices;

namespace FairPlayTube.Controllers.Tests
{
    [TestClass()]
    public class VideoControllerTests : TestsBase
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
            var testVideoEntity = CreateTestVideoEntity();
            dbContext.Entry<VideoInfo>(testVideoEntity).State = EntityState.Detached;
            foreach (var singleVideoAccessTransaction in dbContext.VideoAccessTransaction)
            {
                dbContext.VideoAccessTransaction.Remove(singleVideoAccessTransaction);
            }
            await dbContext.SaveChangesAsync();
            var testEntity = await dbContext.VideoInfo.Where(p => p.Name == testVideoEntity.Name).SingleOrDefaultAsync();
            if (testEntity != null)
            {
                dbContext.VideoInfo.Remove(testEntity);
                await dbContext.SaveChangesAsync();
            }
            foreach (var singlePerson in dbContext.Person)
            {
                dbContext.Person.Remove(singlePerson);
                await dbContext.SaveChangesAsync();
            }
        }

        [TestMethod()]
        public async Task GetMyProcessedVideosTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            VideoClientService videoClientService = base.CreateVideoClientService();
            var dbContext = TestsBase.CreateDbContext();
            var videos = await dbContext.VideoInfo.ToListAsync();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            var result = await videoClientService.GetMyProcessedVideos();
            Assert.AreEqual(1, result!.Length, "Invalid count of owned videos for test user");
        }

        [TestMethod()]
        public async Task GetPublicProcessedVideosTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            VideoClientService videoClientService = base.CreateVideoClientService();
            var dbContext = TestsBase.CreateDbContext();
            var videos = await dbContext.VideoInfo.ToListAsync();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Processed;
            testVideoEntity.VideoVisibilityId = (short)Common.Global.Enums.VideoVisibility.Public;
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            var result = await videoClientService.GetPublicProcessedVideosAsync();
            Assert.AreEqual(1, result.Length);
        }

        [TestMethod()]
        public void UploadVideoTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public void GetVideoEditAccessTokenTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public async Task ListAllKeywordsTest()
        {
            await base.SignIn(Role.User);
            VideoClientService videoClientService = base.CreateVideoClientService();
            var dbContext = TestsBase.CreateDbContext();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            await dbContext.VideoIndexKeyword.AddAsync(new VideoIndexKeyword()
            {
                Keyword = "TestKeyword",
                VideoInfoId = testVideoEntity.VideoInfoId
            });
            await dbContext.SaveChangesAsync();
            var result = await videoClientService.ListAllKeywordsAsync();
            Assert.IsTrue(result!.Length == 1, "Invalid count of keywords");
        }

        [TestMethod()]
        public void ListVideosByKeywordTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public async Task BuyVideoAccessTest()
        {
            await base.SignIn(Role.User);
            VideoClientService videoClientService = base.CreateVideoClientService();
            var dbContext = TestsBase.CreateDbContext();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            userEntity.AvailableFunds = 25;
            await dbContext.SaveChangesAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            await videoClientService.BuyVideoAccessAsync(testVideoEntity.VideoId);
        }

        [TestMethod()]
        public void UpdateMyVideoTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public void GetVideoTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public async Task GetMyPendingVideosQueueTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            VideoClientService videoClientService = base.CreateVideoClientService();
            var dbContext = TestsBase.CreateDbContext();
            var videos = await dbContext.VideoInfo.ToListAsync();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Pending;
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            var result = await videoClientService.GetMyPendingVideosQueue();
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod()]
        public void AddVideoJobTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public async Task GetPersonsTest()
        {
            await base.SignIn(Role.User);
            var dbContext = TestsBase.CreateDbContext();
            await dbContext.Person.AddAsync(new Person()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Name",
                PersonId = 1,
                SampleFaceId = Guid.NewGuid().ToString(),
                SampleFaceState = "ok",
                SampleFaceUrl = "http://wwww.test.local",
                SampleFaceSourceType = "video"
            });
            await dbContext.SaveChangesAsync();
            VideoClientService videoClientService = base.CreateVideoClientService();
            var result = await videoClientService.GetPersonsAsync();
            Assert.AreEqual(1, result.Length);
        }

        [TestMethod()]
        public void AnalyzeVideoCommentTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public void SearchVideosByPersonNameTest()
        {
            Assert.Inconclusive();
        }
    }
}