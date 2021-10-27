using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.VisitorTracking;
using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static FairPlayTube.Common.Global.Constants;

namespace FairPlayTube.ClientServices
{
    public class VisitorTrackingClientService
    {
        private HttpClientService HttpClientService { get; }
        public VisitorTrackingClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task TrackAnonymousVisit(VisitorTrackingModel visitorTrackingModel)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient.PostAsJsonAsync(ApiRoutes.VisitorTrackingController.TrackAnonymousClientInformation, visitorTrackingModel);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new Exception(problemHttpResponse.Detail);
                else
                    throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task TrackAuthenticatedVisit(VisitorTrackingModel visitorTrackingModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(ApiRoutes.VisitorTrackingController.TrackAuthenticatedClientInformation, visitorTrackingModel);
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
