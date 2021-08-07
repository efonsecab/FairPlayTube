using FairPlayTube.DataAccess.Data;
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
        public UserService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
        }

        public async Task AddUserFollowerAsync(string followerUserObjectId, string followedUserObjectId, CancellationToken cancellationToken)
        {
            var followerUserEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleOrDefaultAsync(p => p.AzureAdB2cobjectId.ToString() == followerUserObjectId);
            if (followerUserEntity is null)
                throw new Exception($"User {followerUserObjectId} does not exist");

            var followedUserEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleOrDefaultAsync(p => p.AzureAdB2cobjectId.ToString() == followedUserObjectId);
            if (followedUserEntity is null)
                throw new Exception($"User {followedUserObjectId} does not exist");

            await this.FairplaytubeDatabaseContext.UserFollower.AddAsync(new DataAccess.Models.UserFollower() 
            {
                FollowerApplicationUserId = followerUserEntity.ApplicationUserId,
                FollowedApplicationUserId = followedUserEntity.ApplicationUserId
            });
            await this.FairplaytubeDatabaseContext.SaveChangesAsync();
        }

    }
}
