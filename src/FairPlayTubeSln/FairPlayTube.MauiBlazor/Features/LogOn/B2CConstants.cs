using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.MauiBlazor.Features.LogOn
{
    public class B2CConstants
    {
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string PolicySignUpSignIn { get; set; }
        public string PolicyEditProfile { get; set; }
        public string PolicyResetPassword { get; set; }

        public string ApiScopes { get; set; }
        public string[] ApiScopesArray => ApiScopes.Split(",");
        public string Authority { get; set; }
        public string AuthorityEditProfile { get; set; }
        public string AuthorityResetPassword { get; set; }
        public string RedirectUri { get; set; }
        public IPublicClientApplication PublicClientApp { get; set; }
    }
}
