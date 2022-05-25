using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.ClientSideErrorLog;
using FairPlayTube.Models.CustomHttpResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.ClientServices
{
    public class ClientSideErrorLogClientService
    {
        private readonly HttpClientService HttpClientService;

        public ClientSideErrorLogClientService(HttpClientService httpClientService)
        {
            this.HttpClientService=httpClientService;
        }

        public async Task AddClientSideErrorAsync(CreateClientSideErrorLogModel createClientSideErrorLogModel)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            var response = await anonymousHttpClient.PostAsJsonAsync<CreateClientSideErrorLogModel>(
                Constants.ApiRoutes.ClientSideErrorLogController.AddClientSideError,
                createClientSideErrorLogModel);
            if (!response.IsSuccessStatusCode)
            {
                var problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
        }
    }
}
