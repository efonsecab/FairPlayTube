using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Persons;
using FairPlayTube.Models.Video;
using FairPlayTube.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.AzureVideoIndexer.Models.CreateProject;
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
using static PTI.Microservices.Library.Services.AzureVideoIndexerService;

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
        private TextAnalysisService TextAnalysisService { get; }

        public VideoService(AzureVideoIndexerService azureVideoIndexerService, AzureBlobStorageService azureBlobStorageService,
            DataStorageConfiguration dataStorageConfiguration, ICurrentUserProvider currentUserProvider,
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            AzureVideoIndexerConfiguration azureVideoIndexerConfiguration,
            CustomHttpClient customHttpClient,
            IHubContext<NotificationHub, INotificationHub> hubContext,
            TextAnalysisService textAnalysisService
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
            this.TextAnalysisService = textAnalysisService;
        }


        public async Task MarkVideoAsProcessed(VideoInfo videoInfo, CancellationToken cancellationToken)
        {
            var videoIndex = await GetVideoIndexerStatus(videoInfo.VideoId, cancellationToken);
            await SaveIndexedVideoKeywordsAsync(videoInfo.VideoId, cancellationToken);
            await SaveVideoThumbnailAsync(videoInfo.VideoId, videoIndex.videos.First().thumbnailId, cancellationToken);
            await UpdateVideoIndexStatusAsync(Common.Global.Enums.VideoIndexStatus.Processed, cancellationToken: cancellationToken, videoInfo.VideoId);
        }

        public async Task<bool> DeleteVideoAsync(string userAzureAdB2cObjectId, string videoId, CancellationToken cancellationToken)
        {
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                                        .Include(p => p.ApplicationUser)
                                        .Include(p => p.VideoAccessTransaction)
                                        .SingleOrDefaultAsync(p => p.VideoId == videoId);

            if (videoEntity == null)
                throw new Exception($"Unable to find the video with id {videoId}");

            if (videoEntity.VideoIndexStatusId != (short)Common.Global.Enums.VideoIndexStatus.Processed)
                throw new InvalidOperationException($"Video with id {videoId} cannot be deleted because it has not been indexed yet");

            if (videoEntity.VideoAccessTransaction.Any())
                throw new InvalidOperationException($"Video with id {videoId} cannot be deleted because it was already bought");

            // DELETING VIDEO KEYWORDS

            var existentKeywords = this.FairplaytubeDatabaseContext.VideoIndexKeyword
                                       .Where(p => p.VideoInfoId == videoEntity.VideoInfoId);

            if (existentKeywords.Any())
            {
                this.FairplaytubeDatabaseContext.VideoIndexKeyword.RemoveRange(existentKeywords);
                await this.FairplaytubeDatabaseContext.SaveChangesAsync();
            }

            // UPDATING VIDEO INFO STATUS
            videoEntity.VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Deleted;
            await this.FairplaytubeDatabaseContext.SaveChangesAsync();

            // DELETING VIDEO INDEXER
            var azureVideoIndexerEntity = (await this.AzureVideoIndexerService.SearchVideosByIdsAsync(new string[] { videoId })).results.Single();
            await this.AzureVideoIndexerService.DeleteVideoAsync(videoId, cancellationToken);

            // DELETING VIDEO FROM BLOB STORAGE (ThumbnailUrl)
            string relativePath =
                $"{videoEntity.ApplicationUser.AzureAdB2cobjectId}/Video/{videoId}/Thumbnail/{azureVideoIndexerEntity.thumbnailId}.jpg";
            await this.AzureBlobStorageService.DeleteFileAsync(this.DataStorageConfiguration.ContainerName, relativePath, cancellationToken);

            // DELETING VIDEO FROM BLOB STORAGE (VIDEO)
            await this.AzureBlobStorageService.DeleteFileAsync(this.DataStorageConfiguration.ContainerName, $"{userAzureAdB2cObjectId}/{videoEntity.FileName}", cancellationToken);

            return true;
        }


        public async Task<IQueryable<VideoInfo>> SearchVideosByPersonNameAsync(string personName, CancellationToken cancellationtoken)
        {
            var searchVideoResults = await AzureVideoIndexerService.SearchVideosAsync(personName, new SearchScope[] { SearchScope.NamedPeople }, cancellationToken: cancellationtoken);
            var videoIds = searchVideoResults.results.Select(searchVideo => searchVideo.id);
            return this.FairplaytubeDatabaseContext
                       .VideoInfo
                       .Join(videoIds, video => video.VideoId, search => search, (video, _) => video);
        }

        public async Task<bool> UpdateVideoIndexStatusAsync(Common.Global.Enums.VideoIndexStatus videoIndexStatus,
            CancellationToken cancellationToken,
            params string[] videoIds)
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
                await NotifyVideoOwner(singleVideoEntity);
                await NotifyFollowers(singleVideoEntity);
            }
            return true;
        }

        private async Task NotifyFollowers(VideoInfo singleVideoEntity)
        {
            var followers = this.FairplaytubeDatabaseContext
                                .UserFollower.Include(p => p.FollowerApplicationUser)
                                .Where(p => p.FollowedApplicationUserId == singleVideoEntity.ApplicationUserId);
            if (followers.Count() > 0)
            {
                foreach (var singleFollower in followers)
                {
                    await this.HubContext.Clients.User(singleFollower.FollowerApplicationUser.AzureAdB2cobjectId.ToString())
                        .ReceiveMessage(new Models.Notifications.NotificationModel()
                        {
                            Message = $"A user you follow {singleVideoEntity.ApplicationUser.FullName} has uploaded a new video titled: {singleVideoEntity.Name}",
                            VideoId = singleVideoEntity.VideoId
                        });
                }
            }
        }

        private async Task NotifyVideoOwner(VideoInfo singleVideoEntity)
        {
            await this.HubContext.Clients.User(singleVideoEntity.ApplicationUser.AzureAdB2cobjectId.ToString())
                                .ReceiveMessage(new Models.Notifications.NotificationModel()
                                {
                                    Message = $"Your video: '{singleVideoEntity.Name}' has been processed"
                                });
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

        public async Task SavePersonsAsync(List<PersonModel> personsModels, CancellationToken cancellationToken)
        {
            var videoIndexerPersonsIds = personsModels.Select(p => p.Id).ToArray();
            var existentIds = await this.FairplaytubeDatabaseContext.Person
                .Where(p => videoIndexerPersonsIds.Contains(p.Id)).Select(p => p.Id).ToArrayAsync();
            var modelstoInsert = personsModels.Where(p => !existentIds.Contains(p.Id));
            if (modelstoInsert.Count() > 0)
            {
                foreach (var singleModelToInsert in modelstoInsert)
                {
                    var videoIndexerPersonModelId = Guid.Parse(singleModelToInsert.PersonModelId);
                    var videoIndexerPersonId = Guid.Parse(singleModelToInsert.Id);
                    var sampleFaceId = Guid.Parse(singleModelToInsert.SampleFaceId);
                    var sampleFacePictureBytes = await this.AzureVideoIndexerService
                        .GetCustomFacePictureAsync(videoIndexerPersonModelId, videoIndexerPersonId, sampleFaceId, cancellationToken);
                    string relativePath = $"Persons/{videoIndexerPersonId}/Faces/{sampleFaceId}/SampleFace.jpg";
                    MemoryStream sampleFaceStream = new MemoryStream(sampleFacePictureBytes);
                    sampleFaceStream.Position = 0;
                    await this.AzureBlobStorageService.UploadFileAsync(this.DataStorageConfiguration.ContainerName, relativePath,
                        sampleFaceStream, true, cancellationToken);

                    string sampleFaceUrl = $"https://{this.DataStorageConfiguration.AccountName}" +
                        $".blob.core.windows.net/" +
                        $"{this.DataStorageConfiguration.ContainerName}" +
                        $"/Persons/{videoIndexerPersonId}" +
                        $"/Faces/{sampleFaceId}" +
                        $"/SampleFace.jpg";
                    await this.FairplaytubeDatabaseContext.Person.AddAsync(new Person()
                    {
                        Id = singleModelToInsert.Id,
                        Name = singleModelToInsert.Name,
                        SampleFaceId = singleModelToInsert.SampleFaceId,
                        SampleFaceSourceType = singleModelToInsert.SampleFaceSourceType,
                        SampleFaceState = singleModelToInsert.SampleFaceState,
                        SampleFaceUrl = sampleFaceUrl
                    }, cancellationToken: cancellationToken);
                }
            }
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
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

        public async Task BuyVideoAccessAsync(string azureAdB2CObjectId, string videoId, CancellationToken cancellationToken)
        {
            var videoAccessEntity = await this.FairplaytubeDatabaseContext.VideoAccessTransaction
                .Where(p => p.VideoInfo.VideoId == videoId &&
                p.BuyerApplicationUser.AzureAdB2cobjectId.ToString() == azureAdB2CObjectId)
                .SingleOrDefaultAsync();
            if (videoAccessEntity != null)
                throw new Exception("User has already bought access to the specified video");
            var userEntity = await this.FairplaytubeDatabaseContext.ApplicationUser.SingleAsync(p => p.AzureAdB2cobjectId.ToString() == azureAdB2CObjectId);
            var videoInfoEntity = await this.FairplaytubeDatabaseContext.VideoInfo.SingleAsync(p => p.VideoId == videoId);
            var videoPrice = videoInfoEntity.Price;
            var userAvailableFunds = userEntity.AvailableFunds;
            var commissionToApply = Common.Global.Constants.Commissions.VideoAccess;
            var totalPrice = videoPrice + (videoPrice * commissionToApply);
            if (userAvailableFunds < totalPrice)
            {
                string message = $"User {azureAdB2CObjectId} cannot buy access to video: {videoId}. Total Price: {totalPrice}. Available Funds: {userAvailableFunds}";
                throw new Exception(message);
            }
            await this.FairplaytubeDatabaseContext.VideoAccessTransaction.AddAsync(new VideoAccessTransaction()
            {
                AppliedPrice = videoPrice,
                BuyerApplicationUserId = userEntity.ApplicationUserId,
                AppliedCommission = commissionToApply,
                TotalPrice = totalPrice,
                VideoInfoId = videoInfoEntity.VideoInfoId
            });
            userEntity.AvailableFunds -= totalPrice;
            await this.FairplaytubeDatabaseContext.SaveChangesAsync();
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
            return this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.VideoJob).Include(p => p.ApplicationUser)
                .ThenInclude(p => p.UserYouTubeChannel)
                .Include(p => p.ApplicationUser.UserExternalMonetization)
                .Include(p => p.VisitorTracking)
                .Where(p =>
            p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processed
            && p.VideoVisibilityId == (short)Common.Global.Enums.VideoVisibility.Public
            )
                .OrderByDescending(p => p.VideoInfoId);
        }

        public IQueryable<VideoInfo> GetVideoAsync(string videoId)
        {
            return this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.VideoJob).Include(p => p.ApplicationUser)
                .ThenInclude(p => p.UserYouTubeChannel)
                .Include(p => p.ApplicationUser.UserExternalMonetization)
                .Include(p => p.VisitorTracking)
                .Where(p => p.VideoId == videoId);
        }

        public async Task<string[]> GetBoughtVideosIds(string userObjectId)
        {
            var boughtVideos = this.FairplaytubeDatabaseContext.VideoAccessTransaction
                .Include(p => p.VideoInfo).Include(p => p.BuyerApplicationUser)
                .Where(p => p.BuyerApplicationUser.AzureAdB2cobjectId.ToString() == userObjectId)
                .Select(p => p.VideoInfo);
            var result = await boughtVideos.Select(p => p.VideoId).ToArrayAsync();
            return result;
        }

        public IQueryable<VideoInfo> GetPublicProcessedVideosByKeyword(string keyword)
        {
            return this.FairplaytubeDatabaseContext.VideoInfo.Include(p => p.ApplicationUser)
                .ThenInclude(p => p.UserExternalMonetization)
                .Include(p => p.VideoIndexKeyword)
                .Where(p => p.VideoIndexKeyword.Any(k => k.Keyword.Contains(keyword)) &&
            p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processed);
        }


        public IQueryable<VideoInfo> GetProcessedVideosByUserId(
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
            var fileExtension = string.Empty;
            var userAzueAdB2cObjectId = this.CurrentUserProvider.GetObjectId();
            MemoryStream stream = null;
            if (!uploadVideoModel.UseSourceUrl)
            {
                string fileRelativePath =
                    $"User/{userAzueAdB2cObjectId}/{uploadVideoModel.StoredFileName}";
                stream = new MemoryStream();
                var response = await this.AzureBlobStorageService
                    .GetFileStreamAsync(this.DataStorageConfiguration.UntrustedUploadsContainerName,
                    fileRelativePath, stream, cancellationToken);
                fileExtension = Path.GetExtension(uploadVideoModel.StoredFileName);
            }
            else
            {
                fileExtension = Path.GetExtension(uploadVideoModel.SourceUrl);
                if (String.IsNullOrWhiteSpace(fileExtension))
                    throw new Exception("Please make sure the source file has a valid extension like .mp4");
            }
            var existentVideoName = await this.FairplaytubeDatabaseContext.VideoInfo
                .SingleOrDefaultAsync(p =>
            p.AccountId.ToString() == this.AzureVideoIndexerConfiguration.AccountId &&
            p.Location == this.AzureVideoIndexerConfiguration.Location &&
            p.Name == uploadVideoModel.Name, cancellationToken: cancellationToken);
            if (existentVideoName != null)
                throw new Exception($"Unable to use the Name: {uploadVideoModel.Name}. Please use another name and try again");

            if (uploadVideoModel.UseSourceUrl)
            {
                byte[] fileBytes = null;
                if (!String.IsNullOrWhiteSpace(uploadVideoModel.SourceUrl))
                {
                    fileBytes = await this.CustomHttpClient.GetByteArrayAsync(uploadVideoModel.SourceUrl);
                }
                stream = new MemoryStream(fileBytes);
            }
            cancellationToken.ThrowIfCancellationRequested();
            stream.Position = 0;
            cancellationToken.ThrowIfCancellationRequested();
            var newFileName = $"{uploadVideoModel.Name}{fileExtension}";
            await this.AzureBlobStorageService.UploadFileAsync(this.DataStorageConfiguration.ContainerName,
                $"{userAzueAdB2cObjectId}/{newFileName}",
                stream, overwrite: false, cancellationToken: cancellationToken);
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
                VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Pending,
                VideoLanguageCode = uploadVideoModel.Language,
                VideoVisibilityId = (short)uploadVideoModel.VideoVisibility
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

        public async Task<bool> IsVideosOwnerAsync(string[] videosIds, string azureAdB2cobjectId, CancellationToken cancellationToken)
        {
            bool isVideosOwner = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser)
                .AnyAsync(p => videosIds.Contains(p.VideoId) && p.ApplicationUser.AzureAdB2cobjectId.ToString() == azureAdB2cobjectId,
                cancellationToken: cancellationToken);
            return isVideosOwner;
        }

        public async Task AddVideoIndexTransactionsAsync(string[] videoIds,
            CancellationToken cancellationToken)
        {
            var query = this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser).Where(p => videoIds.Contains(p.VideoId));

            //var costPerMinute = this.FairplaytubeDatabaseContext.VideoIndexingCost
            //    .OrderByDescending(d => d.RowCreationDateTime)
            //    .FirstOrDefault()
            //    .CostPerMinute;

            foreach (var singleVideoEntity in query)
            {
                await this.FairplaytubeDatabaseContext.VideoIndexingTransaction.AddAsync(new VideoIndexingTransaction()
                {
                    VideoInfoId = singleVideoEntity.VideoInfoId,
                    //IndexingCost = costPerMinute * ((decimal)singleVideoEntity.VideoDurationInSeconds / 60)
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

        public async Task<List<VideoInfo>> GetUserPendingVideosQueueAsync(string azureAdB2cobjectId, CancellationToken cancellationToken = default)
        {
            var videosEntitities = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser)
                .Include(p => p.VideoIndexStatus)
                .Where(p => p.ApplicationUser.AzureAdB2cobjectId.ToString() == azureAdB2cobjectId
                && p.VideoIndexStatusId != (short)Common.Global.Enums.VideoIndexStatus.Processed
                )
                .OrderByDescending(p => p.VideoId)
                .ToListAsync();
            return videosEntitities;
        }

        public async Task SaveVideoThumbnailAsync(string videoId, string thumbnailId, CancellationToken cancellationToken)
        {
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser).SingleOrDefaultAsync(p => p.VideoId == videoId);
            if (videoEntity == null)
                throw new Exception($"Video: {videoId} does not exit");
            var videoThumbnailBase64 = await this.AzureVideoIndexerService.GetVideoThumbnailAsync(videoId, thumbnailId);
            string relativePath =
                $"{videoEntity.ApplicationUser.AzureAdB2cobjectId}/Video/{videoId}/Thumbnail/{thumbnailId}.jpg";
            var byteArray = Convert.FromBase64String(videoThumbnailBase64);
            MemoryStream memoryStream = new MemoryStream(byteArray);
            memoryStream.Position = 0;
            var result =
            await this.AzureBlobStorageService.UploadFileAsync(this.DataStorageConfiguration.ContainerName, relativePath,
                memoryStream, true, cancellationToken);
            string videoThumbnailUrl = $"https://{DataStorageConfiguration.AccountName}.blob.core.windows.net/{DataStorageConfiguration.ContainerName}/{relativePath}";
            videoEntity.ThumbnailUrl = videoThumbnailUrl;
            await this.FairplaytubeDatabaseContext.SaveChangesAsync();
        }

        public async Task<List<PersonModel>> GetAllPersonsAsync(CancellationToken cancellationToken)
        {
            List<PersonModel> result = new List<PersonModel>();
            var allPersonModels = await this.AzureVideoIndexerService.GetAllPersonModelsAsync(cancellationToken);
            var defaultModel = allPersonModels.Single(p => p.isDefault == true);
            Guid defaultModelId = Guid.Parse(defaultModel.id);
            var allPersonsInModel = await this.AzureVideoIndexerService.GetPersonsInModelAsync(defaultModelId, cancellationToken);
            if (allPersonsInModel.results.Length > 0)
            {
                result = allPersonsInModel.results.Select(p => new PersonModel
                {
                    Id = p.id,
                    PersonModelId = defaultModel.id,
                    Name = p.name,
                    SampleFaceId = p.sampleFace.id,
                    SampleFaceSourceType = p.sampleFace.sourceType,
                    SampleFaceState = p.sampleFace.state
                }).ToList();
            }
            return result;
        }

        public async Task<List<Person>> GetPersistedPersonsAsync(CancellationToken cancellation)
        {
            return await this.FairplaytubeDatabaseContext.Person.ToListAsync();
        }
        public async Task<ProjectModel> CreateCustomRenderingProject(ProjectModel projectModel, CancellationToken cancellationToken)
        {
            CreateProjectRequest createProjectRequestModel = new CreateProjectRequest()
            {
                name = projectModel.Name,
                videosRanges = projectModel.Videos.Select(p =>
                new PTI.Microservices.Library.AzureVideoIndexer.Models.CreateProject.Videosrange()
                {
                    videoId = p.VideoId,
                    range = new PTI.Microservices.Library.AzureVideoIndexer.Models.CreateProject.Range()
                    {
                        start = p.Start,
                        end = p.End
                    }
                }).ToArray()
            };
            var result = await this.AzureVideoIndexerService.CreateProjectAsync(createProjectRequestModel, cancellationToken: cancellationToken);
            projectModel.ProjectId = result.id;
            return projectModel;
        }

        public async Task<byte[]> DownloadVideoAsync(string videoId, CancellationToken cancellationToken)
        {
            var url = await this.AzureVideoIndexerService.GetVideoSourceFileDownloadUrlAsync(videoId, cancellationToken);
            var videoBytes = await this.CustomHttpClient.GetByteArrayAsync(url, cancellationToken);
            return videoBytes;
        }

        public async Task<string> GetVideoFileNameAsync(string videoId, CancellationToken cancellationToken)
        {
            var videoInfo = await this.FairplaytubeDatabaseContext.VideoInfo.SingleAsync(p => p.VideoId == videoId, cancellationToken);
            return videoInfo.FileName;
        }
    }
}
