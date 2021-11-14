using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.Common.Localization;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.VideoJobApplications;
using FairPlayTube.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;
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
        private IHubContext<NotificationHub, INotificationHub> HubContext { get; }
        private EmailService EmailService { get; set; }

        public VideoJobApplicationService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            ICurrentUserProvider currentUserProvider,
            IStringLocalizer<VideoJobApplicationService> localizer,
            IHubContext<NotificationHub, INotificationHub> hubContext,
            EmailService emailService)
        {
            FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            CurrentUserProvider = currentUserProvider;
            Localizer = localizer;
            HubContext = hubContext;
            EmailService = emailService;
        }

        public async Task AddVideoJobApplicationAsync(CreateVideoJobApplicationModel createVideoJobApplicationModel, CancellationToken cancellationToken)
        {
            var loggedInUserAzureObjectId = CurrentUserProvider.GetObjectId();
            var userEntity = FairplaytubeDatabaseContext.ApplicationUser
                .Single(p => p.AzureAdB2cobjectId.ToString() == loggedInUserAzureObjectId);
            var videoJobApplicationEntity = await FairplaytubeDatabaseContext.VideoJobApplication
                .Include(p => p.ApplicantApplicationUser)
                .Include(p => p.VideoJob)
                .ThenInclude(p => p.VideoInfo)
                .ThenInclude(p => p.ApplicationUser)
                .SingleOrDefaultAsync(p => p.VideoJobId == createVideoJobApplicationModel.VideoJobId &&
                p.ApplicantApplicationUser.AzureAdB2cobjectId.ToString() == loggedInUserAzureObjectId,
                cancellationToken: cancellationToken);
            if (videoJobApplicationEntity is not null)
                throw new CustomValidationException(Localizer[UserApplicationAlreadyExistsTextKey]);
            var videoJobEntity = await FairplaytubeDatabaseContext.VideoJob
                .Include(p=>p.VideoInfo).ThenInclude(p=>p.ApplicationUser)
                .SingleAsync(p => p.VideoJobId == createVideoJobApplicationModel.VideoJobId, 
                cancellationToken: cancellationToken);
            if (videoJobEntity.VideoInfo.ApplicationUser
                .AzureAdB2cobjectId.ToString() == loggedInUserAzureObjectId)
                throw new CustomValidationException(Localizer[CannotApplyToOwnedVideosJobsTextKey]);
            videoJobApplicationEntity = new VideoJobApplication()
            {
                ApplicantApplicationUserId = userEntity.ApplicationUserId,
                ApplicantCoverLetter = createVideoJobApplicationModel.ApplicantCoverLetter,
                VideoJobId = createVideoJobApplicationModel.VideoJobId.Value,
                VideoJobApplicationStatusId = (short)Common.Global.Enums.VideoJobApplicationStatus.New
            };
            await FairplaytubeDatabaseContext.VideoJobApplication.AddAsync(videoJobApplicationEntity, cancellationToken);
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
            string message = String.Format(Localizer[UserHasAppliedToJobTextKey],
                videoJobEntity.VideoInfo.ApplicationUser.FullName, videoJobEntity.Title);
            await HubContext.Clients.User(videoJobEntity.VideoInfo
                .ApplicationUser.AzureAdB2cobjectId.ToString())
                .ReceiveMessage(new Models.Notifications.NotificationModel()
                {
                    Message = message
                });
        }

        public async Task ApproveVideoJobApplicationAsync(long videoJobApplicationId, CancellationToken cancellationToken)
        {
            var loggedInUserAzureObjectId = this.CurrentUserProvider.GetObjectId();
            var videoJobApplication = await this.FairplaytubeDatabaseContext.VideoJobApplication
                .Include(p => p.VideoJob).ThenInclude(p => p.VideoInfo)
                .ThenInclude(p => p.ApplicationUser)
                .Where(p => p.VideoJobApplicationId == videoJobApplicationId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (videoJobApplication is null)
                throw new CustomValidationException(Localizer[VideoJobApplicationNotFoundTextKey]);
            if (loggedInUserAzureObjectId != videoJobApplication.VideoJob
                .VideoInfo.ApplicationUser.AzureAdB2cobjectId.ToString())
                throw new CustomValidationException(Localizer[NotVideoOwnerTextKey]);
            var hasApprovedApplication = await FairplaytubeDatabaseContext.VideoJobApplication
                .AnyAsync(p => p.VideoJobApplicationId == videoJobApplicationId &&
                p.VideoJobApplicationStatusId ==
                (short)Common.Global.Enums.VideoJobApplicationStatus.Selected, 
                cancellationToken: cancellationToken);
            if (hasApprovedApplication)
                throw new CustomValidationException(Localizer[ApprovedApplicationsExistTextKey]);
            videoJobApplication.VideoJobApplicationStatusId =
                (short)Common.Global.Enums.VideoJobApplicationStatus.Selected;
            await FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
            var applicantUserEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleAsync(p => p.ApplicationUserId == 
                videoJobApplication.ApplicantApplicationUserId, 
                cancellationToken: cancellationToken);
            string message = String.Format(Localizer[ApprovedApplicationUserNotificationTextKey],
                    videoJobApplication.VideoJob.Title,
                    videoJobApplication.VideoJob.VideoInfo.Name);
            await HubContext.Clients.User(applicantUserEntity.AzureAdB2cobjectId.ToString())
                .ReceiveMessage(new Models.Notifications.NotificationModel()
                {
                    Message = message,
                });
            await EmailService.SendEmailAsync(toEmailAddress:
                applicantUserEntity.EmailAddress,
                subject: Localizer[ApprovedApplicationEmailSubjectTextKey],
                body: message,
                isBodyHtml: true);
        }

        public IQueryable<VideoJobApplication> GetNewReceivedVideoJobApplications()
        {
            var currentUserObjectId = this.CurrentUserProvider.GetObjectId();
            var receivedApplications = FairplaytubeDatabaseContext.VideoJobApplication
                .Include(p => p.VideoJob)
                .ThenInclude(p => p.VideoInfo)
                .ThenInclude(p => p.ApplicationUser)
                .Where(p => p.VideoJob.VideoInfo.ApplicationUser.AzureAdB2cobjectId.ToString() == currentUserObjectId
                && p.VideoJobApplicationStatusId ==
                (short)Common.Global.Enums.VideoJobApplicationStatus.New);
            return receivedApplications;
        }

        public IQueryable<VideoJobApplication> GetMyVideoJobsApplications()
        {
            var currentUserObjectId = this.CurrentUserProvider.GetObjectId();
            var sentApplications = FairplaytubeDatabaseContext.VideoJobApplication
                .Include(p=>p.ApplicantApplicationUser)
                .Where(p => p.ApplicantApplicationUser.AzureAdB2cobjectId.ToString() == currentUserObjectId);
            return sentApplications;
        }


        #region Resource Keys
        [ResourceKey(defaultValue: "User already has an application for the specified video job")]
        public const string UserApplicationAlreadyExistsTextKey = "UserApplicationAlreadyExistsTextKey";
        [ResourceKey(defaultValue: "{0} has applied to your job titled: {1}")]
        public const string UserHasAppliedToJobTextKey = "UserHasAppliedToJobText";
        [ResourceKey(defaultValue: "Video Job Application was not found")]
        public const string VideoJobApplicationNotFoundTextKey = "VideoJobApplicationNotFoundText";
        [ResourceKey(defaultValue: "Only the video owner can approve received applications")]
        public const string NotVideoOwnerTextKey = "NotVideoOwnerText";
        [ResourceKey(defaultValue: "Job already has an approved application")]
        public const string ApprovedApplicationsExistTextKey = "ApprovedApplicationsExistTextKey";
        [ResourceKey(defaultValue: "Your application has been approved. " +
                    "Title: {0}. Video: {1}")]
        public const string ApprovedApplicationUserNotificationTextKey = "ApprovedApplicationUserNotificationText";
        [ResourceKey(defaultValue: "Your video job application has been approved")]
        public const string ApprovedApplicationEmailSubjectTextKey = "ApprovedApplicationEmailSubjectText";
        [ResourceKey(defaultValue: "Cannot apply to owned videos jobs")]
        public const string CannotApplyToOwnedVideosJobsTextKey = "CannotApplyToOwnedVideosJobsText";
        #endregion Resource Keys
    }
}
