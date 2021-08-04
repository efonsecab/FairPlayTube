using FairPlayTube.MauiBlazor.Features.LogOn;
using Microsoft.Identity.Client;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Application = Microsoft.Maui.Controls.Application;

namespace FairPlayTube.MauiBlazor
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            //DependencyService.Register<B2CAuthenticationService>();
            B2CConstants.PublicClientApp = PublicClientApplicationBuilder.Create(
                B2CConstants.ClientId)
                .WithB2CAuthority(B2CConstants.Authority)
                .WithRedirectUri(B2CConstants.RedirectUri)
                .Build();
            MainPage = new MainPage();
        }
    }
}
