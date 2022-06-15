using Blazored.Toast;
using FairPlayTube.Client.CustomClaims;
using FairPlayTube.ClientServices.CustomLocalization.Api;
using FairPlayTube.Client.Services;
using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Components.FacebookButtons;
using FairPlayTube.Components.GoogleAdsense;
using FairPlayTube.SharedConfiguration;
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
using System.Net;
using Microsoft.AspNetCore.Components.Web;

namespace FairPlayTube.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            string assemblyName = "FairPlayTube";
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<IStringLocalizerFactory, ApiLocalizerFactory>();
            builder.Services.AddSingleton<IStringLocalizer, ApiLocalizer>();
            builder.Services.AddLocalization();

            builder.Services.AddScoped<LocalizationMessageHandler>();

            builder.Services.AddHttpClient($"{assemblyName}.ServerAPI", client =>
                {
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                    client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                    client.DefaultRequestVersion = HttpVersion.Version20;
                })
                .AddHttpMessageHandler<LocalizationMessageHandler>()
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
            {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                client.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
                client.DefaultRequestVersion = HttpVersion.Version20;
            })
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


            builder.Services.AddTransient<ToastifyService>();
            ServicesSetup.AddCrossPlatformServices(builder.Services, builder.Configuration);

            var host = builder.Build();
            ModelsLocalizationSetup.ConfigureModelsLocalizers(host.Services);
            await host.SetDefaultCulture();
            await host.RunAsync();
        }
    }

    public class LocalizationMessageHandler : DelegatingHandler
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
