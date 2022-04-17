using FairPlayTube.ClientServices;
using FairPlayTube.Common.Extensions;
using FairPlayTube.Common.Global;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomClaims
{
    public class CustomAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<CustomRemoteUserAccount>
    {
        private HttpClientService HttpClientService { get; }
        private UserClientService UserClientService { get; }

        public CustomAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor,
            HttpClientService httpClientService, UserClientService userClientService) : base(accessor)
        {
            this.HttpClientService = httpClientService;
            this.UserClientService = userClientService;
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(CustomRemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var userClaimsPrincipal = await base.CreateUserAsync(account, options);
            if (userClaimsPrincipal.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = userClaimsPrincipal.Identity as ClaimsIdentity;
                _ = claimsIdentity.Claims.GetAzureAdB2CUserObjectId();
                var httpClient = this.HttpClientService.CreateAuthorizedClient();
                var userRoles = await UserClientService.GetMyRolesAsync();
                if (userRoles != null)
                    foreach (var singleRole in userRoles)
                    {
                        claimsIdentity.AddClaim(new Claim("Role", singleRole));
                    }
                var subscriptionPlan = await UserClientService.GetMySubscriptionAsync();
                if (subscriptionPlan != null)
                    claimsIdentity.AddClaim(new Claim("Subscription", subscriptionPlan.Name));
                var userStatus = await httpClient.GetStringAsync(Constants.ApiRoutes.UserController.GetMyUserStatus);
                if (!string.IsNullOrWhiteSpace(userStatus))
                    claimsIdentity.AddClaim(new Claim("UserStatus", userStatus));
            }
            return userClaimsPrincipal;
        }
    }
}
