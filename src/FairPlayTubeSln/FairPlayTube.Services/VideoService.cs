using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using FairPlayTube.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Interceptors;
using PTI.Microservices.Library.Models.AzureVideoIndexerService;
using PTI.Microservices.Library.Models.AzureVideoIndexerService.GetVideoIndex;
using PTI.Microservices.Library.Models.AzureVideoIndexerService.SearchVideos;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class VideoService
    {
        private AzureVideoIndexerService AzureVideoIndexerService { get; }
        private AzureBlobStorageService AzureBlobStorageService { get; }
        private DataStorageConfiguration DataStorageConfiguration { get; set; }
        private ICurrentUserProvider CurrentUserProvider { get; set; }
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private AzureVideoIndexerConfiguration AzureVideoIndexerConfiguration { get; }
        private CustomHttpClient CustomHttpClient { get; }
        private IHubContext<NotificationHub, INotificationHub> HubContext { get; }

        public VideoService(AzureVideoIndexerService azureVideoIndexerService, AzureBlobStorageService azureBlobStorageService,
            DataStorageConfiguration dataStorageConfiguration, ICurrentUserProvider currentUserProvider,
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            AzureVideoIndexerConfiguration azureVideoIndexerConfiguration,
            CustomHttpClient customHttpClient,
            IHubContext<NotificationHub, INotificationHub> hubContext
            )
        {
            this.AzureVideoIndexerService = azureVideoIndexerService;
            this.AzureBlobStorageService = azureBlobStorageService;
            this.DataStorageConfiguration = dataStorageConfiguration;
            this.CurrentUserProvider = currentUserProvider;
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.AzureVideoIndexerConfiguration = azureVideoIndexerConfiguration;
            this.CustomHttpClient = customHttpClient;
            this.HubContext = hubContext;
        }

        public async Task<bool> UpdateVideoIndexStatusAsync(string[] videoIds,
            Common.Global.Enums.VideoIndexStatus videoIndexStatus,
            CancellationToken cancellationToken)
        {
            var query = this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser).Where(p => videoIds.Contains(p.VideoId));
            foreach (var singleVideoEntity in query)
            {
                var singleVideoIndex = await this.AzureVideoIndexerService
                    .GetVideoIndexAsync(singleVideoEntity.VideoId, cancellationToken);
                singleVideoEntity.VideoIndexSourceClass = this.GetType().FullName;
                singleVideoEntity.VideoIndexStatusId = (short)videoIndexStatus;
                singleVideoEntity.VideoDurationInSeconds = singleVideoIndex.summarizedInsights.duration.seconds;
            }
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            foreach (var singleVideoEntity in query)
            {
                await this.HubContext.Clients.User(singleVideoEntity.ApplicationUser.AzureAdB2cobjectId.ToString())
                    .ReceiveMessage(new Models.Notifications.NotificationModel()
                    {
                        Message = $"Your video: '{singleVideoEntity.Name}' has been processed"
                    });
            }
            return true;
        }

        public async Task<List<KeywordInfoModel>> GetIndexedVideoKeywordsAsync(string videoId, CancellationToken cancellationToken)
        {
            return await this.AzureVideoIndexerService.GetVideoKeywordsAsync(videoId, cancellationToken);

        }

        public async Task<GlobalKeywordModel[]> ListAllKeywordsAsync(CancellationToken cancellationToken)
        {
            return await this.FairplaytubeDatabaseContext.VideoIndexKeyword
                .GroupBy(p => p.Keyword)
                .Select(p => new GlobalKeywordModel()
                {
                    Keyword = p.Key,
                    Appeareances = p.Count()
                }).ToArrayAsync();
        }

        public async Task<string[]> GetDatabaseProcessingVideosIdsAsync(CancellationToken cancellationToken)
        {
            return await this.FairplaytubeDatabaseContext.VideoInfo.Where(p => p.VideoIndexStatusId ==
            (short)Common.Global.Enums.VideoIndexStatus.Processing)
                .Select(p => p.VideoId).ToArrayAsync(cancellationToken: cancellationToken);
        }

        public async Task<string> GetVideoEditAccessTokenAsync(string videoId)
        {
            var accessToken = await this.AzureVideoIndexerService.GetVideoAccessTokenStringAsync(videoId, allowEdit: true);
            return accessToken;
        }

        public async Task<SearchVideosResponse> GetVideoIndexerStatus(string[] videoIds,
            CancellationToken cancellationToken)
        {
            return await this.AzureVideoIndexerService.SearchVideosByIdsAsync(videoIds, cancellationToken);
        }

        public async Task<GetVideoIndexResponse> GetVideoIndexerStatus(string videoId,
            CancellationToken cancellationToken)
        {
            return await this.AzureVideoIndexerService.GetVideoIndexAsync(videoId, cancellationToken);
        }

        public IQueryable<VideoInfo> GetPublicProcessedVideos()
        {
            return this.FairplaytubeDatabaseContext.VideoInfo.Include(p => p.ApplicationUser)
                .ThenInclude(p => p.UserExternalMonetization)
                .Where(p =>
            p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processed)
                .OrderByDescending(p => p.VideoInfoId);
        }

        public IQueryable<VideoInfo> GetPublicProcessedVideosByKeyword(string keyword)
        {
            return this.FairplaytubeDatabaseContext.VideoInfo.Include(p => p.ApplicationUser)
                .ThenInclude(p => p.UserExternalMonetization)
                .Include(p => p.VideoIndexKeyword)
                .Where(p => p.VideoIndexKeyword.Any(k => k.Keyword.Contains(keyword)) &&
            p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processed);
        }

        public IQueryable<VideoInfo> GetPublicProcessedVideosByUserId(
            string azureAdB2cobjectId)
        {
            return this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser)
                .Where(p => p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processed &&
                p.ApplicationUser.AzureAdB2cobjectId.ToString() == azureAdB2cobjectId);
        }



        public async Task<bool> UploadVideoAsync(UploadVideoModel uploadVideoModel,
            CancellationToken cancellationToken)
        {
            MemoryStream stream = null;
            var fileExtension = Path.GetExtension(uploadVideoModel.SourceUrl);
            if (String.IsNullOrWhiteSpace(fileExtension))
                throw new Exception("Please make sure the source file has a valid extension like .mp4");
            var existentVideoName = await this.FairplaytubeDatabaseContext.VideoInfo
                .SingleOrDefaultAsync(p =>
            p.AccountId.ToString() == this.AzureVideoIndexerConfiguration.AccountId &&
            p.Location == this.AzureVideoIndexerConfiguration.Location &&
            p.Name == uploadVideoModel.Name, cancellationToken: cancellationToken);
            if (existentVideoName != null)
                throw new Exception($"Unable to use the Name: {uploadVideoModel.Name}. Please use another name and try again");
            byte[] fileBytes = null;
            if (!String.IsNullOrWhiteSpace(uploadVideoModel.SourceUrl))
            {
                fileBytes = await this.CustomHttpClient.GetByteArrayAsync(uploadVideoModel.SourceUrl);
            }
            stream = new MemoryStream(fileBytes);
            cancellationToken.ThrowIfCancellationRequested();
            var userAzueAdB2cObjectId = this.CurrentUserProvider.GetObjectId();
            stream.Position = 0;
            cancellationToken.ThrowIfCancellationRequested();
            var newFileName = $"{uploadVideoModel.Name}{fileExtension}";
            await this.AzureBlobStorageService.UploadFileAsync(this.DataStorageConfiguration.ContainerName,
                $"{userAzueAdB2cObjectId}/{newFileName}",
                stream, cancellationToken: cancellationToken);
            string fileUrl = $"https://{this.DataStorageConfiguration.AccountName}.blob.core.windows.net" +
                $"/{this.DataStorageConfiguration.ContainerName}/{userAzueAdB2cObjectId}/{newFileName}";
            cancellationToken.ThrowIfCancellationRequested();
            var user = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleAsync(p => p.AzureAdB2cobjectId.ToString() == userAzueAdB2cObjectId);
            cancellationToken.ThrowIfCancellationRequested();
            await this.FairplaytubeDatabaseContext.VideoInfo.AddAsync(new DataAccess.Models.VideoInfo()
            {
                ApplicationUserId = user.ApplicationUserId,
                Description = uploadVideoModel.Description,
                Location = this.AzureVideoIndexerConfiguration.Location,
                Name = uploadVideoModel.Name,
                Price = uploadVideoModel.Price,
                //VideoId = indexVideoResponse.id,
                VideoBloblUrl = fileUrl,
                //IndexedVideoUrl = $"https://www.videoindexer.ai/embed/player/{this.AzureVideoIndexerConfiguration.AccountId}" +
                //$"/{indexVideoResponse.id}/" +
                //$"?&locale=en&location={this.AzureVideoIndexerConfiguration.Location}",
                AccountId = Guid.Parse(this.AzureVideoIndexerConfiguration.AccountId),
                FileName = newFileName,
                VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Pending
            }, cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return true;
        }

        public async Task SaveIndexedVideoKeywordsAsync(string videoId, CancellationToken cancellationToken)
        {
            var keywordsResponse = await this.GetIndexedVideoKeywordsAsync(videoId, cancellationToken);
            if (keywordsResponse.Count > 0)
            {
                var videoInfoEntity = await this.FairplaytubeDatabaseContext.VideoInfo.Where(p => p.VideoId == videoId).SingleAsync();
                var existentKeywords =
                    this.FairplaytubeDatabaseContext.VideoIndexKeyword
                    .Where(p => p.VideoInfoId == videoInfoEntity.VideoInfoId);
                if (existentKeywords.Count() > 0)
                {
                    foreach (var singleExistentKeyword in existentKeywords)
                    {
                        this.FairplaytubeDatabaseContext.VideoIndexKeyword.Remove(singleExistentKeyword);
                    }
                    await this.FairplaytubeDatabaseContext.SaveChangesAsync();
                }
                foreach (var singleKeyword in keywordsResponse)
                {
                    await this.FairplaytubeDatabaseContext.VideoIndexKeyword.AddAsync(new VideoIndexKeyword()
                    {
                        VideoInfoId = videoInfoEntity.VideoInfoId,
                        Keyword = singleKeyword.Keyword
                    });
                }
                await this.FairplaytubeDatabaseContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsVideoOwnerAsync(string videoId, string azureAdB2cobjectId,
            CancellationToken cancellationToken)
        {
            bool isVideoOwner = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser)
                .AnyAsync(p => p.VideoId == videoId && p.ApplicationUser.AzureAdB2cobjectId.ToString() == azureAdB2cobjectId,
                cancellationToken: cancellationToken);
            return isVideoOwner;
        }

        public async Task AddVideoIndexTransactionsAsync(string[] videoIds,
            CancellationToken cancellationToken)
        {
            var query = this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser).Where(p => videoIds.Contains(p.VideoId));

            foreach (var singleVideoEntity in query)
            {
                await this.FairplaytubeDatabaseContext.VideoIndexingTransaction.AddAsync(new VideoIndexingTransaction()
                {
                    VideoInfoId = singleVideoEntity.VideoInfoId
                }, cancellationToken);
            }

            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
        }

        public async Task UpdateVideo(string videoId, UpdateVideoModel model)
        {
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .SingleOrDefaultAsync(p => p.VideoId == videoId);
            if (videoEntity == null)
                throw new Exception($"Unable to find video with Id: {videoId}");
            videoEntity.Price = model.Price;
            await this.FairplaytubeDatabaseContext.SaveChangesAsync();
        }
    }
}
