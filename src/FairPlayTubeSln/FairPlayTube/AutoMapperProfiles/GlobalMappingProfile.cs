﻿using AutoMapper;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Persons;
using FairPlayTube.Models.UserYouTubeChannel;
using FairPlayTube.Models.Video;
using FairPlayTube.Models.VideoComment;
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
            this.CreateMap<VideoInfo, VideoInfoModel>().AfterMap(afterFunction: (source, dest) =>
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
                    if (source.VideoJob != null)
                    {
                        dest.AvailableJobs = source.VideoJob.Count;
                        dest.CombinedBudget = source.VideoJob.Sum(p => p.Budget);
                    }
                }

                dest.VideoIndexStatus = (Common.Global.Enums.VideoIndexStatus)source.VideoIndexStatusId;
            });
            this.CreateMap<VideoComment, VideoCommentModel>().AfterMap((source, dest) =>
            {
                if (source.ApplicationUser != null && source.ApplicationUser.UserFollowerFollowedApplicationUser != null)
                {
                    var followersCount = source.ApplicationUser.UserFollowerFollowedApplicationUser.LongCount();
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
        }
    }
}
