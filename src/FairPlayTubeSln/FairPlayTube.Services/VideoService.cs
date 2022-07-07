using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Persons;
using FairPlayTube.Models.Video;
using FairPlayTube.Notifications.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PTI.Microservices.Library.Services.AzureVideoIndexerService;

namespace FairPlayTube.Services
{
    public class VideoService
    {
        private AzureBlobStorageService AzureBlobStorageService { get; }
        private DataStorageConfiguration DataStorageConfiguration { get; set; }
        private ICurrentUserProvider CurrentUserProvider { get; set; }
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private CustomHttpClient CustomHttpClient { get; }
        private IHubContext<NotificationHub, INotificationHub> HubContext { get; }
        private EmailService EmailService { get; set; }
        private IConfiguration Configuration { get; set; }
        private VideoIndexerService VideoIndexerService { get; }

        public VideoService(AzureBlobStorageService azureBlobStorageService,
            DataStorageConfiguration dataStorageConfiguration, ICurrentUserProvider currentUserProvider,
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            CustomHttpClient customHttpClient,
            IHubContext<NotificationHub, INotificationHub> hubContext,
            EmailService emailService,
            IConfiguration configuration,
            VideoIndexerService videoIndexerService)
        {
            this.AzureBlobStorageService = azureBlobStorageService;
            this.DataStorageConfiguration = dataStorageConfiguration;
            this.CurrentUserProvider = currentUserProvider;
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.CustomHttpClient = customHttpClient;
            this.HubContext = hubContext;
            this.EmailService = emailService;
            this.Configuration = configuration;
            this.VideoIndexerService = videoIndexerService;
        }


        public async Task MarkVideoAsProcessedAsync(VideoInfo videoInfo, CancellationToken cancellationToken)
        {
            string accountId = videoInfo.AccountId.ToString();
            var videoIndex = await GetVideoIndexerStatusAsync(accountId, videoInfo.VideoId, cancellationToken);
            await SaveIndexedVideoKeywordsAsync(accountId, videoInfo.VideoId, cancellationToken);
            await SaveVideoThumbnailAsync(accountId, videoInfo.VideoId, videoIndex.videos.First().thumbnailId, cancellationToken);
            await UpdateVideoIndexStatusAsync(accountId, Common.Global.Enums.VideoIndexStatus.Processed, cancellationToken: cancellationToken, videoIds: new[] { videoInfo.VideoId });
        }

        public async Task<bool> DeleteVideoAsync(string accountId, string userAzureAdB2cObjectId, string videoId, CancellationToken cancellationToken)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                                        .Include(p => p.ApplicationUser)
                                        .Include(p => p.VideoAccessTransaction)
                                        .SingleOrDefaultAsync(p => p.VideoId == videoId, cancellationToken: cancellationToken);

            if (videoEntity == null)
                throw new CustomValidationException($"Unable to find the video with id {videoId}");

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
                await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
            }

