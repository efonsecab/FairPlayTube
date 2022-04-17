using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Global;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.Models.UserMessage;
using FairPlayTube.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PTI.Microservices.Library.Services;
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
        private ContentModerationService ContentModerationService { get; }

        private readonly EmailService EmailService;
        private readonly IConfiguration Configuration;

        public MessageService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            IHubContext<NotificationHub, INotificationHub> hubContext,
            ContentModerationService contentModerationService,
            EmailService emailService, IConfiguration configuration)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.HubContext = hubContext;
            this.ContentModerationService = contentModerationService;
            this.EmailService = emailService;
            this.Configuration = configuration;
        }

        public async Task SendMessageAsync(UserMessageModel model, string senderObjectId, CancellationToken cancellationToken)
        {
            var sender = await this.FairplaytubeDatabaseContext.ApplicationUser
                .Include(p=>p.ApplicationUserRole).ThenInclude(p=>p.ApplicationRole)
                            .Where(u => u.AzureAdB2cobjectId.ToString() == senderObjectId)
                            .SingleAsync(cancellationToken);
            var lastMonthDate = DateTimeOffset.UtcNow.AddMonths(-1);
            var totalMessagesSentLast30Days =
                await this.FairplaytubeDatabaseContext.UserMessage
                .Where(p => p.FromApplicationUserId == sender.ApplicationUserId &&
                p.RowCreationDateTime >= lastMonthDate)
                .CountAsync(cancellationToken: cancellationToken);
            if (totalMessagesSentLast30Days > Constants.ActionsLimits.MaxMonthlySentMessages &&
                sender.ApplicationUserRole.Any(p=>p.ApplicationRole.Name == Common.Global.Constants.Roles.Admin)==false)
                throw new CustomValidationException($"Monthly messages limit reached. " +
                    $"You cannot send more than {Constants.ActionsLimits.MaxMonthlySentMessages} per month. " +
                    $"You have sent: {totalMessagesSentLast30Days}");
            await this.ContentModerationService.CheckMessageModerationAsync(messageText: model.Message);

            var receiver = await this.FairplaytubeDatabaseContext.ApplicationUser
                .Where(u => u.ApplicationUserId == model.ToApplicationUserId)
                .SingleOrDefaultAsync(cancellationToken);
            if (receiver == null)
                throw new CustomValidationException($"Specified user {model.ToApplicationUserId} does not exist");

            await this.FairplaytubeDatabaseContext.UserMessage.AddAsync(
                new DataAccess.Models.UserMessage()
                {
                    ToApplicationUserId = model.ToApplicationUserId,
                    Message = model.Message,
                    FromApplicationUserId = sender.ApplicationUserId
                }, cancellationToken);

            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);

            await this.HubContext.Clients.User(receiver.AzureAdB2cobjectId.ToString())
                .ReceiveMessage(new Models.Notifications.NotificationModel()
                {
                    Message = $"You have a new message from: {sender.FullName}. Message: {model.Message}"
                });
            StringBuilder htmlMessage = new StringBuilder();
            htmlMessage.AppendLine("<p>");
            htmlMessage.AppendLine($"{sender.FullName} has sent you a message in FairPlayTube.");
            htmlMessage.AppendLine("</p>");

            htmlMessage.AppendLine("<p>");
            htmlMessage.AppendLine(model.Message);
            htmlMessage.AppendLine("</p>");

            htmlMessage.AppendLine("<p>");
            var host = Configuration[Constants.ConfigurationKeysNames.VideoIndexerCallbackUrl];
            string conversationsLink =
                $"{host}{Constants.UserPagesRoutes.MyConversations}";
            htmlMessage.AppendLine($"This is an automated message. " +
                $"Please do not reply to this email. ");
            htmlMessage.AppendLine($"To reply the user who sent you a message go here: " +
                $"<a href=\"{conversationsLink}\">My Conversations</a>");
            htmlMessage.AppendLine("</p>");
            await EmailService.SendEmailAsync(toEmailAddress: receiver.EmailAddress,
                subject: "You have a new message in FairPlayTube",
                body: htmlMessage.ToString(),
                isBodyHtml: true, cancellationToken);
        }
    }
}
