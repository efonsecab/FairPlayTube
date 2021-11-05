using FairPlayTube.Client.CustomClaims;
using FairPlayTube.Client.CustomLocalization.Api;
using FairPlayTube.Client.CustomProviders;
using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Components.FacebookButtons;
using FairPlayTube.Components.GoogleAdsense;
using FairPlayTube.Models.Validations.Video;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
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

            builder.Services.AddSingleton<IStringLocalizerFactory, ApiLocalizerFactory>();
            builder.Services.AddSingleton<IStringLocalizer, ApiLocalizer>();
            builder.Services.AddLocalization();

            builder.Services.AddScoped<LocalizationMessageHandler>();

            builder.Services.AddHttpClient($"{assemblyName}.ServerAPI", client =>
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<LocalizationMessageHandler>()
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<LocalizationMessageHandler>();

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

            builder.Services.AddSingleton<LocalizationClientService>();
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
            builder.Services.AddTransient<VideoPlaylistClientService>();
            var host = builder.Build();
            ConfigureModelsLocalizers(host);
            await host.SetDefaultCulture();
            await host.RunAsync();
        }

        private static void ConfigureModelsLocalizers(WebAssemblyHost host)
        {
            var localizerFactory =
                        host.Services.GetRequiredService<IStringLocalizerFactory>();
            UploadVideoModelLocalizer.Localizer =
                localizerFactory.Create(typeof(UploadVideoModelLocalizer))
                as IStringLocalizer<UploadVideoModelLocalizer>;
            VideoJobModelLocalizer.Localizer =
                localizerFactory.Create(typeof(VideoJobModelLocalizer)) 
                as IStringLocalizer<VideoJobModelLocalizer>;
        }
    }

    public class LocalizationMessageHandler: DelegatingHandler
    {
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentCulture = System.Globalization.CultureInfo.CurrentUICulture;
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(currentCulture.Name));
            return base.Send(request, cancellationToken);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var currentCulture = System.Globalization.CultureInfo.CurrentUICulture;
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(currentCulture.Name));
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
