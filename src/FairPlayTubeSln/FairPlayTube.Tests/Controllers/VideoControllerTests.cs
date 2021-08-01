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

        [TestCleanup]
        public async Task CleanTests()
        {
            using var dbContext = TestsBase.CreateDbContext();
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
        }

        [TestMethod()]
        public async Task GetMyProcessedVideosTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            var dbContext = TestsBase.CreateDbContext();
            var videos = await dbContext.VideoInfo.ToListAsync();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            var result = await authorizedHttpClient
                .GetFromJsonAsync<VideoInfoModel[]>(Constants.ApiRoutes.VideoController.GetMyProcessedVideos);
            Assert.AreEqual(1, result!.Length, "Invalid count of owned videos for test user");
        }

        [TestMethod()]
        public void GetPublicProcessedVideosTest()
        {
            Assert.Inconclusive();
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
            Assert.Inconclusive();
            //TODO: Insert test data: 1. VideoInfo and VideoIndexKeyword.
            //Remember to clean the test data in the [ClassCleanup] method
            var anonymousHttpClient = this.CreateAnonymousClient();
            var result = await anonymousHttpClient
                .GetFromJsonAsync<GlobalKeywordModel[]>(Constants.ApiRoutes.VideoController.ListAllKeywords);
            Assert.IsTrue(result!.Length > 0, "Invalid count of keywords");
        }

        [TestMethod()]
        public void ListVideosByKeywordTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public async Task BuyVideoAccessTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            var dbContext = TestsBase.CreateDbContext();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            userEntity.AvailableFunds = 25;
            await dbContext.SaveChangesAsync();
            var testVideoEntity = CreateTestVideoEntity();
            testVideoEntity.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(testVideoEntity);
            await dbContext.SaveChangesAsync();
            var response = await authorizedHttpClient.PostAsync($"{Constants.ApiRoutes.VideoController.BuyVideoAccess}" +
                $"?videoId={testVideoEntity.VideoId}", null!);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                Assert.Fail(message);
            }
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
        public void GetMyPendingVideosQueueTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public void AddVideoJobTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public void GetPersonsTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod()]
        public void AnalyzeVideoCommentTest()
        {
            Assert.Inconclusive();
        }
    }
}