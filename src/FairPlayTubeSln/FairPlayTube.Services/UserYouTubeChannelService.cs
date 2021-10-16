using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.UserYouTubeChannel;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Services;
using PTI.Microservices.Library.YouTube.Models.GetChannelLatestVideos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class UserYouTubeChannelService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }

        private YoutubeService YoutubeService { get; }

        public UserYouTubeChannelService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            YoutubeService youtubeService)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.YoutubeService = youtubeService;
        }

        public async Task<UserYouTubeChannel> AddUserYouTubeChannelAsync(UserYouTubeChannelModel userYouTubeChannelModel,
            CancellationToken cancellationToken)
        {
            var entity = await this.FairplaytubeDatabaseContext.UserYouTubeChannel
                .SingleOrDefaultAsync(p => p.ApplicationUserId == userYouTubeChannelModel.ApplicationUserId 
                && p.YouTubeChannelId == userYouTubeChannelModel.YouTubeChannelId,
                cancellationToken: cancellationToken);
            if (entity is not null)
                throw new Exception($"User {userYouTubeChannelModel.ApplicationUserId} has already added Channel: {userYouTubeChannelModel.YouTubeChannelId}");
            entity = new DataAccess.Models.UserYouTubeChannel()
            {
                ApplicationUserId = userYouTubeChannelModel.ApplicationUserId,
                YouTubeChannelId = userYouTubeChannelModel.YouTubeChannelId
            };
            await this.FairplaytubeDatabaseContext.UserYouTubeChannel.AddAsync(entity, cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken:cancellationToken);
            return entity;
        }

        public IQueryable<UserYouTubeChannel> GetUserYouTubeChannels(long applicationUserId)
        {
            return this.FairplaytubeDatabaseContext.UserYouTubeChannel
                .Where(p => p.ApplicationUserId == applicationUserId);
        }

        public async Task<GetChannelLatestVideosResponse> GetYouTubeChannelLatestVideosAsync(string channelId,
            CancellationToken cancellationToken)
        {
            var result = await this.YoutubeService.GetChannelLatestVideosAsync(channelId);
            return result;
        }
    }
}
