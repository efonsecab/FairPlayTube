using FairPlayTube.Common.Global;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.VideoComment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.ClientServices
{
    public class VideoCommentClientService
    {
        private HttpClientService HttpClientService { get; }

        public VideoCommentClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }
        public async Task<VideoCommentModel[]> GetVideoCommentsAsync(string videoId)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            var result = await anonymousHttpClient
                .GetFromJsonAsync<VideoCommentModel[]>($"{Constants.ApiRoutes.VideoCommentController.GetVideoComments}" +
                $"?videoId={videoId}");
            return result;
        }

        public async Task AddVideoCommentAsync(CreateVideoCommentModel newCommentModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(Constants.ApiRoutes.VideoCommentController.AddVideoComment,
                newCommentModel);
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
