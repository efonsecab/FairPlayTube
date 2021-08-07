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

        public async Task AddUserFollowerAsync(UserFollowerModel userFollowerModel, CancellationToken cancellationToken)
        {
            var followerUserEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleOrDefaultAsync(p => p.ApplicationUserId == userFollowerModel.FollowerApplicationUserId);
            if (followerUserEntity is null)
                throw new Exception($"User {userFollowerModel.FollowerApplicationUserId} does not exist");

            var followedUserEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleOrDefaultAsync(p => p.ApplicationUserId == userFollowerModel.FollowedApplicationUserId);
            if (followedUserEntity is null)
                throw new Exception($"User {userFollowerModel.FollowedApplicationUserId} does not exist");

            await this.FairplaytubeDatabaseContext.UserFollower.AddAsync(new DataAccess.Models.UserFollower() 
            {
                FollowerApplicationUserId = followerUserEntity.ApplicationUserId,
                FollowedApplicationUserId = followedUserEntity.ApplicationUserId
            });
            await this.FairplaytubeDatabaseContext.SaveChangesAsync();
        }

    }
}
