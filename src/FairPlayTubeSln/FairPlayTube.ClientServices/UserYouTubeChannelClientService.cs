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
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<UserYouTubeChannelModel[]>(
                $"{Constants.ApiRoutes.UserYouTubeChannelController.GetUserYouTubeChannels}" +
                $"?applicationUserId={applicationUserId}");
        }

        public async Task<YouTubeVideoModel[]> GetYouTubeChannelLatestVideos(string channelId)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<YouTubeVideoModel[]>(
                $"{Constants.ApiRoutes.UserYouTubeChannelController.GetYouTubeChannelLatestVideos}" +
                $"?channelId={channelId}");
        }
    }
}
