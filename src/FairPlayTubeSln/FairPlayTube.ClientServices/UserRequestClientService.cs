using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.UsersRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static FairPlayTube.Common.Global.Constants;

namespace FairPlayTube.ClientServices
{
    public class UserRequestClientService
    {
        private HttpClientService HttpClientService { get; }

        public UserRequestClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task AddAnonymousUserRequestAsync(CreateUserRequestModel createUserRequestModel)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient.PostAsJsonAsync(
                ApiRoutes.UserRequestController.AddAnonymousUserRequest, createUserRequestModel);
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
