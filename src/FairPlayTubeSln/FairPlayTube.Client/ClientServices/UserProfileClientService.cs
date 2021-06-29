using FairPlayTube.Client.Services;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.UserProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static FairPlayTube.Common.Global.Constants;

namespace FairPlayTube.Client.ClientServices
{
    public class UserProfileClientService
    {
        private HttpClientService HttpClientService { get; }
        private ToastifyService ToastifyService { get; }

        public UserProfileClientService(HttpClientService httpClientService, ToastifyService toastifyService)
        {
            this.HttpClientService = httpClientService;
            this.ToastifyService = toastifyService;
        }

        public async Task SaveMonetizationAsync(GlobalMonetizationModel globalMonetizationModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(ApiRoutes.UserProfileController.SaveMonetization, 
                globalMonetizationModel);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    await this.ToastifyService.DisplayErrorNotification(problemHttpResponse.Detail);
                else
                    throw new Exception(response.ReasonPhrase);
            }
        }

        public async Task<GlobalMonetizationModel> GetMyMonetizationInfo()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<GlobalMonetizationModel>(
                Constants.ApiRoutes.UserProfileController.GetMyMonetizationInfo);
        }

        public async Task<UserModel[]> ListUsers()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<UserModel[]>(
                Constants.ApiRoutes.UserController.ListUsers);
        }
    }
}
