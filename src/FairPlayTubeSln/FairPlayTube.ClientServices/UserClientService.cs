using FairPlayTube.Common.Global;
using FairPlayTube.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.ClientServices
{
    public class UserClientService
    {
        private HttpClientService HttpClientService { get; }
        public UserClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task<string> GetMyRoleAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient.GetStringAsync(Constants.ApiRoutes.UserController.GetMyRole);
            return result;
        }

        public async Task<UserModel[]> ListUsers()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<UserModel[]>(
                Constants.ApiRoutes.UserController.ListUsers);
        }
    }
}
