using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Global;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Common.Localization;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.UsersRequests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class UserRequestService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; set; }
        private IStringLocalizer<UserRequestService> Localizer { get; set; }
        private ICurrentUserProvider CurrentUserProvider { get; set; }
        private EmailService EmailService { get; }

        public UserRequestService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            IStringLocalizer<UserRequestService> localizer,
            ICurrentUserProvider currentUserProvider,
            EmailService emailService)
        {
            FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            Localizer = localizer;
            CurrentUserProvider = currentUserProvider;
            EmailService = emailService;
        }

        public async Task AddAnonymousUserRequestAsync(CreateUserRequestModel createUserRequestModel,
            CancellationToken cancellationToken)
        {
            UserRequest userRequestEntity = new()
            {
                Description = createUserRequestModel.Description,
                UserRequestTypeId = (short)createUserRequestModel.UserRequestType,
                EmailAddress = createUserRequestModel.EmailAddress,
            };
            await FairplaytubeDatabaseContext.UserRequest.AddAsync(userRequestEntity, cancellationToken);
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
            await NotifyNewUserRequestToAllUsersAsync(userRequestEntity, cancellationToken); ;
        }

        private async Task NotifyNewUserRequestToAllUsersAsync(UserRequest userRequestEntity, CancellationToken cancellationToken)
        {
            var allUsers = await FairplaytubeDatabaseContext.ApplicationUser
                .Include(p => p.ApplicationUserRole)
                .ThenInclude(p => p.ApplicationRole)
                .ToListAsync(cancellationToken);
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("<p>There is a new User Request you may be interested in</p>");
            stringBuilder.Append($"<p>Request Type: {(Common.Global.Enums.UserRequestType)userRequestEntity.UserRequestTypeId}</p>");
            stringBuilder.Append($"<p>Description: {userRequestEntity.Description}</p>");
            stringBuilder.Append($"<p>Email Address: {userRequestEntity.EmailAddress}</p>");
            foreach (var singleUser in allUsers)
            {
                await EmailService.SendEmailAsync(
                    toEmailAddress: singleUser.EmailAddress,
                    subject: "New FairPlayTube User Request",
                    body: stringBuilder.ToString(), isBodyHtml: true,
                    cancellationToken: cancellationToken);
            }
        }

        public async Task AddAuthenticatedUserRequestAsync(CreateUserRequestModel createUserRequestModel,
            CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var userEntity = await this.FairplaytubeDatabaseContext
                .ApplicationUser.SingleAsync(p => p.AzureAdB2cobjectId.ToString() == userObjectId,
                cancellationToken);
            if (userEntity is null)
                throw new CustomValidationException(Localizer[UserNotFoundTextKey]);
            UserRequest userRequestEntity = new()
            {
                Description = createUserRequestModel.Description,
                UserRequestTypeId = (short)createUserRequestModel.UserRequestType,
                EmailAddress = createUserRequestModel.EmailAddress,
                ApplicationUserId = userEntity.ApplicationUserId
            };
            await FairplaytubeDatabaseContext.UserRequest.AddAsync(userRequestEntity, cancellationToken);
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "User not found")]
        private const string UserNotFoundTextKey = "UserNotFoundText";
        #endregion Resource Keys
    }
}
