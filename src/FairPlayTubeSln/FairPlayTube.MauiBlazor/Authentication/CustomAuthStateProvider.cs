using FairPlayTube.ClientServices;
using FairPlayTube.MauiBlazor.Features.LogOn;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.MauiBlazor.Authentication
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private UserClientService UserClientService { get; }
        public CustomAuthStateProvider(UserClientService userClientService)
        {
            this.UserClientService = userClientService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            if (UserState.UserContext.IsLoggedOn)
            {
                var role = await this.UserClientService.GetMyRoleAsync();
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
                identity.AddClaim(new Claim("oid", UserState.UserContext.UserIdentifier));
            }
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
    }
}
