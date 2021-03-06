using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.Pagination;
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

        public async Task<PagedItems<VideoInfoModel>> GetPublicProcessedVideosAsync(
            PageRequestModel pageRequestModel)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<PagedItems<VideoInfoModel>>(
                $"{ApiRoutes.VideoController.GetPublicProcessedVideos}?" +
                $"{nameof(PageRequestModel.PageNumber)}={pageRequestModel.PageNumber}");
        }

        public async Task<List<VideoStatusModel>> GetMyPendingVideosQueueAsync()
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

        public async Task<VideoInfoModel[]> GetMyProcessedVideosAsync()
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
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
        }

        public async Task<string> GetVideoEditAccessTokenAsync(string accountId,string videoId)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetStringAsync($"{ApiRoutes.VideoController.GetVideoEditAccessToken}" +
                $"?accountId={accountId}&videoId={videoId}");
        }

        public async Task<GlobalKeywordModel[]> ListAllKeywordsAsync()
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<GlobalKeywordModel[]>
                (ApiRoutes.VideoController.ListAllKeywords);
        }

        public async Task UpdateMyVideoAsync(string videoId, UpdateVideoModel updateVideoModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PutAsJsonAsync($"{ApiRoutes.VideoController.UpdateMyVideo}" +
                $"?videoId={videoId}", updateVideoModel);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
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
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
        }

        public async Task<PersonModel[]> GetPersonsAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient.GetFromJsonAsync<PersonModel[]>(
                $"{ApiRoutes.VideoController.GetPersons}");
            return result;
        }

        public async Task DeleteVideoAsync(string accountId,string videoId)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsync(
                $"{ApiRoutes.VideoController.DeleteVideo}?accountId={accountId}&videoId={videoId}", null);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
        }

        public async Task<DownloadVideoModel> DownloadVideoAsync(string accountId,string videoId)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient.GetFromJsonAsync<DownloadVideoModel>(
                $"{ApiRoutes.VideoController.DownloadVideo}?accountId={accountId}&videoId={videoId}");
            return result;
        }

        public async Task<string[]> GetBoughtVideosIdsAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient
                .GetFromJsonAsync<string[]>(ApiRoutes.VideoController.GetBoughtVideosIds);
            return result;
        }
    }
}
