using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class VideoJobService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }

        public VideoJobService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
        }

        public async Task AddVideoJobAsync(VideoJobModel videoJobModel, CancellationToken cancellationToken)
        {
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(p => p.VideoId == videoJobModel.VideoId, cancellationToken: cancellationToken);
            if (videoEntity == null)
                throw new Exception($"Video with id: {videoJobModel.VideoId} does not exist");
            var userEntity = videoEntity.ApplicationUser;
            var fundsToDeduct = videoJobModel.Budget + (videoJobModel.Budget * Common.Global.Constants.Commissions.VideoJobComission);
            if (fundsToDeduct > userEntity.AvailableFunds)
                throw new Exception($"User does not have enough funds. Funds required: {fundsToDeduct}");
            VideoJob videoJobEntity = new VideoJob()
            {
                Budget = videoJobModel.Budget,
                Title = videoJobModel.Title,
                Description = videoJobModel.Description,
                VideoInfoId = videoEntity.VideoInfoId
            };
            videoJobEntity.VideoJobEscrow = new VideoJobEscrow()
            {
                Amount = fundsToDeduct
            };
            await this.FairplaytubeDatabaseContext.VideoJob.AddAsync(videoJobEntity, cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }


        public IQueryable<VideoJob> GetVideosJobs()
        {
            return this.FairplaytubeDatabaseContext.VideoJob.Include(p => p.VideoInfo);
        }
    }
}
