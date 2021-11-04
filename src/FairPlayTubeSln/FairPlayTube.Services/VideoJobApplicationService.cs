using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.VideoJobApplications;
using Microsoft.EntityFrameworkCore;
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

        public VideoJobApplicationService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
        }

        public async Task AddVideoJobApplicationAsync(CreateVideoJobApplicationModel createVideoJobApplicationModel, CancellationToken cancellationToken)
        {
            var userObjectId = CurrentUserProvider.GetObjectId();
            var videoJobApplicationEntity = await FairplaytubeDatabaseContext.VideoJobApplication
                .Include(p => p.ApplicantApplicationUser)
                .SingleOrDefaultAsync(p => p.VideoJobId == createVideoJobApplicationModel.VideoJobId &&
                p.ApplicantApplicationUser.AzureAdB2cobjectId.ToString() == userObjectId);
            if (videoJobApplicationEntity is not null)
                throw new Exception($"User already has an application for the specified video job");
            videoJobApplicationEntity = new VideoJobApplication()
            {
                ApplicantCoverLetter = createVideoJobApplicationModel.ApplicantCoverLetter,
                VideoJobId = createVideoJobApplicationModel.VideoJobId,
                VideoJobApplicationStatusId = (short)Common.Global.Enums.VideoJobApplicationStatus.New
            };
            await FairplaytubeDatabaseContext.VideoJobApplication.AddAsync(videoJobApplicationEntity, cancellationToken);
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }
    }
}
