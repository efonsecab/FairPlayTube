using FairPlayTube.Client.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.ClientServices.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientServicesExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddClientServices(this IServiceCollection services)
        {
            services.AddTransient<ToastifyService>();
            services.AddTransient<HttpClientService>();
            services.AddTransient<VideoClientService>();
            services.AddTransient<UserProfileClientService>();
            services.AddTransient<VisitorTrackingClientService>();
            services.AddTransient<UserClientService>();
            return services;
        }
    }
}
