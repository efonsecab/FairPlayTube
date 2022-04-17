﻿using FairPlayTube.ClientServices.CustomLocalization;
using FairPlayTube.ClientServices.CustomLocalization.Api;
using FairPlayTube.ClientServices.Extensions;
using FairPlayTube.MauiBlazor.Authentication;
using FairPlayTube.MauiBlazor.Features.LogOn;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Reflection;

namespace FairPlayTube.MauiBlazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
            var stream = assembly.GetManifestResourceStream("FairPlayTube.MauiBlazor.appsettings.Development.json");
            builder.Configuration.AddJsonStream(stream);
            var services = builder.Services;

            services.AddSingleton<IStringLocalizerFactory, ApiLocalizerFactory>();
            services.AddSingleton<IStringLocalizer, ApiLocalizer>();
            services.AddLocalization();

            builder.Services.AddScoped<LocalizationMessageHandler>();
            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

            string assemblyName = "FairPlayTube";
            string fairPlayTubeapiAddress = builder.Configuration["ApiBaseUrl"];
            B2CConstants b2CConstants = builder.Configuration.GetSection("B2CConstants").Get<B2CConstants>();
            builder.Services.AddSingleton(b2CConstants);
            /* When running in an emulator localhost woult not work as expected.
             * You need to do forwarding, you can use ngrok, check an example before
             * Use your correct FairPlayTube API port
             * */
            //ngrok.exe http https://localhost:44373 -host-header="localhost:44373"
            //string fairPlayTubeapiAddress = "REPLACE_WITH_NGROK_GENERATED_URL";
            services.AddScoped<BaseAddressAuthorizationMessageHandler>();
            services.AddHttpClient($"{assemblyName}.ServerAPI", client =>
        client.BaseAddress = new Uri(fairPlayTubeapiAddress))
        .AddHttpMessageHandler<LocalizationMessageHandler>()
        .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
                client.BaseAddress = new Uri(fairPlayTubeapiAddress))
                .AddHttpMessageHandler<LocalizationMessageHandler>();

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{assemblyName}.ServerAPI"));

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{assemblyName}.ServerAPI.Anonymous"));
            services.AddCrossPlatformServices(builder.Configuration);
            services.AddLogging();
            services.AddScoped<IErrorBoundaryLogger, CustomBoundaryLogger>();
            var host = builder.Build();
            host.Services.ConfigureModelsLocalizers();
            CultureInfo culture = new("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            return builder.Build();
        }
    }

    public class CustomBoundaryLogger : IErrorBoundaryLogger
    {
        public ValueTask LogErrorAsync(Exception exception)
        {
            return ValueTask.CompletedTask;
        }
    }

    public class BaseAddressAuthorizationMessageHandler : DelegatingHandler
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