using FairPlayTube.Client.CustomClaims;
using FairPlayTube.Client.CustomProviders;
using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Components.FacebookButtons;
using FairPlayTube.Components.GoogleAdsense;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FairPlayTube.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string assemblyName = "FairPlayTube";
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient($"{assemblyName}.ServerAPI", client =>
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{assemblyName}.ServerAPI"));

            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{assemblyName}.ServerAPI.Anonymous"));

            builder.Services.AddMsalAuthentication<RemoteAuthenticationState, CustomRemoteUserAccount>(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                var defaultScope = builder.Configuration["AzureAdB2CScopes:DefaultScope"];
                options.ProviderOptions.DefaultAccessTokenScopes.Add(defaultScope);
                options.ProviderOptions.LoginMode = "redirect";
                options.UserOptions.NameClaim = "name";
                options.UserOptions.RoleClaim = "Role";
            }).AddAccountClaimsPrincipalFactory<
                RemoteAuthenticationState, CustomRemoteUserAccount, CustomAccountClaimsPrincipalFactory>();

            AzureQnABotConfiguration azureQnABotConfiguration =
                builder.Configuration.GetSection("AzureQnABotConfiguration").Get<AzureQnABotConfiguration>();
            builder.Services.AddSingleton(azureQnABotConfiguration);


            DisplayResponsiveAdConfiguration displayResponsiveAdConfiguration =
                builder.Configuration.GetSection("DisplayResponsiveAdConfiguration")
                .Get<DisplayResponsiveAdConfiguration>();
            builder.Services.AddSingleton(displayResponsiveAdConfiguration);

            FaceBookLikeButtonConfiguration faceBookLikeButtonConfiguration =
                builder.Configuration.GetSection(nameof(faceBookLikeButtonConfiguration))
                .Get<FaceBookLikeButtonConfiguration>();
            builder.Services.AddSingleton(faceBookLikeButtonConfiguration);

            builder.Services.AddTransient<IVideoEditAccessTokenProvider, VideoEditAccessTokenProvider>();

            builder.Services.AddTransient<HttpClientService>();
            builder.Services.AddTransient<ToastifyService>();
            builder.Services.AddTransient<VideoClientService>();
            builder.Services.AddTransient<UserProfileClientService>();
            builder.Services.AddTransient<ToastifyService>();
            builder.Services.AddTransient<VisitorTrackingClientService>();
            builder.Services.AddTransient<UserClientService>();
            builder.Services.AddTransient<SearchClientService>();
            builder.Services.AddTransient<VideoCommentClientService>();
            builder.Services.AddTransient<UserYouTubeChannelClientService>();
            await builder.Build().RunAsync();
        }
    }
}
