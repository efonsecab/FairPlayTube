using AutoMapper;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.AutoMapperProfiles
{
    public class GlobalMappingProfile: Profile
    {
        public GlobalMappingProfile()
        {
            this.CreateMap<VideoInfo, VideoInfoModel>().AfterMap(afterFunction: (source,dest)=> 
            {
                if (source.ApplicationUser != null)
                {
                    dest.Publisher = source.ApplicationUser.FullName;
                }
            });
        }
    }
}
