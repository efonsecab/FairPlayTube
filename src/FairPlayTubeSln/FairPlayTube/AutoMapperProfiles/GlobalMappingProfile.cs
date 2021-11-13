using AutoMapper;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Localization;
using FairPlayTube.Models.Persons;
using FairPlayTube.Models.UserYouTubeChannel;
using FairPlayTube.Models.Video;
using FairPlayTube.Models.VideoComment;
using FairPlayTube.Models.VideoJobApplications;
using FairPlayTube.Models.VisitorTracking;
using PTI.Microservices.Library.YouTube.Models.GetChannelLatestVideos;
using System.Linq;

namespace FairPlayTube.AutoMapperProfiles
{
    /// <summary>
    /// Configures the Automapper mapping
    /// </summary>
    public class GlobalMappingProfile : Profile
    {
        /// <summary>
        /// Initializes <see cref="GlobalMappingProfile"/>
        /// </summary>
        public GlobalMappingProfile()
        {
            this.CreateMap<Person, PersonModel>().ConstructUsing(person => new PersonModel()
            {
                Id = person.Id,
                Name = person.Name,
                //PersonModelId=person.PersonModelId,
                //SampleFaceId=person.SampleFaceId,
                //SampleFaceSourceType=person.SampleFaceSourceType,
                //SampleFaceState = person.SampleFaceState,
                SampleFaceUrl = person.SampleFaceUrl
            });
            this.CreateMap<VideoInfo, VideoInfoModel>()
                .ForMember(p=>p.VideoIndexStatus, m => 
                {
                    m.Ignore();
                })
                .AfterMap(afterFunction: (source, dest) =>
                {
                    if (source.ApplicationUser != null)
                    {
                        dest.Publisher = source.ApplicationUser.FullName;
                        if (source.ApplicationUser.UserExternalMonetization != null
                        && source.ApplicationUser.UserExternalMonetization.Count > 0)
                        {
                            dest.UserGlobalMonetization = new Models.UserProfile.GlobalMonetizationModel()
                            {
                                MonetizationItems = source.ApplicationUser.UserExternalMonetization
                                .Select(p => new Models.UserProfile.MonetizationItem()
                                {
                                    MonetizationUrl = p.MonetizationUrl
                                }).ToList()
                            };

                        }
                        if (source.ApplicationUser.UserYouTubeChannel != null)
                        {
                            dest.YouTubeChannels = source.ApplicationUser.UserYouTubeChannel.Count;
                        }
                        if (source.VideoJob != null)
                        {
                            dest.AvailableJobs = source.VideoJob.Count;
                            dest.CombinedBudget = source.VideoJob.Sum(p => p.Budget);
                        }
                    }
                    if (source.VisitorTracking != null)
                    {
                        dest.VisitsCount = source.VisitorTracking.Count;
                    }
                });
            this.CreateMap<VideoComment, VideoCommentModel>().AfterMap((source, dest) =>
            {
                if (source.ApplicationUser != null && source.ApplicationUser.UserFollowerFollowedApplicationUser != null)
                {
                    var followersCount = source.ApplicationUser.UserFollowerFollowedApplicationUser.Count;
                    dest.ApplicationUserFollowersCount = followersCount;
                }
            });

            this.CreateMap<VideoJob, VideoJobModel>().AfterMap((source, dest) =>
            {
                if (source.VideoInfo != null)
                {
                    dest.VideoId = source.VideoInfo.VideoId;
                }
            });

            this.CreateMap<UserYouTubeChannel, UserYouTubeChannelModel>();
            this.CreateMap<Item, YouTubeVideoModel>();
            this.CreateMap<Resource, ResourceModel>().AfterMap((source, dest) =>
            {
                if (source.Culture != null)
                {
                    dest.CultureName = source.Culture.Name;
                }
            });
            this.CreateMap<VisitorTracking, VisitorTrackingModel>();
            this.CreateMap<VideoJobApplication, VideoJobApplicationModel>()
                .AfterMap(afterFunction: (source, dest) => 
                {
                    if (source.VideoJob is not null)
                    {
                        dest.VideoJobTitle = source.VideoJob.Title;
                        dest.VideoJobDescription = source.VideoJob.Description;
                    }
                });
        }
    }
}
