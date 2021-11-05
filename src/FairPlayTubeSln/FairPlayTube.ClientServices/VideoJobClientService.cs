using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static FairPlayTube.Common.Global.Constants;

namespace FairPlayTube.ClientServices
{
    public class VideoJobClientService
    {
        private HttpClientService HttpClientService { get; }
        public VideoJobClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task AddVideoJobAsync(VideoJobModel videoJobModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            authorizedHttpClient.Timeout = TimeSpan.FromMinutes(15);
            var response = await authorizedHttpClient.PostAsJsonAsync(ApiRoutes.VideoJobController.AddVideoJob, videoJobModel);
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
