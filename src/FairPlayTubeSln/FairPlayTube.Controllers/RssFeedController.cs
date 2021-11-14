using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Exposes the Rss feeds
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RssFeedController : ControllerBase
    {
        private RssFeedService RssFeedService { get; }

        /// <summary>
        /// Initializes <see cref="RssFeedController"/>
        /// </summary>
        public RssFeedController(RssFeedService rssFeedService)
        {
            this.RssFeedService = rssFeedService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<ContentResult> Videos()
        {
            string host = Request.Scheme + "://" + Request.Host;
            string contentType = "application/xml";
            var feedXml = await this.RssFeedService.GetPublicProcessedVideosRssAsync(host);
            return Content(feedXml, contentType);
        }
    }
}
