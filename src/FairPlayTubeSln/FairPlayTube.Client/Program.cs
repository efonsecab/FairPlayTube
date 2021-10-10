using FairPlayTube.Client.CustomClaims;
using FairPlayTube.Client.CustomProviders;
using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
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


            DisplayResponsiveAdConfiguration displayResponsiveAdConfiguration =
                builder.Configuration.GetSection("DisplayResponsiveAdConfiguration")
                .Get<DisplayResponsiveAdConfiguration>();
            builder.Services.AddSingleton(displayResponsiveAdConfiguration);

            builder.Services.AddTransient<IVideoEditAccessTokenProvider, VideoEditAccessTokenProvider>();
            ConfigureCommonServices(builder.Services);
            await builder.Build().RunAsync();
        }

        public static void ConfigureCommonServices(IServiceCollection services)
        {
            services.AddTransient<HttpClientService>();
            services.AddTransient<ToastifyService>();
            services.AddTransient<VideoClientService>();
            services.AddTransient<UserProfileClientService>();
            services.AddTransient<ToastifyService>();
            services.AddTransient<VisitorTrackingClientService>();
            services.AddTransient<UserClientService>();
        }
    }
}
