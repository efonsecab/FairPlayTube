using FairPlayTube.Models.UserProfile.Localizers;
using FairPlayTube.Models.UsersRequests.Localizers;
using FairPlayTube.Models.Validations.Video;
using FairPlayTube.Models.Validations.VideoJobApplications;
using FairPlayTube.Models.VideoJobApplications.Localizers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace FairPlayTube.SharedConfiguration
{
    public static class ModelsLocalizationSetup
    {
        public static void ConfigureModelsLocalizers(IServiceProvider services)
        {
            var localizerFactory = services.GetRequiredService<IStringLocalizerFactory>();
            UploadVideoModelLocalizer.Localizer =
                localizerFactory.Create(typeof(UploadVideoModelLocalizer))
                as IStringLocalizer<UploadVideoModelLocalizer>;
            VideoJobModelLocalizer.Localizer =
                localizerFactory.Create(typeof(VideoJobModelLocalizer))
                as IStringLocalizer<VideoJobModelLocalizer>;
            CreateVideoJobApplicationLocalizer.Localizer =
                localizerFactory.Create(typeof(CreateVideoJobApplicationLocalizer))
                as IStringLocalizer<CreateVideoJobApplicationLocalizer>;
            VideoJobApplicationLocalizer.Localizer =
                localizerFactory.Create(typeof(VideoJobApplicationLocalizer))
                as IStringLocalizer<VideoJobApplicationLocalizer>;
            UpdateUserProfileModelLocalizer.Localizer =
                localizerFactory.Create(typeof(UpdateUserProfileModelLocalizer))
                as IStringLocalizer<UpdateUserProfileModelLocalizer>;
            CreateUserRequestModelLocalizer.Localizer =
                localizerFactory.Create(typeof(CreateUserRequestModelLocalizer))
                as IStringLocalizer<CreateUserRequestModelLocalizer>;
        }
    }
}