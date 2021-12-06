using AutoMapper;
using FairPlayTube.Common.Global.Enums;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.VideoJobApplications;
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
    /// 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [FeatureGate(FeatureType.VideoJobSystem)]
    public class VideoJobApplicationController : ControllerBase
    {
        private VideoJobApplicationService VideoJobApplicationService { get; }
        private IMapper Mapper { get; }

        /// <summary>
        /// Initializes <see cref="VideoJobApplicationController"/>
        /// </summary>
        /// <param name="videoJobApplicationService"></param>
        /// <param name="mapper"></param>
        public VideoJobApplicationController(VideoJobApplicationService videoJobApplicationService,
            IMapper mapper)
        {
            this.VideoJobApplicationService = videoJobApplicationService;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Adda a new Video Job Application
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<IActionResult> AddVideoJobApplication(CreateVideoJobApplicationModel createVideoJobApplicationModel,
            CancellationToken cancellationToken)
        {
            await this.VideoJobApplicationService.AddVideoJobApplicationAsync(createVideoJobApplicationModel,
                cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Retrieves all received video job applications for the logged in user
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
        public async Task<VideoJobApplicationModel[]> GetNewReceivedVideoJobApplications(
            CancellationToken cancellationToken)
        {
            var receivedApplications = await this.VideoJobApplicationService
                .GetNewReceivedVideoJobApplications()
                .Select(p=> this.Mapper.Map<VideoJobApplication, VideoJobApplicationModel>(p))
                .ToArrayAsync(cancellationToken);
            return receivedApplications;
        }

        /// <summary>
        /// Retrieves all video job applications sent by the logged in user
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<VideoJobApplicationModel[]> GetMyVideoJobsApplications(
            CancellationToken cancellationToken)
        {
            var result = await this.VideoJobApplicationService.GetMyVideoJobsApplications()
                .Select(p => this.Mapper.Map<VideoJobApplication, VideoJobApplicationModel>(p))
                .ToArrayAsync(cancellationToken);
            return result;
        }

        /// <summary>
        /// Approves a video job application with the given Id
        /// </summary>
        /// <param name="videoJobApplicationId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.Creator)]
        public async Task<IActionResult> ApproveVideoJobApplication(long videoJobApplicationId,
            CancellationToken cancellationToken)
        {
            await this.VideoJobApplicationService.ApproveVideoJobApplicationAsync(videoJobApplicationId,
                cancellationToken);
            return Ok();
        }
    }
}
