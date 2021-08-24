using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.Persons;
using FairPlayTube.Models.Video;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using ApiRoutes = FairPlayTube.Common.Global.Constants.ApiRoutes;

namespace FairPlayTube.ClientServices
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

        public async Task<List<VideoStatusModel>> GetMyPendingVideosQueue()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<List<VideoStatusModel>>(
                ApiRoutes.VideoController.GetMyPendingVideosQueue);
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
                    throw new Exception(problemHttpResponse.Detail);
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

        public async Task UpdateMyVideo(string videoId, UpdateVideoModel updateVideoModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PutAsJsonAsync($"{ApiRoutes.VideoController.UpdateMyVideo}" +
                $"?videoId={videoId}", updateVideoModel);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new Exception(problemHttpResponse.Detail);
                else
                    throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<VideoInfoModel> GetVideoAsync(string videoId)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            var result = await anonymousHttpClient.GetFromJsonAsync<VideoInfoModel>(
                $"{ApiRoutes.VideoController.GetVideo}?videoId={videoId}");
            return result;
        }

        public async Task BuyVideoAccessAsync(string videoId)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(
                $"{ApiRoutes.VideoController.BuyVideoAccess}?videoId={videoId}", null);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new Exception(problemHttpResponse.Detail);
                else
                    throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<PersonModel[]> GetPersonsAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient.GetFromJsonAsync<PersonModel[]>(
                $"{ApiRoutes.VideoController.GetPersons}");
            return result;
        }

        public async Task DeleteVideoAsync(string videoId)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(
                $"{ApiRoutes.VideoController.DeleteVideo}?videoId={videoId}", null);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new Exception(problemHttpResponse.Detail);
                else
                    throw new Exception(response.ReasonPhrase);
            }
        }
    }
}
