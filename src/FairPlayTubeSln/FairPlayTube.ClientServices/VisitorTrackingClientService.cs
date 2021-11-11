using FairPlayTube.Common.CustomExceptions;
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
        public static Guid SessionId { get; set; }
        private long? VisitorTrackingId { get; set; }
        private HttpClientService HttpClientService { get; }
        public VisitorTrackingClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task TrackAnonymousVisit(VisitorTrackingModel visitorTrackingModel,
            bool createNewSession)
        {
            if (createNewSession)
                SessionId = Guid.NewGuid();
            visitorTrackingModel.SessionId = SessionId;
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient.PostAsJsonAsync(ApiRoutes.VisitorTrackingController.TrackAnonymousClientInformation, visitorTrackingModel);
            if (!response.IsSuccessStatusCode)
            {
                try
                {
                    ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                    if (problemHttpResponse != null)
                        throw new CustomValidationException(problemHttpResponse.Detail);
                    else
                        throw new CustomValidationException(response.ReasonPhrase);
                }
                catch (Exception)
                {
                    string errorText = await response.Content.ReadAsStringAsync();
                    string reasonPhrase = response.ReasonPhrase;
                    throw new CustomValidationException($"{reasonPhrase} - {errorText}");
                }
            }
            else
            {
                visitorTrackingModel = await response.Content.ReadFromJsonAsync<VisitorTrackingModel>();
                this.VisitorTrackingId = visitorTrackingModel.VisitorTrackingId;
            }
        }

        public async Task TrackAuthenticatedVisit(VisitorTrackingModel visitorTrackingModel,
            bool createNewSession)
        {
            if (createNewSession)
                SessionId = Guid.NewGuid();
            visitorTrackingModel.SessionId = SessionId;
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(ApiRoutes.VisitorTrackingController.TrackAuthenticatedClientInformation, visitorTrackingModel);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
            else
            {
                visitorTrackingModel = await response.Content.ReadFromJsonAsync<VisitorTrackingModel>();
                this.VisitorTrackingId = visitorTrackingModel.VisitorTrackingId;
            }
        }

        public async Task UpdateVisitTimeElapsed()
        {
            var anonymousHttpClient = HttpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient.PutAsync(
                $"{ApiRoutes.VisitorTrackingController.UpdateVisitTimeElapsed}" +
                $"?visitorTrackingId={this.VisitorTrackingId}", null);
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
