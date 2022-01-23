using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static FairPlayTube.Common.Global.Constants;

namespace FairPlayTube.ClientServices
{
    public class UserMessageClientService
    {
        private readonly HttpClientService HttpClientService;

        public UserMessageClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task<long[]> GetMyConversationsUsersAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<long[]>(
                ApiRoutes.UserMessagecontroller.GetMyConversationsUsers);
        }
    }
}
