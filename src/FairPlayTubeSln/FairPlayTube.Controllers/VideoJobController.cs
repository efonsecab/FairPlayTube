using AutoMapper;
using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Global.Enums;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all the data related to videos jobs
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [FeatureGate(FeatureType.VideoJobSystem)]
    public class VideoJobController : ControllerBase
    {
        private VideoJobService VideoJobService { get; }

        private VideoService VideoService { get; }

        private ICurrentUserProvider CurrentUserProvider { get; }
        private IMapper Mapper { get; }

        /// <summary>
        /// Initializes <see cref="VideoJobService"/>
        /// </summary>
        /// <param name="videoJobService"></param>
        /// <param name="videoService"></param>
        /// <param name="currentUserProvider"></param>
        /// <param name="mapper"></param>
        public VideoJobController(VideoJobService videoJobService,
            VideoService videoService,
            ICurrentUserProvider currentUserProvider, IMapper mapper)
        {
            this.VideoJobService = videoJobService;
            this.VideoService = videoService;
            this.CurrentUserProvider = currentUserProvider;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Adds a job associatd to a given video
        /// </summary>
        /// <param name="videoJobModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
        public async Task AddVideoJob(VideoJobModel videoJobModel, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            if (!await this.VideoService.IsVideoOwnerAsync(videoJobModel.VideoId, userObjectId, cancellationToken))
                throw new CustomValidationException("You are not an owner of this video");
            await this.VideoJobService.AddVideoJobAsync(videoJobModel, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Retrieve a list of available jobs
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableVideosJobs(CancellationToken cancellationToken)
        {
            var result = await this.VideoJobService.GetAvailableVideosJobs()
                .OrderByDescending(p => p.RowCreationDateTime)
                .Select(p => this.Mapper.Map<VideoJob, VideoJobModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves the jobs available for the specified video
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetVideoJobs(string videoId, CancellationToken cancellationToken)
        {
            var result = await this.VideoJobService.GetVideoJobs(videoId)
                .OrderByDescending(p => p.RowCreationDateTime)
                .Select(p => this.Mapper.Map<VideoJob, VideoJobModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            return Ok(result);
        }
    }
}
