using AutoMapper;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.VisitorTracking;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VisitorTrackingController: ControllerBase
    {
        private VisitorTrackingService VisitorTrackingService { get; }
        public VisitorTrackingController(VisitorTrackingService visitorTrackingService)
        {
            this.VisitorTrackingService = visitorTrackingService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> TrackClientInformation(VisitorTrackingModel visitorTrackingModel)
        {
            await this.VisitorTrackingService.TrackVisit(visitorTrackingModel);
            return Ok();
        }
    }
}
