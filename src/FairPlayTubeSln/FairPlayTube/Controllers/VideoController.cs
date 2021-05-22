using AutoMapper;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoController : ControllerBase
    {
        private VideoService VideoService { get; }
        private IMapper Mapper { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }

        public VideoController(VideoService videoService, IMapper mapper, ICurrentUserProvider currentUserProvider)
        {
            this.VideoService = videoService;
            this.Mapper = mapper;
            this.CurrentUserProvider = currentUserProvider;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<VideoInfoModel[]> GetPublicProcessedVideos()
        {
            var result = await this.VideoService.GetPublicProcessedVideosAsync()
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).ToArrayAsync();
            return result;
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<string> UploadVideo(UploadVideoModel uploadVideoModel)
        {
            return await this.VideoService.UploadVideoAsync(uploadVideoModel);
        }


        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<VideoInfoModel[]> GetMyProcessedVideos()
        {
            var azureAdB2cobjectId = this.CurrentUserProvider.GetObjectId();
            var result = await this.VideoService.GetPublicProcessedVideosByUserIdAsync(azureAdB2cobjectId)
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).ToArrayAsync();
            return result;

        }

        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<string> GetVideoEditAccessToken(string videoId)
        {
            var azureAdB2cobjectId = this.CurrentUserProvider.GetObjectId();
            bool isVideoOwner = await VideoService.IsVideoOwnerAsync(videoId: videoId, azureAdB2cobjectId: azureAdB2cobjectId);
            if (!isVideoOwner)
                throw new Exception("You are not allowed to edit this video");
            string accessToken = await this.VideoService.GetVideoEditAccessTokenAsync(videoId:videoId);
            return accessToken;
        }
    }
}
