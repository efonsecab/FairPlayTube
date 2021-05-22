using FairPlayTube.Client.ClientServices;
using FairPlayTube.Common.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomClaims
{
    public class CustomAccountClaimsPrincipalFactory : AccountClaimsPrincipalFactory<CustomRemoteUserAccount>
    {
        private IAccessTokenProviderAccessor Accessor { get; }
        private HttpClientService HttpClientService { get; }

        public CustomAccountClaimsPrincipalFactory(IAccessTokenProviderAccessor accessor,
            HttpClientService httpClientService) : base(accessor)
        {
            this.Accessor = accessor;
            this.HttpClientService = httpClientService;
        }

        public async override ValueTask<ClaimsPrincipal> CreateUserAsync(CustomRemoteUserAccount account,
            RemoteAuthenticationUserOptions options)
        {
            var userClaimsPrincipal = await base.CreateUserAsync(account, options);
            if (userClaimsPrincipal.Identity.IsAuthenticated)
            {
                ClaimsIdentity claimsIdentity = userClaimsPrincipal.Identity as ClaimsIdentity;
                var userObjectId = claimsIdentity.Claims.GetAzureAdB2CUserObjectId();
                var httpClient = this.HttpClientService.CreateAuthorizedClient();
                var userRole = await httpClient.GetStringAsync($"api/User/GetUserRole?userAdB2CObjectId={userObjectId}");
                claimsIdentity.AddClaim(new Claim("Role", userRole));
            }
            return userClaimsPrincipal;
        }
    }
}
