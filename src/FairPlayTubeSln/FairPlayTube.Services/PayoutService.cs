using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Payouts;
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
        private ILogger<CustomHttpClientHandler> CustomHttpClientHandleerLogger { get; }

        public PayoutService(PaypalService paypalService,
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ILogger<CustomHttpClientHandler> customHttpClientHandleerLogger)
        {
            this.PaypalService = paypalService;
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CustomHttpClientHandleerLogger = customHttpClientHandleerLogger;
        }

        public async Task<PaypalPayoutBatch> SendPayout(PaypalPayoutBatchModel paypalPayoutBatchModel,
            CancellationToken cancellationToken)
        {
            CreateBatchPayoutRequest createBatchPayoutRequest =
                new CreateBatchPayoutRequest()
                {
                    sender_batch_header =
                    new Sender_Batch_Header()
                    {
                        email_message = paypalPayoutBatchModel.EmailMessage,
                        email_subject = paypalPayoutBatchModel.EmailSubject,
                        sender_batch_id = paypalPayoutBatchModel.SenderBatchId.ToString()
                    },
                    items = paypalPayoutBatchModel.PaypalPayoutBatchItem.Select(p => new Item
                    {
                        amount = new Amount()
                        {
                            currency = p.AmountCurrency,
                            value = p.AmountValue.ToString()
                        },
                        note = p.Note,
                        notification_language = p.NotificationLanguage,
                        receiver = p.ReceiverEmailAddress,
                        recipient_type = p.RecipientType,
                        sender_item_id = p.SenderItemId.ToString()
                    }).ToArray()
                };
            var accessTokenResponse = await this.PaypalService.GetAccessTokenAsync(CustomHttpClientHandleerLogger,
                cancellationToken);
            var response = await this.PaypalService.CreateBatchPayoutAsync(createBatchPayoutRequest,
                accessTokenResponse.access_token,
                cancellationToken);
            PaypalPayoutBatch paypalPayoutBatch = new PaypalPayoutBatch()
            {
                PayoutBatchId = Guid.Parse(response.batch_header.payout_batch_id),
                EmailMessage = paypalPayoutBatchModel.EmailMessage,
                EmailSubject = paypalPayoutBatchModel.EmailSubject,
                SenderBatchId = paypalPayoutBatchModel.SenderBatchId,
                PaypalPayoutBatchItem = paypalPayoutBatchModel.PaypalPayoutBatchItem.
                Select(p => new PaypalPayoutBatchItem()
                {
                    AmountCurrency = p.AmountCurrency,
                    AmountValue = p.AmountValue,
                    Note = p.Note,
                    NotificationLanguage = p.NotificationLanguage,
                    RecipientType = p.RecipientType,
                    ReceiverEmailAddress = p.ReceiverEmailAddress,
                    SenderItemId = p.SenderItemId
                }).ToArray()
            };
            await FairplaytubeDatabaseContext.PaypalPayoutBatch.AddAsync(paypalPayoutBatch, cancellationToken);
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
            return paypalPayoutBatch;
        }

    }
}
