using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.VideoJobApplications;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ApiRoutes = FairPlayTube.Common.Global.Constants.ApiRoutes;

namespace FairPlayTube.ClientServices
{
    public class VideoJobApplicationClientService
    {
        private HttpClientService HttpClientService { get; }

        public VideoJobApplicationClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task AddVideoJobApplication(CreateVideoJobApplicationModel createVideoJobApplicationModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsJsonAsync(ApiRoutes.VideoJobApplicationController.AddVideoJobApplication,
                createVideoJobApplicationModel);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
        }
    }
}
