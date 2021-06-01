using FairPlayTube.Client.Services;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.Video;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using ApiRoutes = FairPlayTube.Common.Global.Constants.ApiRoutes;

namespace FairPlayTube.Client.ClientServices
{
    public class VideoClientService
    {
        private HttpClientService HttpClientService { get; }
        private ToastifyService ToastifyService { get; }

        public VideoClientService(HttpClientService httpClientService, ToastifyService toastifyService)
        {
            this.HttpClientService = httpClientService;
            this.ToastifyService = toastifyService;
        }

        public async Task<VideoInfoModel[]> GetPublicProcessedVideosAsync()
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<VideoInfoModel[]>(
                ApiRoutes.VideoController.GetPublicProcessedVideos);
        }

        public async Task<VideoInfoModel[]> ListVideosByKeywordAsync(string Keyword)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<VideoInfoModel[]>(
                $"{ApiRoutes.VideoController.ListVideosByKeyword}" +
                $"?keyword={HttpUtility.UrlEncode(Keyword)}");
        }

        public async Task<VideoInfoModel[]> GetMyProcessedVideos()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<VideoInfoModel[]>(
                ApiRoutes.VideoController.GetMyProcessedVideos);
        }

        public async Task UploadVideoAsync(UploadVideoModel uploadVideoModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            authorizedHttpClient.Timeout = TimeSpan.FromMinutes(15);
            var response = await authorizedHttpClient.PostAsJsonAsync(ApiRoutes.VideoController.UploadVideo, uploadVideoModel);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    await this.ToastifyService.DisplayErrorNotification(problemHttpResponse.Detail);
                else
                    throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<string> GetVideoEditAccessToken(string videoId)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetStringAsync($"{ApiRoutes.VideoController.GetVideoEditAccessToken}" +
                $"?videoId={videoId}");
        }

        public async Task<GlobalKeywordModel[]> ListAllKeywordsAsync()
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<GlobalKeywordModel[]>
                (ApiRoutes.VideoController.ListAllKeywords);
        }
    }
}
