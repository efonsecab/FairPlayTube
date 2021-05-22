using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Models.AzureVideoIndexerService.SearchVideos;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AzureVideoIndexerModels = PTI.Microservices.Library.Models.AzureVideoIndexerService;

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

        public VideoService(AzureVideoIndexerService azureVideoIndexerService, AzureBlobStorageService azureBlobStorageService,
            DataStorageConfiguration dataStorageConfiguration, ICurrentUserProvider currentUserProvider,
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            AzureVideoIndexerConfiguration azureVideoIndexerConfiguration
            )
        {
            this.AzureVideoIndexerService = azureVideoIndexerService;
            this.AzureBlobStorageService = azureBlobStorageService;
            this.DataStorageConfiguration = dataStorageConfiguration;
            this.CurrentUserProvider = currentUserProvider;
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.AzureVideoIndexerConfiguration = azureVideoIndexerConfiguration;
        }

        public async Task<bool> UpdateVideoIndexStatusAsync(string[] videoIds,
            Common.Global.Enums.VideoIndexStatus videoIndexStatus,
            CancellationToken cancellationToken)
        {
            var query = this.FairplaytubeDatabaseContext.VideoInfo.Where(p => videoIds.Contains(p.VideoId));
            foreach (var singleVideoEntity in query)
            {
                singleVideoEntity.VideoIndexStatusId = (short)videoIndexStatus;
            }
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return true;
        }

        public async Task<string[]> GetDatabaseProcessingVideosIdsAsync(CancellationToken cancellationToken)
        {
            return await this.FairplaytubeDatabaseContext.VideoInfo.Where(p => p.VideoIndexStatusId ==
            (int)Common.Global.Enums.VideoIndexStatus.Processing)
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

        public IQueryable<VideoInfo> GetPublicProcessedVideos()
        {
            return this.FairplaytubeDatabaseContext.VideoInfo.Where(p =>
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



        public async Task<string> UploadVideoAsync(UploadVideoModel uploadVideoModel,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            MemoryStream stream = new MemoryStream(uploadVideoModel.FileBytes);
            var userAzueAdB2cObjectId = this.CurrentUserProvider.GetObjectId();
            string extension = Path.GetExtension(uploadVideoModel.FileName);
            string newFileName = $"{uploadVideoModel.Name}{extension}";
            var existentVideo = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser)
                .SingleOrDefaultAsync(p =>
            p.AccountId.ToString() == this.AzureVideoIndexerConfiguration.AccountId &&
            p.Location == this.AzureVideoIndexerConfiguration.Location &&
            p.ApplicationUser.AzureAdB2cobjectId.ToString() == userAzueAdB2cObjectId &&
            p.FileName == newFileName, cancellationToken: cancellationToken);
            if (existentVideo != null)
                throw new Exception($"You already have a video named: {newFileName}. Please use another name and try again");
            stream.Position = 0;
            cancellationToken.ThrowIfCancellationRequested();
            await this.AzureBlobStorageService.UploadFileAsync(this.DataStorageConfiguration.ContainerName,
                $"{userAzueAdB2cObjectId}/{newFileName}",
                stream, cancellationToken: cancellationToken);
            string fileUrl = $"https://{this.DataStorageConfiguration.AccountName}.blob.core.windows.net" +
                $"/{this.DataStorageConfiguration.ContainerName}/{userAzueAdB2cObjectId}/{newFileName}";
            cancellationToken.ThrowIfCancellationRequested();
            var allPersonModels = await this.AzureVideoIndexerService.GetAllPersonModelsAsync(cancellationToken);
            var defaultPersonModel = allPersonModels.Single(p => p.isDefault == true);
            cancellationToken.ThrowIfCancellationRequested();
            var indexVideoResponse =
            await this.AzureVideoIndexerService.UploadVideoAsync(new Uri(fileUrl),
                uploadVideoModel.Name, uploadVideoModel.Description, newFileName,
                personModelId: Guid.Parse(defaultPersonModel.id), privacy: AzureVideoIndexerService.VideoPrivacy.Public,
                callBackUri: new Uri("https://fairplaytube.com"), cancellationToken: cancellationToken);
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
                VideoId = indexVideoResponse.id,
                VideoBloblUrl = fileUrl,
                IndexedVideoUrl = $"https://www.videoindexer.ai/embed/player/{this.AzureVideoIndexerConfiguration.AccountId}" +
                $"/{indexVideoResponse.id}/" +
                $"?&locale=en&location={this.AzureVideoIndexerConfiguration.Location}",
                AccountId = Guid.Parse(this.AzureVideoIndexerConfiguration.AccountId),
                FileName = uploadVideoModel.FileName,
                VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Processing
            }, cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return indexVideoResponse.id;
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
    }
}
