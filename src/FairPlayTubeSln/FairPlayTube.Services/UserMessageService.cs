using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class UserMessageService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; set; }

        private ICurrentUserProvider CurrentUserProvider;

        public UserMessageService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
        }

        public async Task<IQueryable<UserMessage>> GetMyReceivedMessagesFromUserAsync(
            long otherUserApplicationUserId, CancellationToken cancellationToken)
        {
            var currentUser = await 
                FairplaytubeDatabaseContext.ApplicationUser
                .SingleAsync(p=>p.AzureAdB2cobjectId.ToString() == 
                this.CurrentUserProvider.GetObjectId(), cancellationToken: cancellationToken);
            return FairplaytubeDatabaseContext.UserMessage
                .Include(p => p.ToApplicationUser)
                .Where(p=>
                (p.FromApplicationUserId == currentUser.ApplicationUserId &&
                p.ToApplicationUserId == otherUserApplicationUserId) 
                ||
                (p.FromApplicationUserId == otherUserApplicationUserId &&
                p.ToApplicationUserId == currentUser.ApplicationUserId)
                )
                .OrderByDescending(p=>p.RowCreationDateTime);
        }
    }
}
