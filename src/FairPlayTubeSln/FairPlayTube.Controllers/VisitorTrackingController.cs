using FairPlayTube.Common.Interfaces;
using FairPlayTube.Models.VisitorTracking;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Used to persis a visitor informaation
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorTrackingController : ControllerBase
    {
        private VisitorTrackingService VisitorTrackingService { get; }
        /// <summary>
        /// Initialized <see cref="VisitorTrackingController"/>
        /// </summary>
        /// <param name="visitorTrackingService"></param>
        public VisitorTrackingController(VisitorTrackingService visitorTrackingService)
        {
            this.VisitorTrackingService = visitorTrackingService;
        }

        /// <summary>
        /// Persists the visitors information and visited page
        /// </summary>
        /// <param name="visitorTrackingModel"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> TrackAnonymousClientInformation(VisitorTrackingModel visitorTrackingModel)
        {
            await this.VisitorTrackingService.TrackVisit(visitorTrackingModel);
            return Ok();
        }

        /// <summary>
        /// Persists the visitors information and visited page
        /// </summary>
        /// <param name="visitorTrackingModel"></param>
        /// <param name="currentUserProvider"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize]
        public async Task<IActionResult> TrackAuthenticatedClientInformation(
            VisitorTrackingModel visitorTrackingModel, [FromServices] ICurrentUserProvider currentUserProvider)
        {
            var userObjectId = currentUserProvider.GetObjectId();
            visitorTrackingModel.UserAzureAdB2cObjectId = userObjectId;
            await this.VisitorTrackingService.TrackVisit(visitorTrackingModel);
            return Ok();
        }
    }
}
