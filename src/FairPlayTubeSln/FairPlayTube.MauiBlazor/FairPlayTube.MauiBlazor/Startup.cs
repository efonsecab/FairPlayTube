using FairPlayTube.ClientServices.Extensions;
using FairPlayTube.MauiBlazor.Data;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Hosting;
using System;
using System.Net.Http;

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
                    services.AddBlazorWebView();
                    services.AddSingleton<WeatherForecastService>();

                    string assemblyName = "FairPlayTube";
                    string fairPlayTubeapiAddress = "https://localhost:44373";
                //    services.AddHttpClient($"{assemblyName}.ServerAPI", client =>
                //client.BaseAddress = new Uri(fairPlayTubeapiAddress))
                //.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

                    services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
                        client.BaseAddress = new Uri(fairPlayTubeapiAddress));

                    //services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                    //    .CreateClient($"{assemblyName}.ServerAPI"));

                    services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                        .CreateClient($"{assemblyName}.ServerAPI.Anonymous"));
                    services.AddClientServices();
                });
        }
    }
}