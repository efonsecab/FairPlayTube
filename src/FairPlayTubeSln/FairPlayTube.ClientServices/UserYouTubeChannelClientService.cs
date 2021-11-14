using FairPlayTube.Common.Global;
using FairPlayTube.Models.UserYouTubeChannel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.ClientServices
{
    public class UserYouTubeChannelClientService
    {
        private HttpClientService HttpClientService { get; }

        public UserYouTubeChannelClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task<UserYouTubeChannelModel[]> GetUserYouTubeChannelsAsync(long applicationUserId)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<UserYouTubeChannelModel[]>(
                $"{Constants.ApiRoutes.UserYouTubeChannelController.GetUserYouTubeChannels}" +
                $"?applicationUserId={applicationUserId}");
        }

        public async Task<YouTubeVideoModel[]> GetYouTubeChannelLatestVideosAsync(string channelId)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<YouTubeVideoModel[]>(
                $"{Constants.ApiRoutes.UserYouTubeChannelController.GetYouTubeChannelLatestVideos}" +
                $"?channelId={channelId}");
        }
    }
}
