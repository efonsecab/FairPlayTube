using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using FairPlayTube.Models.VideoComment;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class VideoCommentService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private TextAnalysisService TextAnalysisService { get; }
        private ContentModerationService ContentModerationService { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }

        public VideoCommentService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            TextAnalysisService textAnalysisService,
            ContentModerationService contentModerationService,
            ICurrentUserProvider currentUserProvider)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.TextAnalysisService = textAnalysisService;
            this.ContentModerationService = contentModerationService;
            this.CurrentUserProvider = currentUserProvider;
        }

        public async Task AnalyzeVideoCommentAsync(long videoCommentId, CancellationToken cancellationToken)
        {
            var videoCommentEntity = await this.FairplaytubeDatabaseContext.VideoComment
                .Where(p => p.VideoCommentId == videoCommentId).SingleOrDefaultAsync();
            if (videoCommentEntity == null)
                throw new Exception($"Video comment id: {videoCommentId} does not exit");
            var videoCommentAnalysisEntity = await this.FairplaytubeDatabaseContext
                .VideoCommentAnalysis.Where(p => p.VideoCommentId == videoCommentId)
                .SingleOrDefaultAsync();
            if (videoCommentAnalysisEntity != null)
                throw new Exception($"Video comment id: {videoCommentId} has already been analyzed");
            var detectedLanguage = await this.TextAnalysisService.DetectLanguageAsync(videoCommentEntity.Comment, cancellationToken);
            var keyPhrases = await this.TextAnalysisService.GetKeyPhrasesAsync(videoCommentEntity.Comment, detectedLanguage, cancellationToken);
            var sentiment = await this.TextAnalysisService.GetSentimentAsync(videoCommentEntity.Comment, detectedLanguage, cancellationToken);
            var joinedKeyPhrases = String.Join(",", keyPhrases);
            await this.FairplaytubeDatabaseContext.VideoCommentAnalysis.AddAsync(new VideoCommentAnalysis()
            {
                VideoCommentId = videoCommentId,
                KeyPhrases = joinedKeyPhrases,
                Sentiment = sentiment,
            }, cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public IQueryable<VideoComment> GetVideoComments(string videoId)
        {
            return this.FairplaytubeDatabaseContext.VideoComment
                .Include(p => p.VideoInfo)
                .Include(p=>p.ApplicationUser)
                .ThenInclude(p=>p.UserFollowerFollowedApplicationUser)
                .Where(p => p.VideoInfo.VideoId == videoId);
        }

        public async Task AddVideoCommentAsync(CreateVideoCommentModel createVideoCommentModel, CancellationToken cancellationToken)
        {
            await this.ContentModerationService.CheckMessageModeration(createVideoCommentModel.Comment);
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .SingleOrDefaultAsync(p => p.VideoId == createVideoCommentModel.VideoId, cancellationToken: cancellationToken);
            if (videoEntity is null)
                throw new Exception($"Unable to find {nameof(createVideoCommentModel.VideoId)}: {createVideoCommentModel.VideoId}");
            var commentedUserObjectId = this.CurrentUserProvider.GetObjectId();
            var commentedApplicationUser = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleAsync(p => p.AzureAdB2cobjectId.ToString() == commentedUserObjectId, cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.VideoComment.AddAsync(new VideoComment()
            {
                Comment = createVideoCommentModel.Comment,
                VideoInfoId = videoEntity.VideoInfoId,
                ApplicationUserId = commentedApplicationUser.ApplicationUserId,
            }, cancellationToken: cancellationToken);
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }
    }
}
