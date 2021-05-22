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
using System.Threading;
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
        public async Task<VideoInfoModel[]> GetPublicProcessedVideos(CancellationToken cancellationToken)
        {
            var result = await this.VideoService.GetPublicProcessedVideos()
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).ToArrayAsync(cancellationToken:cancellationToken);
            return result;
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(1073741824)] //1GB
        public async Task<string> UploadVideo(UploadVideoModel uploadVideoModel, CancellationToken cancellationToken)
        {
            return await this.VideoService.UploadVideoAsync(uploadVideoModel, cancellationToken:cancellationToken);
        }


        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<VideoInfoModel[]> GetMyProcessedVideos(CancellationToken cancellationToken)
        {
            var azureAdB2cobjectId = this.CurrentUserProvider.GetObjectId();
            var result = await this.VideoService.GetPublicProcessedVideosByUserId(azureAdB2cobjectId)
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).ToArrayAsync(cancellationToken: cancellationToken);
            return result;

        }

        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<string> GetVideoEditAccessToken(string videoId, CancellationToken cancellationToken)
        {
            var azureAdB2cobjectId = this.CurrentUserProvider.GetObjectId();
            bool isVideoOwner = await VideoService.IsVideoOwnerAsync(videoId: videoId, azureAdB2cobjectId: azureAdB2cobjectId,
                cancellationToken:cancellationToken);
            if (!isVideoOwner)
                throw new Exception("You are not allowed to edit this video");
            string accessToken = await this.VideoService.GetVideoEditAccessTokenAsync(videoId:videoId);
            return accessToken;
        }
    }
}
