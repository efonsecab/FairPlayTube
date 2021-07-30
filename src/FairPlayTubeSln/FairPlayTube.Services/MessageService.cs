using FairPlayTube.DataAccess.Data;
using FairPlayTube.Models.UserMessage;
using FairPlayTube.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class MessageService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private IHubContext<NotificationHub, INotificationHub> HubContext { get; }

        public MessageService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.HubContext = hubContext;
        }

        public async Task SendMessageAsync(UserMessageModel model, string senderObjectId, CancellationToken cancellationToken)
        {
            var sender = await this.FairplaytubeDatabaseContext.ApplicationUser
                            .Where(u => u.AzureAdB2cobjectId.ToString() == senderObjectId)
                            .SingleAsync(cancellationToken);

            var receiver = await this.FairplaytubeDatabaseContext.ApplicationUser
                .Where(u => u.ApplicationUserId == model.ToApplicationUserId)
                .SingleAsync(cancellationToken);

            await this.FairplaytubeDatabaseContext.UserMessage.AddAsync(
                new DataAccess.Models.UserMessage()
                {
                    ToApplicationUserId = model.ToApplicationUserId,
                    Message = model.Message,
                    FromApplicationUserId = sender.ApplicationUserId
                }, cancellationToken);

            await this.FairplaytubeDatabaseContext.SaveChangesAsync();

            await this.HubContext.Clients.User(receiver.AzureAdB2cobjectId.ToString())
                .ReceiveMessage(new Models.Notifications.NotificationModel()
                {
                    Message = $"You have a new message from: {sender.FullName}"
                });
        }
    }
}
