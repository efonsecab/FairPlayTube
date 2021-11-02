using Microsoft.VisualStudio.TestTools.UnitTesting;
using FairPlayTube.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FairPlayTube.Tests;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.Video;
using Microsoft.EntityFrameworkCore;

namespace FairPlayTube.Controllers.Tests
{
    [TestClass]
    public class VideoPlaylistControllerTests : TestsBase
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
            foreach (var singleVideoPlaylist in dbContext.VideoPlaylist)
            {
                dbContext.VideoPlaylist.Remove(singleVideoPlaylist);
            }
            await dbContext.SaveChangesAsync();
        }

        [TestMethod]
        public async Task CreateVideoPlaylistTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            var dbContext = TestsBase.CreateDbContext();
            VideoPlaylistModel testVideoPlaylist = CreateTestVideoPlaylist();
            VideoPlaylistClientService videoPlaylistClientService = base.CreateVideoPlaylistClientService();
            await videoPlaylistClientService.CreateVideoPlaylist(testVideoPlaylist);
            var entity = await dbContext.VideoPlaylist
                .Include(p => p.OwnerApplicationUser)
                .Where(p => p.PlaylistName == testVideoPlaylist.PlaylistName &&
                p.OwnerApplicationUser.AzureAdB2cobjectId.ToString() == TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId)
                .AsNoTracking().SingleOrDefaultAsync();

            Assert.IsNotNull(entity);
        }

        [TestMethod]
        public async Task DeleteVideoPlaylistTest()
        {
            var authorizedHttpClient = await base.SignIn(Role.User);
            var dbContext = TestsBase.CreateDbContext();
            VideoPlaylistModel testVideoPlaylist = CreateTestVideoPlaylist();
            VideoPlaylistClientService videoPlaylistClientService = base.CreateVideoPlaylistClientService();
            await videoPlaylistClientService.CreateVideoPlaylist(testVideoPlaylist);
            var entity = await dbContext.VideoPlaylist
                .Include(p => p.OwnerApplicationUser)
                .Where(p => p.PlaylistName == testVideoPlaylist.PlaylistName &&
                p.OwnerApplicationUser.AzureAdB2cobjectId.ToString() == TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId)
                .AsNoTracking().SingleOrDefaultAsync();

            Assert.IsNotNull(entity);
            await videoPlaylistClientService.DeleteVideoPlaylist(entity.VideoPlaylistId);
            entity = await dbContext.VideoPlaylist
                .Include(p => p.OwnerApplicationUser)
                .Where(p => p.PlaylistName == testVideoPlaylist.PlaylistName &&
                p.OwnerApplicationUser.AzureAdB2cobjectId.ToString() == TestsBase.TestAzureAdB2CAuthConfiguration!.AzureAdUserObjectId)
                .AsNoTracking().SingleOrDefaultAsync();

            Assert.IsNull(entity);
        }

        private static VideoPlaylistModel CreateTestVideoPlaylist()
        {
            return new VideoPlaylistModel()
            {
                PlaylistName = "Automated Test Playlist",
                PlaylistDescription = "Description for Automated Test Playlisit"
            };
        }

        [TestMethod()]
        public async Task AddVideoToPlaylistTest()
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
            VideoPlaylist testVideoPlaylist = new VideoPlaylist()
            {
                PlaylistDescription = "Test Desc",
                PlaylistName = "Test Name",
                OwnerApplicationUserId = userEntity.ApplicationUserId
            };
            await dbContext.VideoPlaylist.AddAsync(testVideoPlaylist);
            await dbContext.SaveChangesAsync();
            VideoPlaylistItemModel videoPlaylistItemModel = new VideoPlaylistItemModel()
            {
                VideoPlaylistId = testVideoPlaylist.VideoPlaylistId,
                VideoId = testVideoEntity.VideoId,
                Order = 1
            };
            VideoPlaylistClientService videoPlaylistClientService = base.CreateVideoPlaylistClientService();
            await videoPlaylistClientService.AddVideoToPlaylist(videoPlaylistItemModel);
            var result = await dbContext.VideoPlaylistItem.SingleOrDefaultAsync(p=>p.VideoInfoId == testVideoEntity.VideoInfoId);
            Assert.IsNotNull(result);
        }
    }
}