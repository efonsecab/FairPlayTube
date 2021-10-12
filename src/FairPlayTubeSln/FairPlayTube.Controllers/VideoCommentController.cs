using AutoMapper;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using FairPlayTube.Models.VideoComment;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all of the data regarding a VideoComment
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VideoCommentController : ControllerBase
    {
        private VideoCommentService VideoCommentService { get; }
        private IMapper Mapper { get; }

        /// <summary>
        /// Initializes <see cref="VideoCommentController"/>
        /// </summary>
        /// <param name="videoCommentService"></param>
        /// <param name="mapper"></param>
        public VideoCommentController(VideoCommentService videoCommentService,
            IMapper mapper)
        {
            this.VideoCommentService = videoCommentService;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Creates a new comment for a given video id
        /// </summary>
        /// <param name="createVideoCommentModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> AddVideoComment(CreateVideoCommentModel createVideoCommentModel,
            CancellationToken cancellationToken)
        {
            await this.VideoCommentService.AddVideoCommentAsync(createVideoCommentModel, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Gets the comments for a given videoId
        /// </summary>
        /// <param name="videoId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<VideoCommentModel[]> GetVideoComments(string videoId, CancellationToken cancellationToken)
        {
            var result = await this.VideoCommentService.GetVideoComments(videoId)
                .Select(p => this.Mapper.Map<VideoComment, VideoCommentModel>(p))
                .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
