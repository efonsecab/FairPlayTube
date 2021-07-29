using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.Models.UserProfile;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all the data regarind a USer's Profile
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }
        private PaymentService PaymentService { get; }

        /// <summary>
        /// Initializes <see cref="UserProfileController"/>
        /// </summary>
        /// <param name="fairplaytubeDatabaseContext"></param>
        /// <param name="currentUserProvider"></param>
        /// <param name="paymentService"></param>
        public UserProfileController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider, PaymentService paymentService)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
            this.PaymentService = paymentService;
        }

        /// <summary>
        /// Saves the Monetization Profile
        /// </summary>
        /// <param name="globalMonetizationModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        public async Task SaveMonetization(GlobalMonetizationModel globalMonetizationModel,
            CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var user = await this.FairplaytubeDatabaseContext.ApplicationUser.Include(p => p.UserExternalMonetization)
                .Where(p => p.AzureAdB2cobjectId.ToString() == userObjectId)
                .SingleAsync();
            if (user.UserExternalMonetization.Count > 0)
            {
                var userMonetizationItems = user.UserExternalMonetization;
                foreach (var singleItem in userMonetizationItems)
                {
                    this.FairplaytubeDatabaseContext.UserExternalMonetization.Remove(singleItem);
                }
                await this.FairplaytubeDatabaseContext.SaveChangesAsync();
            }
            if (globalMonetizationModel.MonetizationItems.Count > 0)
            {
                foreach (var singleItem in globalMonetizationModel.MonetizationItems)
                {
                    await this.FairplaytubeDatabaseContext.UserExternalMonetization.AddAsync(
                        new DataAccess.Models.UserExternalMonetization()
                        {
                            ApplicationUserId = user.ApplicationUserId,
                            MonetizationUrl = singleItem.MonetizationUrl
                        });
                }
                await this.FairplaytubeDatabaseContext.SaveChangesAsync();
            }

        }

        /// <summary>
        /// Gets the Logged In User Monetization Profile
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<GlobalMonetizationModel> GetMyMonetizationInfo(CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var user = await this.FairplaytubeDatabaseContext.ApplicationUser.Include(p => p.UserExternalMonetization)
                .Where(p => p.AzureAdB2cobjectId.ToString() == userObjectId)
                .SingleAsync();
            if (user.UserExternalMonetization.Count > 0)
                return new GlobalMonetizationModel()
                {
                    MonetizationItems = user.UserExternalMonetization.Select(p => new MonetizationItem()
                    {
                        MonetizationUrl = p.MonetizationUrl
                    }).ToList()
                };
            else
                return new GlobalMonetizationModel()
                {
                    MonetizationItems = new List<MonetizationItem>()
                    {

                    }
                };
        }

        /// <summary>
        /// Verifies if the Paypal order id is valid, and adds fund to the user's system wallet
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task AddFunds(string orderId, CancellationToken cancellationToken)
        {
            var azureAdB2CObjectId = this.CurrentUserProvider.GetObjectId();
            await this.PaymentService.AddFundsAsync(azureAdB2CObjectId: azureAdB2CObjectId, orderId, cancellationToken);
        }

        /// <summary>
        /// Gets the available funds for the Logged In user
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Authorize(Roles = Common.Global.Constants.Roles.User)]
        public async Task<decimal> GetMyFunds(CancellationToken cancellationToken)
        {
            var azureAdB2CObjectId = this.CurrentUserProvider.GetObjectId();
            var result = await this.FairplaytubeDatabaseContext.ApplicationUser
                .Where(p => p.AzureAdB2cobjectId.ToString() == azureAdB2CObjectId)
                .Select(p => p.AvailableFunds)
                .SingleAsync();
            return result;
        }
    }
}
