using FairPlayTube.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all of the Open Graph routes
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OpenGraphController: ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; set; }
        private CustomHttpClient CustomHttpClient { get; }

        /// <summary>
        /// Initializes <see cref="OpenGraphController"/>
        /// </summary>
        /// <param name="fairplaytubeDatabaseContext"></param>
        /// <param name="customHttpClient"></param>
        public OpenGraphController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            CustomHttpClient customHttpClient)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CustomHttpClient = customHttpClient;
        }

        /// <summary>
        /// Retrieves the thumbnail image for the specified videoid
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        [HttpGet("[action]/{videoId}")]
        public async Task<FileResult> VideoThumbnail([FromRoute] string videoId)
        {
            var videoInfo = await this.FairplaytubeDatabaseContext.VideoInfo.SingleAsync(p => p.VideoId == videoId);
            var thumbnailStream = await this.CustomHttpClient.GetStreamAsync(videoInfo.ThumbnailUrl);
            return File(thumbnailStream, System.Net.Mime.MediaTypeNames.Image.Jpeg);
        }
    }
}
