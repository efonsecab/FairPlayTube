using FairPlayTube.Models.VideoJobApplications;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class VideoJobApplicationController : ControllerBase
    {
        private VideoJobApplicationService VideoJobApplicationService { get; }
        /// <summary>
        /// Initializes <see cref="VideoJobApplicationController"/>
        /// </summary>
        /// <param name="videoJobApplicationService"></param>
        public VideoJobApplicationController(VideoJobApplicationService videoJobApplicationService)
        {
            this.VideoJobApplicationService = videoJobApplicationService;
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
    }
}
