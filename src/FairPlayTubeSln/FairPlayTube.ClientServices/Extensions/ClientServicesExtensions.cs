using FairPlayTube.Client.CustomProviders;
using FairPlayTube.Client.Services;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Components.FacebookButtons;
using FairPlayTube.Components.GoogleAdsense;
using FairPlayTube.Models.Validations.Video;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.ClientServices.Extensions
{
    public static class ClientServicesExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddCrossPlatformServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            DisplayResponsiveAdConfiguration displayResponsiveAdConfiguration =
                configuration.GetSection("DisplayResponsiveAdConfiguration")
                .Get<DisplayResponsiveAdConfiguration>();
            services.AddSingleton(displayResponsiveAdConfiguration);

            FaceBookLikeButtonConfiguration faceBookLikeButtonConfiguration =
                configuration.GetSection(nameof(faceBookLikeButtonConfiguration))
                .Get<FaceBookLikeButtonConfiguration>();
            services.AddSingleton(faceBookLikeButtonConfiguration);

            services.AddTransient<IVideoEditAccessTokenProvider, VideoEditAccessTokenProvider>();

            services.AddTransient<IVideoEditAccessTokenProvider, VideoEditAccessTokenProvider>();

            services.AddSingleton<LocalizationClientService>();
            services.AddTransient<HttpClientService>();
            services.AddTransient<ToastifyService>();
            services.AddTransient<VideoClientService>();
            services.AddTransient<UserProfileClientService>();

            services.AddTransient<VisitorTrackingClientService>();
            services.AddTransient<UserClientService>();
            services.AddTransient<SearchClientService>();
            services.AddTransient<VideoCommentClientService>();
            services.AddTransient<UserYouTubeChannelClientService>();
            services.AddTransient<VideoPlaylistClientService>();
            services.AddTransient<VideoJobClientService>();
            
            return services;
        }

        public static void ConfigureModelsLocalizers(this IServiceProvider services)
        {
            var localizerFactory = services.GetRequiredService<IStringLocalizerFactory>();
            UploadVideoModelLocalizer.Localizer =
                localizerFactory.Create(typeof(UploadVideoModelLocalizer))
                as IStringLocalizer<UploadVideoModelLocalizer>;
            VideoJobModelLocalizer.Localizer =
                localizerFactory.Create(typeof(VideoJobModelLocalizer))
                as IStringLocalizer<VideoJobModelLocalizer>;
        }
    }
}
