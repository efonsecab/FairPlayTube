using AutoMapper;
using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Global;
using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Pagination;
using FairPlayTube.Models.Persons;
using FairPlayTube.Models.Video;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all of the data related to a video
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoController : ControllerBase
    {
        private VideoService VideoService { get; }
        private IMapper Mapper { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }
        private VideoCommentService VideoCommentService { get; }

        /// <summary>
        /// Initializes <see cref="VideoController"/>
        /// </summary>
        /// <param name="videoService"></param>
        /// <param name="videoCommentService"></param>
        /// <param name="mapper"></param>
        /// <param name="currentUserProvider"></param>
        public VideoController(VideoService videoService, VideoCommentService videoCommentService,
            IMapper mapper, ICurrentUserProvider currentUserProvider)
        {
            this.VideoService = videoService;
            this.Mapper = mapper;
            this.CurrentUserProvider = currentUserProvider;
            this.VideoCommentService = videoCommentService;
        }

        /// <summary>
        /// Allows to delete a video
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<ActionResult> DeleteVideo(string videoId, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();

            bool isVideoOwner = await VideoService.IsVideoOwnerAsync(videoId: videoId, azureAdB2cobjectId: userObjectId,
                cancellationToken: cancellationToken);
            if (!isVideoOwner)
                throw new CustomValidationException($"Delete denied. You are not an owner of this video");

            if (await VideoService.DeleteVideoAsync(userAzureAdB2cObjectId: userObjectId, videoId: videoId, cancellationToken))
                return Ok();
            else
                throw new CustomValidationException("An error occurred trying to delete your video");

        }

        /// <summary>
        /// Gets all of the public processed videos
        /// </summary>
        /// <param name="pageRequestModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<PagedItems<VideoInfoModel>> GetPublicProcessedVideos([FromQuery] PageRequestModel pageRequestModel,
            CancellationToken cancellationToken)
        {
            var query = this.VideoService.GetPublicProcessedVideos();
            var totalItems = query.Count();
            var itemsToSkip = (pageRequestModel.PageNumber - 1) * Constants.Paging.DefaultPageSize;
            var items = await query
                .Skip(itemsToSkip)
                .Take(Constants.Paging.DefaultPageSize)
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            return new PagedItems<VideoInfoModel>()
            {
                Items = items,
                PageNumber = pageRequestModel.PageNumber,
                PageSize = Constants.Paging.DefaultPageSize,
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling((decimal)totalItems / Constants.Paging.DefaultPageSize),
            };
        }

        /// <summary>
        /// Gets all of the videosids for the bought videos
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<string[]> GetBoughtVideosIds(CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var result = await this.VideoService.GetBoughtVideosIdsAsync(userObjectId, cancellationToken);
            return result;
        }

        /// <summary>
        /// Gets videos by person
        /// </summary>
        /// <param name="personName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<VideoInfoModel[]> SearchVideosByPersonName(string personName, CancellationToken cancellationToken)
        {
            var result = await this.VideoService.SearchVideosByPersonNameAsync(personName, cancellationToken);
            return result.Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).ToArray();
        }

        /// <summary>
        /// Uploads a video 
        /// </summary>
        /// <param name="uploadVideoModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(1073741824)] //1GB
        public async Task<IActionResult> UploadVideo(UploadVideoModel uploadVideoModel, CancellationToken cancellationToken)
        {
            if (uploadVideoModel.UseSourceUrl && String.IsNullOrWhiteSpace(uploadVideoModel.SourceUrl))
                throw new CustomValidationException("You muse specify a Source Url");
            if (!uploadVideoModel.UseSourceUrl && String.IsNullOrWhiteSpace(uploadVideoModel.StoredFileName))
                throw new CustomValidationException("You muse upload a file");
            if (await this.VideoService.HasReachedWeeklyVideoUploadLimitAsync(cancellationToken:cancellationToken))
            {
                throw new CustomValidationException("You are not allowed to upload videos. You have reached your subscription's weekly limit");
            }
            if (await this.VideoService.UploadVideoAsync(uploadVideoModel, cancellationToken: cancellationToken))
                return Ok();
            else
                throw new CustomValidationException("An error occurred trying to upload your video");
        }

        /// <summary>
        /// Gets the Logged In user processed videos
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = $"{Common.Global.Constants.Roles.User},{Common.Global.Constants.Roles.Creator}")]
        public async Task<VideoInfoModel[]> GetMyProcessedVideos(CancellationToken cancellationToken)
        {
            var azureAdB2cobjectId = this.CurrentUserProvider.GetObjectId();
            var result = await this.VideoService.GetProcessedVideosByUserId(azureAdB2cobjectId)
                .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p)).ToArrayAsync(cancellationToken: cancellationToken);
            return result;

        }

        /// <summary>
        /// Gets a given video access token to enable edit mode in the insights widget
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<string> GetVideoEditAccessToken(string videoId, CancellationToken cancellationToken)
        {
            var azureAdB2cobjectId = this.CurrentUserProvider.GetObjectId();
            bool isVideoOwner = await VideoService.IsVideoOwnerAsync(videoId: videoId, azureAdB2cobjectId: azureAdB2cobjectId,
                cancellationToken: cancellationToken);
            if (!isVideoOwner)
                throw new CustomValidationException("You are not allowed to edit this video");
            string accessToken = await this.VideoService.GetVideoEditAccessTokenAsync(videoId: videoId);
            return accessToken;
        }

        /// <summary>
        /// List all keywords found on the processed videos
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<GlobalKeywordModel[]> ListAllKeywords(CancellationToken cancellationToken)
        {
            return await this.VideoService.ListAllKeywordsAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Lists all the videos having a given keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Updates a video
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="model"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
        public async Task<IActionResult> UpdateMyVideo(string videoId, UpdateVideoModel model,
            CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();

            if (!await this.VideoService.IsVideoOwnerAsync(videoId, userObjectId, cancellationToken))
                throw new CustomValidationException($"User {userObjectId} is not allowed to modify Video: {videoId}");
            await this.VideoService.UpdateVideoAsync(videoId, model);
            return Ok();
        }

        /// <summary>
        /// Gets the information for a given video
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<VideoInfoModel> GetVideo(string videoId, CancellationToken cancellationToken)
        {
            return await this.VideoService.GetVideo(videoId).Select(
                p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p))
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Buys access to a given video
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task BuyVideoAccess(string videoId, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            await VideoService.BuyVideoAccessAsync(azureAdB2CObjectId: userObjectId, videoId: videoId, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Gets the status of the logged in user queued videos
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
        public async Task<List<VideoStatusModel>> GetMyPendingVideosQueue(CancellationToken cancellationToken)
        {
            List<VideoStatusModel> result = new();
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
                var processingVideosStatuses = await VideoService.GetVideoIndexerStatusAsync(processingVideosIds, cancellationToken);
                result.AddRange(processingVideosStatuses.results.Select(p => new VideoStatusModel()
                {
                    Name = p.name,
                    Status = p.state,
                    ProcessingProgress = p.processingProgress
                }));
            }
            return result;
        }

        /// <summary>
        /// Gets the persons found in the videos
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        //[Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<PersonModel[]> GetPersons(CancellationToken cancellationToken)
        {
            var personsList = await this.VideoService.GetPersistedPersonsAsync(cancellationToken);
            var result = personsList.Select(p => this.Mapper.Map<Person, PersonModel>(p)).ToArray();
            return result;
        }

        /// <summary>
        /// Analyzes given video's comment to generate additional insights
        /// </summary>
        /// <param name="videoCommentId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        [FeatureGate(FeatureType.PaidFeature)]
        public async Task<IActionResult> AnalyzeVideoComment(long videoCommentId, CancellationToken cancellationToken)
        {
            await this.VideoCommentService.AnalyzeVideoCommentAsync(videoCommentId, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Creates a new custom rendering project
        /// </summary>
        /// <param name="projectModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        [FeatureGate(FeatureType.PaidFeature)]
        public async Task<ProjectModel> CreateCustomRenderingProject(ProjectModel projectModel, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var allVideoIds = projectModel.Videos.Select(p => p.VideoId).ToArray();
            var isVideosOwner = await this.VideoService
                .IsVideosOwnerAsync(videosIds: allVideoIds, azureAdB2cobjectId: userObjectId, cancellationToken: cancellationToken);
            if (!isVideosOwner)
                throw new CustomValidationException("Access denied. User does not own all of the specified videos");
            var result = await this.VideoService.CreateCustomRenderingProjectAsync(projectModel, cancellationToken);
            return result;
        }

        /// <summary>
        /// Download the source file for the specified video id
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        [FeatureGate(FeatureType.PaidFeature)]
        public async Task<DownloadVideoModel> DownloadVideo(string videoId, CancellationToken cancellationToken)
        {
            var videoBytes = await this.VideoService.DownloadVideoAsync(videoId, cancellationToken);
            return new DownloadVideoModel()
            {
                VideoId = videoId,
                VideoBytes = videoBytes
            };
        }
    }
}
