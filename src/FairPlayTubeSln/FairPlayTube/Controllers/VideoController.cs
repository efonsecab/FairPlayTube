using AutoMapper;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Persons;
using FairPlayTube.Models.Video;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Models.AzureVideoIndexerService;
using System;
using System.Collections.Generic;
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
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(1073741824)] //1GB
        public async Task<IActionResult> UploadVideo(UploadVideoModel uploadVideoModel, CancellationToken cancellationToken)
        {
            if (await this.VideoService.UploadVideoAsync(uploadVideoModel, cancellationToken: cancellationToken))
                return Ok();
            else
                throw new Exception("An error occurred trying to upload your video");
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
                cancellationToken: cancellationToken);
            if (!isVideoOwner)
                throw new Exception("You are not allowed to edit this video");
            string accessToken = await this.VideoService.GetVideoEditAccessTokenAsync(videoId: videoId);
            return accessToken;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<GlobalKeywordModel[]> ListAllKeywords(CancellationToken cancellationToken)
        {
            return await this.VideoService.ListAllKeywordsAsync(cancellationToken: cancellationToken);
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<VideoInfoModel[]> ListVideosByKeyword(string keyword,
            CancellationToken cancellationToken)
        {
            var result = await this.VideoService.GetPublicProcessedVideosByKeyword(keyword)
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }


        [HttpPut("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<IActionResult> UpdateMyVideo(string videoId, UpdateVideoModel model,
            CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();

            if (!await this.VideoService.IsVideoOwnerAsync(videoId, userObjectId, cancellationToken))
                throw new Exception($"User {userObjectId} is not allowed to modify Video: {videoId}");
            await this.VideoService.UpdateVideo(videoId, model);
            return Ok();
        }

        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<VideoInfoModel> GetVideo(string videoId, CancellationToken cancellationToken)
        {
            return await this.VideoService.GetvideoAsync(videoId).Select(
                p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).SingleOrDefaultAsync();
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task BuyVideoAccess(string videoId, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            await VideoService.BuyVideoAccessAsync(azureAdB2CObjectId: userObjectId, videoId: videoId, cancellationToken: cancellationToken);
        }


        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<List<VideoStatusModel>> GetMyPendingVideosQueue(CancellationToken cancellationToken)
        {
            List<VideoStatusModel> result = new List<VideoStatusModel>();
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var userVideosQueue = await this.VideoService
                .GetUserPendingVideosQueueAsync(azureAdB2cobjectId: userObjectId, cancellationToken: cancellationToken);
            if (userVideosQueue == null)
                return null;
            var processingVideos = userVideosQueue
                .Where(p => p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processing)
                .ToArray();
            var pendingVideos = userVideosQueue.Except(processingVideos);
            result.AddRange(pendingVideos.Select(p => new VideoStatusModel()
            {
                Name = p.Name,
                ProcessingProgress = "0%",
                Status = p.VideoIndexStatus.Name,
                VideoId = p.VideoId
            }));
            if (processingVideos != null && processingVideos.Length > 0)
            {
                var processingVideosIds = processingVideos.Select(p => p.VideoId).ToArray();
                var processingVideosStatuses = await VideoService.GetVideoIndexerStatus(processingVideosIds, cancellationToken);
                result.AddRange(processingVideosStatuses.results.Select(p => new VideoStatusModel()
                {
                    Name = p.name,
                    Status = p.state,
                    ProcessingProgress = p.processingProgress
                }));
            }
            return result;
        }

        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task AddVideoJob(VideoJobModel videoJobModel, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            if (!await this.VideoService.IsVideoOwnerAsync(videoJobModel.VideoId, userObjectId, cancellationToken))
                throw new Exception("You are not an owner of this video");
            await this.VideoService.AddVideoJobAsync(videoJobModel, cancellationToken: cancellationToken);
        }


        [HttpGet("[action]")]
        //[Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<PersonModel[]> GetPersons(CancellationToken cancellationToken)
        {
            var personsList = await this.VideoService.GetPersistedPersonsAsync(cancellationToken);
            var result = personsList.Select(p => this.Mapper.Map<Person, PersonModel>(p)).ToArray();
            return result;
        }
    }
}
