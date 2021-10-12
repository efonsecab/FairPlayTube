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
    }
}