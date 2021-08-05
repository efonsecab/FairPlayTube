using FairPlayTube.ClientServices.Extensions;
using FairPlayTube.MauiBlazor.Authentication;
using FairPlayTube.MauiBlazor.Data;
using FairPlayTube.MauiBlazor.Features.LogOn;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Hosting;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

[assembly: XamlCompilationAttribute(XamlCompilationOptions.Compile)]

namespace FairPlayTube.MauiBlazor
{
    public class Startup : IStartup
    {
        public void Configure(IAppHostBuilder appBuilder)
        {
            appBuilder
                .RegisterBlazorMauiWebView(typeof(Startup).Assembly)
                .UseMicrosoftExtensionsServiceProviderFactory()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureServices(services =>
                {
                    services.AddOptions();
                    services.AddAuthorizationCore();
                    services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
                    services.AddBlazorWebView();
                    services.AddSingleton<WeatherForecastService>();

                    string assemblyName = "FairPlayTube";
                    string fairPlayTubeapiAddress = "https://localhost:44373";
                    services.AddScoped<BaseAddressAuthorizationMessageHandler>();
                    services.AddHttpClient($"{assemblyName}.ServerAPI", client =>
                client.BaseAddress = new Uri(fairPlayTubeapiAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

                    services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
                        client.BaseAddress = new Uri(fairPlayTubeapiAddress));

                    services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                        .CreateClient($"{assemblyName}.ServerAPI"));

                    services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                        .CreateClient($"{assemblyName}.ServerAPI.Anonymous"));
                    services.AddClientServices();
                });
        }
    }

    public class BaseAddressAuthorizationMessageHandler:DelegatingHandler
    {
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddAuthToken(request);
            return base.Send(request, cancellationToken);
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            AddAuthToken(request);
            return await base.SendAsync(request, cancellationToken);
        }

        private void AddAuthToken(HttpRequestMessage request)
        {
            request.Headers.Authorization = new System.Net.Http.Headers
                .AuthenticationHeaderValue("bearer", UserState.UserContext.AccessToken);
        }
    }
}