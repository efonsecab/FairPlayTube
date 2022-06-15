using Blazored.Toast;
using FairPlayTube.ClientServices.CustomLocalization;
using FairPlayTube.ClientServices.CustomLocalization.Api;
using FairPlayTube.MauiBlazor.Authentication;
using FairPlayTube.MauiBlazor.Features.LogOn;
using FairPlayTube.SharedConfiguration;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Polly;
using Polly.Extensions.Http;
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
            string strAppConfigStreamName = string.Empty;
            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            strAppConfigStreamName = "FairPlayTube.MauiBlazor.appsettings.Development.json";
#else
            strAppConfigStreamName = "FairPlayTube.MauiBlazor.appsettings.json";
#endif

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MauiProgram)).Assembly;
            var stream = assembly.GetManifestResourceStream(strAppConfigStreamName);
            builder.Configuration.AddJsonStream(stream);
            var services = builder.Services;
            builder.Services.AddBlazoredToast();
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
        .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>()
        .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
        .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
                client.BaseAddress = new Uri(fairPlayTubeapiAddress))
                .AddHttpMessageHandler<LocalizationMessageHandler>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{assemblyName}.ServerAPI"));

            services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
                .CreateClient($"{assemblyName}.ServerAPI.Anonymous"));
            ServicesSetup.AddCrossPlatformServices(builder.Services, builder.Configuration);
            services.AddLogging();
            services.AddScoped<IErrorBoundaryLogger, CustomBoundaryLogger>();
            var host = builder.Build();
            ModelsLocalizationSetup.ConfigureModelsLocalizers(host.Services);
            CultureInfo culture = new("en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            return builder.Build();
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
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