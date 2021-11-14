using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Common.Localization;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class VideoJobService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }
        private IStringLocalizer<VideoJobService> Localizer { get; set; }

        public VideoJobService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider,
            IStringLocalizer<VideoJobService> localizer)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CurrentUserProvider = currentUserProvider;
            this.Localizer = localizer;
        }

        public async Task AddVideoJobAsync(VideoJobModel videoJobModel, CancellationToken cancellationToken)
        {
            if (videoJobModel.Budget <= 0)
                throw new CustomValidationException(Localizer[BudgetMustbeGreaterThan0TextKey]);
            var userObjectId = this.CurrentUserProvider.GetObjectId();
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(p => p.VideoId == videoJobModel.VideoId, cancellationToken: cancellationToken);
            if (videoEntity == null)
                throw new CustomValidationException($"{String.Format(Localizer[VideoWithIdDoesNotExistsTextKey], videoJobModel.VideoId)}");
            var userEntity = videoEntity.ApplicationUser;
            var fundsToDeduct = videoJobModel.Budget + (videoJobModel.Budget * Common.Global.Constants.Commissions.VideoJobComission);
            if (fundsToDeduct > userEntity.AvailableFunds)
                throw new CustomValidationException(String.Format(Localizer[NotEnoughFundsTextKey], fundsToDeduct, userEntity.AvailableFunds));
            VideoJob videoJobEntity = new()
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
            userEntity.AvailableFunds -= fundsToDeduct;
            await this.FairplaytubeDatabaseContext.VideoJob.AddAsync(videoJobEntity, cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }


        public IQueryable<VideoJob> GetAvailableVideosJobs()
        {
            return this.FairplaytubeDatabaseContext.VideoJob.Include(p => p.VideoInfo)
                .Include(p => p.VideoJobApplication)
                .Where(p => p.VideoJobApplication.Any(p => p.VideoJobApplicationStatusId != 1) == false);
        }

        public IQueryable<VideoJob> GetVideoJobs(string videoId)
        {
            return this.FairplaytubeDatabaseContext.VideoJob
                .Include(p => p.VideoInfo)
                .Include(p => p.VideoJobApplication)
                .Where(p => p.VideoInfo.VideoId == videoId);
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Budget must be greater than 0")]
        public const string BudgetMustbeGreaterThan0TextKey = "BudgetMustbeHigherThan0Text";
        [ResourceKey(defaultValue: "Video with id: {0} does not exist")]
        public const string VideoWithIdDoesNotExistsTextKey = "VideoWithIdDoesNotExistsTextKey";
        [ResourceKey(defaultValue: "User does not have enough funds. Funds required: {0}. Funds available: {1}")]
        public const string NotEnoughFundsTextKey = "NotEnoughFundsText";
        #endregion Resource Keys
    }
}
