using FairPlayTube.Models.Video;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ApiRoutes = FairPlayTube.Common.Global.Constants.ApiRoutes;

namespace FairPlayTube.Client.ClientServices
{
    public class VideoClientService
    {
        private HttpClientService HttpClientService { get; }
        public VideoClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }


        public async Task<VideoInfoModel[]> GetPublicProcessedVideosAsync()
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<VideoInfoModel[]>(
                ApiRoutes.VideoController.GetPublicProcessedVideos);
        }

        public async Task<string> UploadVideoAsync(UploadVideoModel uploadVideoModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            authorizedHttpClient.Timeout = TimeSpan.FromMinutes(15);
            var response = await authorizedHttpClient.PostAsJsonAsync(ApiRoutes.VideoController.UploadVideo, uploadVideoModel);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception(response.ReasonPhrase);
        }
    }
}
