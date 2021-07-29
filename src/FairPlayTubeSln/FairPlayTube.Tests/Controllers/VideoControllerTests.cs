using Microsoft.VisualStudio.TestTools.UnitTesting;
using FairPlayTube.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairPlayTube.Tests;
using FairPlayTube.Common.Global;
using System.Net.Http.Json;
using FairPlayTube.Models.Video;
using Microsoft.EntityFrameworkCore;
using FairPlayTube.DataAccess.Models;

namespace FairPlayTube.Controllers.Tests
{
    [TestClass()]
    public class VideoControllerTests : TestsBase
    {
        private static VideoInfo TestVideo = new()
        {
            Description = "AUTOMATED TEST VIDEO DESC",
            FileName = "AUTOMATEDTESTVIDEO.MP4",
            Name = "AUTOMATESTESTVIDEONAME",
            VideoIndexStatusId = (int)Common.Global.Enums.VideoIndexStatus.Processed,
            VideoBloblUrl = TestsBase.TestVideoBloblUrl,
            Location = TestsBase.AzureVideoIndexerConfiguration!.Location,
            AccountId = Guid.Parse(TestsBase.AzureVideoIndexerConfiguration.AccountId),
            Price=5,
            VideoId=Guid.NewGuid().ToString()
        };

        [ClassCleanup]
        public static async Task CleanTests()
        {
            using var dbContext = TestsBase.CreateDbContext();
            foreach (var singleVideoAccessTransaction in dbContext.VideoAccessTransaction)
            {
                dbContext.VideoAccessTransaction.Remove(singleVideoAccessTransaction);
            }
            await dbContext.SaveChangesAsync();
            var testEntity = await dbContext.VideoInfo.Where(p => p.Name == TestVideo.Name).SingleAsync();
            dbContext.VideoInfo.Remove(testEntity);
            await dbContext.SaveChangesAsync();
        }

        [TestMethod()]
        public async Task GetMyProcessedVideosTest()
        {
            var dbContext = TestsBase.CreateDbContext();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            TestVideo.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(TestVideo);
            await dbContext.SaveChangesAsync();
            var authorizedHttpClient = await base.CreateAuthorizedClientAsync(Role.User);
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
            var dbContext = TestsBase.CreateDbContext();
            var userEntity = await dbContext.ApplicationUser.Where(p => p.AzureAdB2cobjectId.ToString() ==
            TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId).SingleAsync();
            userEntity.AvailableFunds = 25;
            await dbContext.SaveChangesAsync();
            TestVideo.ApplicationUserId = userEntity.ApplicationUserId;
            await dbContext.VideoInfo.AddAsync(TestVideo);
            await dbContext.SaveChangesAsync();
            var authorizedHttpClient = await base.CreateAuthorizedClientAsync(Role.User);
            var response = await authorizedHttpClient.PostAsync($"{Constants.ApiRoutes.VideoController.BuyVideoAccess}" +
                $"?videoId={TestVideo.VideoId}",null!);
            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();
                Assert.Fail(message);
            }
        }
    }
}