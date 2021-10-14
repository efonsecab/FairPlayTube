using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FairPlayTube
{
    /// <summary>
    /// Application Entryy class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Application entry method
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Initializes the Host Builder
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(configureLogging: (builderContext)=> 
            {
                builderContext.AddSimpleConsole(simpleConsoleOptions => 
                {
                    simpleConsoleOptions.TimestampFormat = "[HH:mm:ss] ";
                });
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddEnvironmentVariables(prefix: "SERVER_");
                config.AddUserSecrets<Program>(optional: true);
                var configRoot = config.Build();

                if (configRoot["AzureAppConfigConnectionString"] != null)
                {
                    config.AddAzureAppConfiguration(options =>
                    {
                        var azureAppConfigConnectionString =
                            configRoot["AzureAppConfigConnectionString"];
                        options
                            .Connect(azureAppConfigConnectionString).UseFeatureFlags(
                            featureFlagOptions =>
                            {
                                featureFlagOptions.CacheExpirationInterval = TimeSpan.FromMinutes(5);
                            });
                    });
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
    }
}