            // UPDATING VIDEO INFO STATUS
            videoEntity.VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Deleted;
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);

            // DELETING VIDEO INDEXER
            var azureVideoIndexerEntity = (await azureVideoIndexerService
                .SearchVideosByIdsAsync(new string[] { videoId }, cancellationToken)).results.Single();
            await azureVideoIndexerService.DeleteVideoAsync(videoId, cancellationToken);

            // DELETING VIDEO FROM BLOB STORAGE (ThumbnailUrl)
            string relativePath =
                $"{videoEntity.ApplicationUser.AzureAdB2cobjectId}/Video/{videoId}/Thumbnail/{azureVideoIndexerEntity.thumbnailId}.jpg";
            await this.AzureBlobStorageService.DeleteFileAsync(this.DataStorageConfiguration.ContainerName, relativePath, cancellationToken);

            // DELETING VIDEO FROM BLOB STORAGE (VIDEO)
            await this.AzureBlobStorageService.DeleteFileAsync(this.DataStorageConfiguration.ContainerName, $"{userAzureAdB2cObjectId}/{videoEntity.FileName}", cancellationToken);

            return true;
        }


        public async Task<List<VideoInfo>> SearchVideosByPersonNameAsync(string personName, CancellationToken cancellationtoken)
        {
            List<VideoInfo> result = new();
            var videoIndexerAccountIds = this.VideoIndexerService.GetAllAccountIds(includeDisabledForIndexing:false);
            foreach (var singleVideoIndexerAccountId in videoIndexerAccountIds)
            {
                var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(singleVideoIndexerAccountId);
                var searchVideoResults = await azureVideoIndexerService.SearchVideosAsync(personName,
                    new SearchScope[] { SearchScope.NamedPeople }, cancellationToken: cancellationtoken);
                var videoIds = searchVideoResults.results.Select(searchVideo => searchVideo.id);
                var accountResults = await this.FairplaytubeDatabaseContext
                       .VideoInfo
                       .Join(videoIds, video => video.VideoId, search => search, (video, _) => video)
                       .ToArrayAsync();
                result.AddRange(accountResults);
            }
            return result;
        }

        public async Task<bool> HasReachedWeeklyVideoUploadLimitAsync(CancellationToken cancellationToken)
        {
            var userAdB2CObjectId = this.CurrentUserProvider.GetObjectId();
            var userEntity = await FairplaytubeDatabaseContext.ApplicationUser
                .Include(p => p.VideoInfo)
                .Include(p => p.ApplicationUserSubscriptionPlan).ThenInclude(p => p.SubscriptionPlan)
                .Where(p => p.AzureAdB2cobjectId.ToString() == userAdB2CObjectId)
                .SingleAsync(cancellationToken: cancellationToken);
            var userSubscription = userEntity.ApplicationUserSubscriptionPlan;
            if (userSubscription.SubscriptionPlanId == (short)Common.Global.Enums.SubscriptionPlan.Unlimited)
                return false;
            var uploadedVideosLast7Days =
                await FairplaytubeDatabaseContext.VideoInfo.Where(p => p.ApplicationUserId ==
                userEntity.ApplicationUserId && p.RowCreationDateTime >=
                DateTimeOffset.UtcNow.AddDays(-7))
                .CountAsync(cancellationToken: cancellationToken);
            if (uploadedVideosLast7Days < userSubscription.SubscriptionPlan.MaxAllowedWeeklyVideos)
                return false;
            else
                return true;
        }

        public async Task<bool> UpdateVideoIndexStatusAsync(string accountId,
            Common.Global.Enums.VideoIndexStatus videoIndexStatus,
            CancellationToken cancellationToken, string[] videoIds)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            var query = this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser).Where(p => videoIds.Contains(p.VideoId));
            foreach (var singleVideoEntity in query)
            {
                var singleVideoIndex = await azureVideoIndexerService
                    .GetVideoIndexAsync(singleVideoEntity.VideoId, cancellationToken);
                singleVideoEntity.VideoIndexSourceClass = this.GetType().FullName;
                singleVideoEntity.VideoIndexStatusId = (short)videoIndexStatus;
                singleVideoEntity.VideoDurationInSeconds = singleVideoIndex.summarizedInsights.duration.seconds;
            }
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            foreach (var singleVideoEntity in query)
            {
                await NotifyVideoOwnerAsync(singleVideoEntity);
                if (singleVideoEntity.VideoVisibilityId == (short)Common.Global.Enums.VideoVisibility.Public)
                {
                    //await NotifyFollowersAsync(singleVideoEntity);
                    await NotifyAllUsersAsync(singleVideoEntity, cancellationToken);
                }
            }
            return true;
        }

        private async Task NotifyAllUsersAsync(VideoInfo singleVideoEntity,
            CancellationToken cancellationToken)
        {
            var videoOwnerUser = await
                this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleAsync(p => p.ApplicationUserId == singleVideoEntity.ApplicationUserId,
                cancellationToken: cancellationToken);
            var allUsers = this.FairplaytubeDatabaseContext.ApplicationUser;
            if (allUsers.Any())
            {
                string message = $"User {videoOwnerUser.FullName} has upload a new video on FairPlayTube. Title: {singleVideoEntity.Name}";
                foreach (var singleUser in allUsers)
                {
                    await this.HubContext.Clients.User(singleUser.AzureAdB2cobjectId.ToString())
                        .ReceiveMessage(new Models.Notifications.NotificationModel()
                        {
                            Message = message,
                            VideoId = singleVideoEntity.VideoId
                        });

                    try
                    {
                        string baseUrl = Configuration["VideoIndexerCallbackUrl"];
                        var videoDetailsUrl = $"{baseUrl}/Public/Videos/Details/{singleVideoEntity.VideoId}";
                        StringBuilder emailMessage = new StringBuilder();

                        emailMessage.AppendLine("<p>");
                        emailMessage.AppendLine($"Hello {singleUser.FullName}. " +
                            $"You are receiving this email because you are a FairPlayTube user.");
                        emailMessage.AppendLine("</p>");

                        emailMessage.AppendLine("<p>");
                        emailMessage.AppendLine($"We have some information for you.");
                        emailMessage.AppendLine("</p>");

                        emailMessage.AppendLine("<p>");
                        emailMessage.AppendLine(message);
                        emailMessage.AppendLine("</p>");

                        emailMessage.AppendLine("<p>");
                        emailMessage.AppendLine($"<a href=\"{videoDetailsUrl}\">{singleVideoEntity.Name}</a>");
                        emailMessage.AppendLine("</p>");
                        await this.EmailService.SendEmailAsync(
                            toEmailAddress: singleUser.EmailAddress,
                            subject:
                            "FairPlayTube has a new video you may be interested in",
                            body: emailMessage.ToString(), isBodyHtml: true,
                            cancellationToken: cancellationToken
                            );
                    }
                    catch (Exception)
                    {
                        //capture the exception so the rest of the flow continues execution
                    }
                }
            }
        }

        private async Task NotifyFollowersAsync(VideoInfo singleVideoEntity)
        {
            var followers = this.FairplaytubeDatabaseContext
                                .UserFollower.Include(p => p.FollowerApplicationUser)
                                .Where(p => p.FollowedApplicationUserId == singleVideoEntity.ApplicationUserId);
            if (followers.Any())
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

        private async Task NotifyVideoOwnerAsync(VideoInfo singleVideoEntity)
        {
            await this.HubContext.Clients.User(singleVideoEntity.ApplicationUser.AzureAdB2cobjectId.ToString())
                                .ReceiveMessage(new Models.Notifications.NotificationModel()
                                {
                                    Message = $"Your video: '{singleVideoEntity.Name}' has been processed"
                                });
        }

        public async Task<List<KeywordInfoModel>> GetIndexedVideoKeywordsAsync(
            string accountId, string videoId, CancellationToken cancellationToken)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            return await azureVideoIndexerService.GetVideoKeywordsAsync(videoId, cancellationToken);

        }

        public async Task<GlobalKeywordModel[]> ListAllKeywordsAsync(CancellationToken cancellationToken)
        {
            return await this.FairplaytubeDatabaseContext.VideoIndexKeyword
                .GroupBy(p => p.Keyword)
                .Select(p => new GlobalKeywordModel()
                {
                    Keyword = p.Key,
                    Appeareances = p.Count()
                }).ToArrayAsync(cancellationToken: cancellationToken);
        }

        public async Task SavePersonsAsync(string accountId, List<PersonModel> personsModels, CancellationToken cancellationToken)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            var videoIndexerPersonsIds = personsModels.Select(p => p.Id).ToArray();
            var existentIds = await this.FairplaytubeDatabaseContext.Person
                .Where(p => videoIndexerPersonsIds.Contains(p.Id)).Select(p => p.Id)
                .ToArrayAsync(cancellationToken: cancellationToken);
            var modelstoInsert = personsModels.Where(p => !existentIds.Contains(p.Id));
            if (modelstoInsert.Any())
            {
                foreach (var singleModelToInsert in modelstoInsert)
                {
                    var videoIndexerPersonModelId = Guid.Parse(singleModelToInsert.PersonModelId);
                    var videoIndexerPersonId = Guid.Parse(singleModelToInsert.Id);
                    var sampleFaceId = Guid.Parse(singleModelToInsert.SampleFaceId);
                    var sampleFacePictureBytes = await azureVideoIndexerService
                        .GetCustomFacePictureAsync(videoIndexerPersonModelId, videoIndexerPersonId, sampleFaceId, cancellationToken);
                    string relativePath = $"Persons/{videoIndexerPersonId}/Faces/{sampleFaceId}/SampleFace.jpg";
                    MemoryStream sampleFaceStream = new(sampleFacePictureBytes);
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

        public async Task<string[]> GetDatabaseProcessingVideosIdsAsync(string accountId, CancellationToken cancellationToken)
        {
            return await this.FairplaytubeDatabaseContext.VideoInfo.Where(p =>
            p.AccountId.ToString() == accountId &&
            p.VideoIndexStatusId ==
            (short)Common.Global.Enums.VideoIndexStatus.Processing)
                .Select(p => p.VideoId).ToArrayAsync(cancellationToken: cancellationToken);
        }

        public async Task<string> GetVideoEditAccessTokenAsync(string accountId, string videoId)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            var accessToken = await azureVideoIndexerService.GetVideoAccessTokenStringAsync(videoId, allowEdit: true);
            return accessToken;
        }

        public async Task BuyVideoAccessAsync(string azureAdB2CObjectId, string videoId, CancellationToken cancellationToken)
        {
            var videoAccessEntity = await this.FairplaytubeDatabaseContext.VideoAccessTransaction
                .Where(p => p.VideoInfo.VideoId == videoId &&
                p.BuyerApplicationUser.AzureAdB2cobjectId.ToString() == azureAdB2CObjectId)
                .SingleOrDefaultAsync(cancellationToken: cancellationToken);
            if (videoAccessEntity != null)
                throw new CustomValidationException("User has already bought access to the specified video");
            var userEntity = await this.FairplaytubeDatabaseContext.ApplicationUser
                .SingleAsync(p => p.AzureAdB2cobjectId.ToString() == azureAdB2CObjectId, cancellationToken: cancellationToken);
            var videoInfoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .SingleAsync(p => p.VideoId == videoId, cancellationToken: cancellationToken);
            var videoPrice = videoInfoEntity.Price;
            var userAvailableFunds = userEntity.AvailableFunds;
            var commissionToApply = Common.Global.Constants.Commissions.VideoAccess;
            var totalPrice = videoPrice + (videoPrice * commissionToApply);
            if (userAvailableFunds < totalPrice)
            {
                string message = $"User {azureAdB2CObjectId} cannot buy access to video: {videoId}. Total Price: {totalPrice}. Available Funds: {userAvailableFunds}";
                throw new CustomValidationException(message);
            }
            await this.FairplaytubeDatabaseContext.VideoAccessTransaction.AddAsync(new VideoAccessTransaction()
            {
                AppliedPrice = videoPrice,
                BuyerApplicationUserId = userEntity.ApplicationUserId,
                AppliedCommission = commissionToApply,
                TotalPrice = totalPrice,
                VideoInfoId = videoInfoEntity.VideoInfoId
            }, cancellationToken);
            userEntity.AvailableFunds -= totalPrice;
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<SearchVideosResponse> GetVideoIndexerStatusAsync(string accountId, string[] videoIds,
            CancellationToken cancellationToken)
        {
            try
            {
                var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
                return await azureVideoIndexerService.SearchVideosByIdsAsync(videoIds, cancellationToken);
            }
            catch(Exception ex)
            {
                ErrorLog errorLog = new()
                {
                    FullException = ex.ToString(),
                    StackTrace = ex.StackTrace,
                    Message = ex.Message
                };
                await this.FairplaytubeDatabaseContext.ErrorLog.AddAsync(errorLog, cancellationToken);
                await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
                throw;
            }
        }

        public async Task<GetVideoIndexResponse> GetVideoIndexerStatusAsync(string accountId, string videoId,
            CancellationToken cancellationToken)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            return await azureVideoIndexerService.GetVideoIndexAsync(videoId, cancellationToken);
        }

        public IQueryable<VideoInfo> GetPublicProcessedVideos()
        {
            return this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.VideoJob)
                .ThenInclude(p => p.VideoJobApplication)
                .Include(p => p.ApplicationUser)
                .ThenInclude(p => p.UserYouTubeChannel)
                .Include(p => p.ApplicationUser.UserExternalMonetization)
                .Include(p => p.VisitorTracking)
                .Where(p =>
            p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processed
            && p.VideoVisibilityId == (short)Common.Global.Enums.VideoVisibility.Public
            )
                .OrderByDescending(p => p.VideoInfoId);
        }

        public IQueryable<VideoInfo> GetVideo(string videoId)
        {
            return this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.VideoJob).Include(p => p.ApplicationUser)
                .ThenInclude(p => p.UserYouTubeChannel)
                .Include(p => p.ApplicationUser.UserExternalMonetization)
                .Include(p => p.VisitorTracking)
                .Where(p => p.VideoId == videoId);
        }

        public async Task<string[]> GetBoughtVideosIdsAsync(string userObjectId, CancellationToken cancellationToken)
        {
            var boughtVideos = this.FairplaytubeDatabaseContext.VideoAccessTransaction
                .Include(p => p.VideoInfo).Include(p => p.BuyerApplicationUser)
                .Where(p => p.BuyerApplicationUser.AzureAdB2cobjectId.ToString() == userObjectId)
                .Select(p => p.VideoInfo);
            var result = await boughtVideos.Select(p => p.VideoId).ToArrayAsync(cancellationToken);
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
            var (accountId, location) = await GetMostSuitableVideoIndexerServerConfigurationAsync();
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
                    throw new CustomValidationException("Please make sure the source file has a valid extension like .mp4");
            }
            var existentVideoName = await this.FairplaytubeDatabaseContext.VideoInfo
                .SingleOrDefaultAsync(p =>
            p.AccountId.ToString() == accountId &&
            p.Location == location &&
            p.Name == uploadVideoModel.Name, cancellationToken: cancellationToken);
            if (existentVideoName != null)
                throw new CustomValidationException($"Unable to use the Name: {uploadVideoModel.Name}. Please use another name and try again");

            if (uploadVideoModel.UseSourceUrl)
            {
                byte[] fileBytes = null;
                if (!String.IsNullOrWhiteSpace(uploadVideoModel.SourceUrl))
                {
                    fileBytes = await this.CustomHttpClient
                        .GetByteArrayAsync(uploadVideoModel.SourceUrl, cancellationToken);
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
                .SingleAsync(p => p.AzureAdB2cobjectId.ToString() == userAzueAdB2cObjectId, cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await this.FairplaytubeDatabaseContext.VideoInfo.AddAsync(new DataAccess.Models.VideoInfo()
            {
                ApplicationUserId = user.ApplicationUserId,
                Description = uploadVideoModel.Description,
                Location = location,
                Name = uploadVideoModel.Name,
                Price = uploadVideoModel.Price,
                //VideoId = indexVideoResponse.id,
                VideoBloblUrl = fileUrl,
                //IndexedVideoUrl = $"https://www.videoindexer.ai/embed/player/{this.AzureVideoIndexerConfiguration.AccountId}" +
                //$"/{indexVideoResponse.id}/" +
                //$"?&locale=en&location={this.AzureVideoIndexerConfiguration.Location}",
                AccountId = Guid.Parse(accountId),
                FileName = newFileName,
                VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Pending,
                VideoLanguageCode = uploadVideoModel.Language,
                VideoVisibilityId = (short)uploadVideoModel.VideoVisibility
            }, cancellationToken: cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken: cancellationToken);
            return true;
        }

        private async Task<(string accountId, string location)>
            GetMostSuitableVideoIndexerServerConfigurationAsync()
        {
            var allAccountsIds = this.VideoIndexerService.GetAllAccountIds(includeDisabledForIndexing:false);
            var usedAccountsIds = await this.FairplaytubeDatabaseContext.VideoInfo.AsNoTracking()
                .Select(p => p.AccountId.ToString().ToLower()).Distinct().ToArrayAsync();
            var accountsWithoutVideos = allAccountsIds.Except(usedAccountsIds);
            if (this.FairplaytubeDatabaseContext.VideoInfo.Count() == 0)
            {
                var viInstance = this.VideoIndexerService.GetByAccountId(accountsWithoutVideos.First());
                return (viInstance.AccountId, viInstance.Location);
            }
            if (accountsWithoutVideos?.Count() > 0)
            {
                var viInstance = this.VideoIndexerService.GetByAccountId(accountsWithoutVideos.First());
                return (viInstance.AccountId, viInstance.Location);
            }
            else
            {
                string accountIdWithLessVideos = string.Empty;
                var videosAccountGroup = this.FairplaytubeDatabaseContext
                    .VideoInfo.AsNoTracking()
                    .Where(p=> allAccountsIds.Contains(p.AccountId.ToString().ToLower()))
                    .GroupBy(p => p.AccountId)
                    .Select(p=> new 
                    {
                        AccountId=p.Key.ToString(),
                        Count = p.Count()
                    }).ToList();
                if (videosAccountGroup?.Count() > 0)
                {
                    var groupWithLessItems = videosAccountGroup.OrderByDescending(p => p.Count).First();
                    if (groupWithLessItems != null)
                    {
                        var viInstance = this.VideoIndexerService
                            .GetByAccountId(groupWithLessItems.AccountId.ToString());
                        return (viInstance.AccountId, viInstance.Location);
                    }
                    else
                    {
                        var viInstance = this.VideoIndexerService.GetByAccountId(accountsWithoutVideos.First());
                        return (viInstance.AccountId, viInstance.Location);
                    }
                }
                else
                {
                    var viInstance = this.VideoIndexerService.GetByAccountId(accountsWithoutVideos.First());
                    return (viInstance.AccountId, viInstance.Location);
                }
            }
        }

        public async Task SaveIndexedVideoKeywordsAsync(string accountId, string videoId, CancellationToken cancellationToken)
        {
            var keywordsResponse = await this.GetIndexedVideoKeywordsAsync(accountId, videoId, cancellationToken);
            if (keywordsResponse.Count > 0)
            {
                var videoInfoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                    .Where(p => p.VideoId == videoId).SingleAsync(cancellationToken: cancellationToken);
                var existentKeywords =
                    this.FairplaytubeDatabaseContext.VideoIndexKeyword
                    .Where(p => p.VideoInfoId == videoInfoEntity.VideoInfoId);
                if (existentKeywords.Any())
                {
                    foreach (var singleExistentKeyword in existentKeywords)
                    {
                        this.FairplaytubeDatabaseContext.VideoIndexKeyword.Remove(singleExistentKeyword);
                    }
                    await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
                }
                foreach (var singleKeyword in keywordsResponse)
                {
                    await this.FairplaytubeDatabaseContext.VideoIndexKeyword.AddAsync(new VideoIndexKeyword()
                    {
                        VideoInfoId = videoInfoEntity.VideoInfoId,
                        Keyword = singleKeyword.Keyword
                    }, cancellationToken);
                }
                await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
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

        public async Task UpdateVideoAsync(string videoId, UpdateVideoModel model)
        {
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .SingleOrDefaultAsync(p => p.VideoId == videoId);
            if (videoEntity == null)
                throw new CustomValidationException($"Unable to find video with Id: {videoId}");
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
                .ToListAsync(cancellationToken: cancellationToken);
            return videosEntitities;
        }

        public async Task SaveVideoThumbnailAsync(string accountId, string videoId, string thumbnailId, CancellationToken cancellationToken)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            var videoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.ApplicationUser).SingleOrDefaultAsync(p => p.VideoId == videoId, cancellationToken: cancellationToken);
            if (videoEntity == null)
                throw new CustomValidationException($"Video: {videoId} does not exit");
            var videoThumbnailBase64 = await azureVideoIndexerService
                .GetVideoThumbnailAsync(videoId, thumbnailId, cancellationToken);
            string relativePath =
                $"{videoEntity.ApplicationUser.AzureAdB2cobjectId}/Video/{videoId}/Thumbnail/{thumbnailId}.jpg";
            var byteArray = Convert.FromBase64String(videoThumbnailBase64);
            MemoryStream memoryStream = new(byteArray);
            memoryStream.Position = 0;
            var result =
            await this.AzureBlobStorageService.UploadFileAsync(this.DataStorageConfiguration.ContainerName, relativePath,
                memoryStream, true, cancellationToken);
            string videoThumbnailUrl = $"https://{DataStorageConfiguration.AccountName}.blob.core.windows.net/{DataStorageConfiguration.ContainerName}/{relativePath}";
            videoEntity.ThumbnailUrl = videoThumbnailUrl;
            await this.FairplaytubeDatabaseContext.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<PersonModel>> GetAllPersonsAsync(string accountId, CancellationToken cancellationToken)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            List<PersonModel> result = new();
            var allPersonModels = await azureVideoIndexerService.GetAllPersonModelsAsync(cancellationToken);
            var defaultModel = allPersonModels.Single(p => p.isDefault == true);
            Guid defaultModelId = Guid.Parse(defaultModel.id);
            var allPersonsInModel = await azureVideoIndexerService.GetPersonsInModelAsync(defaultModelId, cancellationToken);
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

        public async Task<List<Person>> GetPersistedPersonsAsync(CancellationToken cancellationToken)
        {
            return await this.FairplaytubeDatabaseContext.Person.ToListAsync(cancellationToken);
        }
        public async Task<ProjectModel> CreateCustomRenderingProjectAsync(
            string accountId,
            ProjectModel projectModel, CancellationToken cancellationToken)
        {
            CreateProjectRequest createProjectRequestModel = new()
            {
                name = projectModel.Name,
                videosRanges = projectModel.Videos.Select(p => new PTI.Microservices.Library.AzureVideoIndexer.Models.CreateProject.Videosrange() { videoId = p.VideoId, range = new PTI.Microservices.Library.AzureVideoIndexer.Models.CreateProject.Range() { start = p.Start, end = p.End } }).ToArray()
            };
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            var result = await azureVideoIndexerService.CreateProjectAsync(createProjectRequestModel, cancellationToken: cancellationToken);
            projectModel.ProjectId = result.id;
            return projectModel;
        }

        public async Task<byte[]> DownloadVideoAsync(string accountId, string videoId, CancellationToken cancellationToken)
        {
            var azureVideoIndexerService = this.VideoIndexerService.GetByAccountId(accountId);
            var url = await azureVideoIndexerService.GetVideoSourceFileDownloadUrlAsync(videoId, cancellationToken);
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
