using FairPlayTube.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Interceptors;
using PTI.Microservices.Library.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class PaymentService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private PaypalService PaypalService { get; }
        public ILogger<CustomHttpClientHandler> CustomHttpClientHandlerLogger { get; }

        public PaymentService(PaypalService paypalService, FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ILogger<CustomHttpClientHandler> customHttpClientHandlerLogger)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.PaypalService = paypalService;
            this.CustomHttpClientHandlerLogger = customHttpClientHandlerLogger;
        }

        public async Task AddFundsAsync(string azureAdB2CObjectId, string orderId, CancellationToken cancellationToken = default)
        {
            var paypalAccessTokenResult = await this.PaypalService.GetAccessTokenAsync(CustomHttpClientHandlerLogger, cancellationToken);
            var paypalOrder = await this.PaypalService.GetOrderDetailsAsync(orderId, paypalAccessTokenResult.access_token, cancellationToken);
            if (paypalOrder.id != orderId)
                throw new Exception($"Invalid Order: {orderId}");
            var transactionEntity = await this.FairplaytubeDatabaseContext.PaypalTransaction.SingleOrDefaultAsync(p => p.OrderId == orderId);
            if (transactionEntity != null)
                throw new Exception($"Funds have already been added for Order: {orderId}");

            var userEntity = await this.FairplaytubeDatabaseContext.ApplicationUser.SingleAsync(p => p.AzureAdB2cobjectId.ToString() == azureAdB2CObjectId);
            decimal orderAmount = Convert.ToDecimal(paypalOrder.gross_total_amount.value);
            userEntity.AvailableFunds += orderAmount;
            await this.FairplaytubeDatabaseContext.PaypalTransaction.AddAsync(new DataAccess.Models.PaypalTransaction()
            {
                ApplicationUserId = userEntity.ApplicationUserId,
                OrderAmount = orderAmount,
                OrderId = paypalOrder.id
            });
            await this.FairplaytubeDatabaseContext.SaveChangesAsync();
        }
    }
}
