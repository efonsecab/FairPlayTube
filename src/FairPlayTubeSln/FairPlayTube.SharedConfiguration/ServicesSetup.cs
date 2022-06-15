using FairPlayTube.ClientServices;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Components.FacebookButtons;
using FairPlayTube.Components.GoogleAdsense;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.SharedConfiguration
{
    public static class ServicesSetup
    {
        public static void AddCrossPlatformServices(IServiceCollection services,
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
            
            services.AddSingleton<LocalizationClientService>();
            services.AddTransient<HttpClientService>();
            //services.AddTransient<ToastifyService>();
            services.AddTransient<VideoClientService>();
            services.AddTransient<UserProfileClientService>();
            services.AddTransient<VisitorTrackingClientService>();
            services.AddTransient<UserClientService>();
            services.AddTransient<SearchClientService>();
            services.AddTransient<VideoCommentClientService>();
            services.AddTransient<UserYouTubeChannelClientService>();
            services.AddTransient<VideoPlaylistClientService>();
            services.AddTransient<VideoJobClientService>();
            services.AddTransient<VideoJobApplicationClientService>();
            services.AddTransient<FeatureClientService>();
            services.AddTransient<UserRequestClientService>();
            services.AddTransient<UserMessageClientService>();
            services.AddTransient<ClientSideErrorLogClientService>();

            services.AddTransient<IVideoEditAccessTokenProvider, VideoEditAccessTokenProvider>();

        }
    }
}
