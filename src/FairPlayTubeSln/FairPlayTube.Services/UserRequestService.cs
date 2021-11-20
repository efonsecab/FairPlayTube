﻿using FairPlayTube.Common.CustomExceptions;
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

        public UserRequestService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            IStringLocalizer<UserRequestService> localizer, ICurrentUserProvider currentUserProvider)
        {
            FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            Localizer = localizer;
            CurrentUserProvider = currentUserProvider;
        }

        public async Task AddUserRequestAsync(CreateUserRequestModel createUserRequestModel,
            CancellationToken cancellationToken)
        {
            long? applicationUserId = null;
            if (this.CurrentUserProvider.IsLoggedIn())
            {
                var userObjectId = this.CurrentUserProvider.GetObjectId();
                var userEntity = await this.FairplaytubeDatabaseContext
                    .ApplicationUser.SingleAsync(p => p.AzureAdB2cobjectId.ToString() == userObjectId,
                    cancellationToken);
                if (userEntity is null)
                    throw new CustomValidationException(Localizer[UserNotFoundTextKey]);
                applicationUserId = userEntity.ApplicationUserId;
            }
            UserRequest userRequestEntity = new()
            {
                Description = createUserRequestModel.Description,
                UserRequestTypeId = (short)createUserRequestModel.UserRequestType,
                ApplicationUserId = applicationUserId
            };
            await FairplaytubeDatabaseContext.UserRequest.AddAsync(userRequestEntity, cancellationToken);
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"User not found")]
        private const string UserNotFoundTextKey = "UserNotFoundText";
        #endregion Resource Keys
    }
}
