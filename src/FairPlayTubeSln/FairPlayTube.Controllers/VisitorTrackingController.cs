using AutoMapper;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.VisitorTracking;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading;
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
        private IMapper Mapper { get; }

        /// <summary>
        /// Initialized <see cref="VisitorTrackingController"/>
        /// </summary>
        /// <param name="visitorTrackingService"></param>
        /// <param name="mapper"></param>
        public VisitorTrackingController(VisitorTrackingService visitorTrackingService,
            IMapper mapper)
        {
            this.VisitorTrackingService = visitorTrackingService;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Persists the visitors information and visited page
        /// </summary>
        /// <param name="visitorTrackingModel"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task<VisitorTrackingModel> TrackAnonymousClientInformation(VisitorTrackingModel visitorTrackingModel)
        {
            var response = await this.VisitorTrackingService.TrackVisit(visitorTrackingModel);
            if (response != null)
            {
                visitorTrackingModel = this.Mapper.Map<VisitorTracking, VisitorTrackingModel>(response);
                return visitorTrackingModel;
            }
            return null;
        }

        /// <summary>
        /// Persists the visitors information and visited page
        /// </summary>
        /// <param name="visitorTrackingModel"></param>
        /// <param name="currentUserProvider"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize]
        public async Task<VisitorTrackingModel> TrackAuthenticatedClientInformation(
            VisitorTrackingModel visitorTrackingModel, [FromServices] ICurrentUserProvider currentUserProvider)
        {
            var userObjectId = currentUserProvider.GetObjectId();
            visitorTrackingModel.UserAzureAdB2cObjectId = userObjectId;
            var response = await this.VisitorTrackingService.TrackVisit(visitorTrackingModel);
            if (response != null)
            {
                visitorTrackingModel = this.Mapper.Map<VisitorTracking, VisitorTrackingModel>(response);
                return visitorTrackingModel;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPut("[action]")]
        public async Task<VisitorTrackingModel> UpdateVisitTimeElapsed(long visitorTrackingId, CancellationToken cancellationToken)
        {
            var response = await VisitorTrackingService.UpdateVisitTimeElapsedAsync(visitorTrackingId, cancellationToken);
            var result = this.Mapper.Map<VisitorTracking, VisitorTrackingModel>(response);
            return result;
        }
    }
}
