using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Common.Localization;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.VideoJobApplications;
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
    public class VideoJobApplicationService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }

        private ICurrentUserProvider CurrentUserProvider { get; }
        private IStringLocalizer<VideoJobApplicationService> Localizer { get; }

        public VideoJobApplicationService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider, 
            IStringLocalizer<VideoJobApplicationService> localizer)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
            this.Localizer = localizer;
        }

        public async Task AddVideoJobApplicationAsync(CreateVideoJobApplicationModel createVideoJobApplicationModel, CancellationToken cancellationToken)
        {
            var userObjectId = CurrentUserProvider.GetObjectId();
            var userEntity = FairplaytubeDatabaseContext.ApplicationUser
                .Single(p => p.AzureAdB2cobjectId.ToString() == userObjectId);
            var videoJobApplicationEntity = await FairplaytubeDatabaseContext.VideoJobApplication
                .Include(p => p.ApplicantApplicationUser)
                .SingleOrDefaultAsync(p => p.VideoJobId == createVideoJobApplicationModel.VideoJobId &&
                p.ApplicantApplicationUser.AzureAdB2cobjectId.ToString() == userObjectId, 
                cancellationToken: cancellationToken);
            if (videoJobApplicationEntity is not null)
                throw new CustomValidationException(Localizer[UserApplicationAlreadyExistsTextKey]);
            videoJobApplicationEntity = new VideoJobApplication()
            {
                ApplicantApplicationUserId = userEntity.ApplicationUserId,
                ApplicantCoverLetter = createVideoJobApplicationModel.ApplicantCoverLetter,
                VideoJobId = createVideoJobApplicationModel.VideoJobId.Value,
                VideoJobApplicationStatusId = (short)Common.Global.Enums.VideoJobApplicationStatus.New
            };
            await FairplaytubeDatabaseContext.VideoJobApplication.AddAsync(videoJobApplicationEntity, cancellationToken);
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "User already has an application for the specified video job")]
        public const string UserApplicationAlreadyExistsTextKey = "UserApplicationAlreadyExistsTextKey";
        #endregion Resource Keys
    }
}
