using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.MauiBlazor.Features.LogOn
{
    public static class B2CConstants
    {
        public static string Tenant = "pticostaricadev.onmicrosoft.com";
        public static string ClientId = "110facd2-07c2-4ecd-80e4-aeb3627100cf";
        public static string PolicySignUpSignIn = "b2c_1_signupsignin";
        public static string PolicyEditProfile = "b2c_1_edit_profile";
        public static string PolicyResetPassword = "b2c_1_reset";

        public static string[] ApiScopes = { "https://pticostaricadev.onmicrosoft.com/525bccd5-2ac9-4ba7-9b55-e1e6be50c464/API.Access" };

        private static string BaseAuthority = "https://pticostaricadev.b2clogin.com/tfp/{tenant}/{policy}/oauth2/v2.0/authorize";
        public static string Authority = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicySignUpSignIn);
        public static string AuthorityEditProfile = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyEditProfile);
        public static string AuthorityResetPassword = BaseAuthority.Replace("{tenant}", Tenant).Replace("{policy}", PolicyResetPassword);
        public static readonly string RedirectUri = $"msal110facd2-07c2-4ecd-80e4-aeb3627100cf://auth";
        public static IPublicClientApplication PublicClientApp { get; set; }
    }
}
