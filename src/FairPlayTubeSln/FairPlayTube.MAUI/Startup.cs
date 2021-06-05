using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Compatibility;
using FairPlayTube.MAUI.Data;
using System.Net.Http;
using System;

namespace FairPlayTube.MAUI
{
	public class Startup : IStartup
	{
		public void Configure(IAppHostBuilder appBuilder)
		{
			appBuilder
				.UseFormsCompatibility()
				.RegisterBlazorMauiWebView(typeof(Startup).Assembly)
				.UseMicrosoftExtensionsServiceProviderFactory()
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				})
				.ConfigureServices(services =>
				{
					var assemblyName = "FairPlayTube";
					services.AddBlazorWebView();
					services.AddSingleton<WeatherForecastService>();
					Uri baseAddress = new Uri("http://fairplaytube.pticostarica.com/");

					services.AddHttpClient($"{assemblyName}.ServerAPI.Anonymous", client =>
					client.BaseAddress = baseAddress);

					services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
					.CreateClient($"{assemblyName}.ServerAPI.Anonymous"));
					
					services.AddScoped<ClientServices.HttpClientService>();
					services.AddScoped<ClientServices.VideoClientService>();
				});
		}
	}
}