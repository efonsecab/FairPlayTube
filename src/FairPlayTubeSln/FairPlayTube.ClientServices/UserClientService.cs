using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Global;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Models.Invites;
using FairPlayTube.Models.SubscriptionPlan;
using FairPlayTube.Models.UserMessage;
using FairPlayTube.Models.UserProfile;
using FairPlayTube.Models.UserSubscription;
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

        public async Task SendMessageAsync(UserMessageModel userMessageModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync(
                    requestUri: Constants.ApiRoutes.UserController.SendMessage,
                    value: userMessageModel
                );

            if (!response.IsSuccessStatusCode)
            {
                var problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
        }

        public async Task<string[]> GetMyRolesAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient.GetFromJsonAsync<string[]>(Constants.ApiRoutes.UserController.GetMyRoles);
            return result;
        }

        public async Task<SubscriptionPlanModel> GetMySubscriptionAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient.GetFromJsonAsync<SubscriptionPlanModel>(Constants.ApiRoutes.UserController.GetMySubscription);
            return result;
        }
        public async Task<UserSubscriptionStatusModel> GetMySubscriptionStatusAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var result = await authorizedHttpClient
                .GetFromJsonAsync<UserSubscriptionStatusModel>(Constants.ApiRoutes.UserController.GetMySubscriptionStatus);
            return result;
        }

        public async Task<UserModel[]> ListUsersAsync()
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            return await authorizedHttpClient.GetFromJsonAsync<UserModel[]>(
                Constants.ApiRoutes.UserController.ListUsers);
        }

        public async Task InviteUserAsync(InviteUserModel inviteUserModel)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.PostAsJsonAsync<InviteUserModel>(
                Constants.ApiRoutes.UserController.InviteUser, inviteUserModel);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
        }

        public async Task AddUserFollowerAsync(long followedApplicationUserId)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient
                .PostAsync($"{Constants.ApiRoutes.UserController.AddUserFollower}" +
                $"?followedApplicationUserId={followedApplicationUserId}", null);
            if (!response.IsSuccessStatusCode)
            {
                ProblemHttpResponse problemHttpResponse = await response.Content.ReadFromJsonAsync<ProblemHttpResponse>();
                if (problemHttpResponse != null)
                    throw new CustomValidationException(problemHttpResponse.Detail);
                else
                    throw new CustomValidationException(response.ReasonPhrase);
            }
        }

        public async Task ValidateInviteCodeAsync(Guid inviteCode)
        {
            var authorizedHttpClient = this.HttpClientService.CreateAuthorizedClient();
            var response = await authorizedHttpClient.GetAsync(
                $"{Common.Global.Constants.ApiRoutes.UserController.ValidateUserInviteCode}?userInviteCode={inviteCode}");
            response.EnsureSuccessStatusCode();
        }
    }
}
