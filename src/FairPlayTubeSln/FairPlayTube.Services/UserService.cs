using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Invites;
using FairPlayTube.Models.UserAudience;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class UserService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }

        public UserService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext, 
            ICurrentUserProvider currentUserProvider)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
        }

        public async Task AddUserFollowerAsync(string followerUserObjectId, string followedUserObjectId, CancellationToken cancellationToken)
        {
            var followerUserEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleOrDefaultAsync(p => p.AzureAdB2cobjectId.ToString() == followerUserObjectId, cancellationToken: cancellationToken);
            if (followerUserEntity is null)
                throw new CustomValidationException($"User {followerUserObjectId} does not exist");

            var followedUserEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleOrDefaultAsync(p => p.AzureAdB2cobjectId.ToString() == followedUserObjectId, cancellationToken: cancellationToken);
            if (followedUserEntity is null)
                throw new CustomValidationException($"User {followedUserObjectId} does not exist");

            await this.FairplaytubeDatabaseContext.UserFollower.AddAsync(new DataAccess.Models.UserFollower() 
            {
                FollowerApplicationUserId = followerUserEntity.ApplicationUserId,
                FollowedApplicationUserId = followedUserEntity.ApplicationUserId
            }, cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<UserInvitation> InviteUserAsync(InviteUserModel inviteUserModel, CancellationToken cancellationToken)
        {
            var userInvitationEntity = await this.FairplaytubeDatabaseContext.UserInvitation
                .Where(p => p.InvitedUserEmail == inviteUserModel.ToEmailAddress).SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (userInvitationEntity is not null)
                throw new CustomValidationException($"An invitation already exists for user: {inviteUserModel.ToEmailAddress}");
            var currentUserObjectId = this.CurrentUserProvider.GetObjectId();
            var currentUserEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .Where(p => p.AzureAdB2cobjectId.ToString() == currentUserObjectId).SingleAsync(cancellationToken: cancellationToken);
            userInvitationEntity = new UserInvitation()
            {
                InviteCode = Guid.NewGuid(),
                InvitedUserEmail = inviteUserModel.ToEmailAddress,
                InvitingApplicationUserId = currentUserEntity.ApplicationUserId,
            };
            await this.FairplaytubeDatabaseContext.UserInvitation.AddAsync(userInvitationEntity, cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return userInvitationEntity;
        }

    }
}
