using FairPlayTube.Models.Payouts;
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
    /// Payout Controller
    /// </summary>
    [Authorize]
    [ApiController]
    public class PayoutController : ControllerBase
    {
        private PayoutService PayoutService { get; }
        /// <summary>
        /// Initializes <see cref="PayoutController"/>
        /// </summary>
        public PayoutController(PayoutService payoutService)
        {
            this.PayoutService = payoutService;
        }

        /// <summary>
        /// Sends payout for a video job
        /// </summary>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<IActionResult> SendVideoJobPaymentAsync(long videoJobId, CancellationToken cancellationToken)
        {
            await this.PayoutService.SendVideoJobPaymentAsync(videoJobId, cancellationToken);
            return Ok();
        }
    }
}
