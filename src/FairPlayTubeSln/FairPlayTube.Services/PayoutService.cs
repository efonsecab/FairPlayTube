using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Common.Localization;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Payouts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Interceptors;
using PTI.Microservices.Library.PayPal.Models.CreateBatchPayout;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class PayoutService
    {
        private PaypalService PaypalService { get; }
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }
        private IStringLocalizer<PayoutService> Localizer { get; set; }

        public PayoutService(PaypalService paypalService,
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider, 
            IStringLocalizer<PayoutService> localizer)
        {
            this.PaypalService = paypalService;
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
            Localizer = localizer;
        }

        public async Task<PaypalPayoutBatch> SendVideoJobPaymentAsync(long videoJobId, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var user = await FairplaytubeDatabaseContext.ApplicationUser
                .SingleAsync(p => p.AzureAdB2cobjectId.ToString() == userObjectId, cancellationToken: cancellationToken);
            var videoJobEntity = await this.FairplaytubeDatabaseContext.VideoJob
                .Include(p => p.VideoInfo)
                .Include(p => p.VideoJobEscrow)
                .SingleOrDefaultAsync(p => p.VideoJobId == videoJobId, cancellationToken);
            if (videoJobEntity is null)
                throw new CustomValidationException(Localizer[JobNotFoundTextKey]);
            if (user.ApplicationUserId != videoJobEntity.VideoInfo.ApplicationUserId)
                throw new CustomValidationException(Localizer[NotVideoOwnerTextKey]);
            var acceptedApplication = await this.FairplaytubeDatabaseContext
                .VideoJobApplication.Include(p => p.ApplicantApplicationUser)
                .Where(p => p.VideoJobApplicationStatusId == (short)Common.Global.Enums.VideoJobApplicationStatus.Selected)
                .SingleAsync(cancellationToken: cancellationToken);
            var userPaypalEmailAddress = acceptedApplication.ApplicantApplicationUser.EmailAddress;
            string detailedMessage = $"You have been paid for your work on the FairPlayTube Platform, " +
                        $"specifically on the video titled : {videoJobEntity.VideoInfo.Name}." +
                        $"Job Title: {videoJobEntity.Title}" +
                        $"Job Description: {videoJobEntity.Description}" +
                        $"Amoung Paid: {videoJobEntity.Budget}";
            CreateBatchPayoutRequest createBatchPayoutRequest =
                new()
                {
                    sender_batch_header = new Sender_Batch_Header() { email_message = detailedMessage, email_subject = $"You have been paid on FairPlayTube", sender_batch_id = Guid.NewGuid().ToString() },
                    items = new Item[] { new Item() { amount = new Amount() { currency = "USD", 
                        value = videoJobEntity.Budget.ToString("0.00") }, note = $"Thank You for using FairPlayTube!. Please keep the great work. " + $"{detailedMessage}", notification_language = "en-US", receiver = userPaypalEmailAddress, recipient_type = "EMAIL", sender_item_id = Guid.NewGuid().ToString() } }
                };
            var accessTokenResponse = await this.PaypalService.GetAccessTokenAsync(null, cancellationToken);
            var response = await this.PaypalService.CreateBatchPayoutAsync(createBatchPayoutRequest,
                accessTokenResponse.access_token, cancellationToken);
            PaypalPayoutBatch paypalPayoutBatch = new()
            {
                PayoutBatchId = response.batch_header.payout_batch_id,
                EmailMessage = createBatchPayoutRequest.sender_batch_header.email_message,
                EmailSubject = createBatchPayoutRequest.sender_batch_header.email_subject,
                SenderBatchId = createBatchPayoutRequest.sender_batch_header.sender_batch_id,
                PaypalPayoutBatchItem = createBatchPayoutRequest.items.Select(p => new PaypalPayoutBatchItem() { AmountCurrency = p.amount.currency, AmountValue = Convert.ToDecimal(p.amount.value), Note = p.note, NotificationLanguage = p.notification_language, RecipientType = p.recipient_type, ReceiverEmailAddress = p.receiver, SenderItemId = p.sender_item_id }).ToArray()
            };
            await FairplaytubeDatabaseContext.PaypalPayoutBatch.AddAsync(paypalPayoutBatch, cancellationToken);
            acceptedApplication.VideoJobApplicationStatusId = (short)Common.Global.Enums.VideoJobApplicationStatus.Paid;
            videoJobEntity.VideoJobEscrow.PaypalPayoutBatchItemId = paypalPayoutBatch.PaypalPayoutBatchItem.Single().PaypalPayoutBatchItemId;
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
            return paypalPayoutBatch;
        }


        #region Resource Keys
        [ResourceKey(defaultValue: "Unable to find the specified job")]
        public const string JobNotFoundTextKey = "JobNotFoundText";
        [ResourceKey(defaultValue: "Access denied. User is not the video owner")]
        public const string NotVideoOwnerTextKey = "NotVideoOwnerText";
        #endregion Resource Keys
    }
}
