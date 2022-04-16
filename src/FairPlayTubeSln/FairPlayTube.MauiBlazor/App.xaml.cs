using FairPlayTube.MauiBlazor.Features.LogOn;
using Microsoft.Identity.Client;

namespace FairPlayTube.MauiBlazor
{
    public partial class App : Application
    {
        public App(B2CConstants b2CConstants)
        {
            InitializeComponent();
            b2CConstants.PublicClientApp = PublicClientApplicationBuilder.Create(
                b2CConstants.ClientId)
                .WithB2CAuthority(b2CConstants.Authority)
                .WithRedirectUri(b2CConstants.RedirectUri)
                .Build();
            MainPage = new MainPage();
        }

#if ANDROID
        public static MainActivity ParentWindow { get; internal set; }

#endif
    }
}