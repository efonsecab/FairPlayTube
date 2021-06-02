using AutoMapper;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.AutoMapperProfiles
{
    public class GlobalMappingProfile : Profile
    {
        public GlobalMappingProfile()
        {
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
                }
            });
        }
    }
}
