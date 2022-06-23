using FairPlayTube.Common.Global;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace FairPlayTube.Services.BackgroundServices
{
    public class VideoIndexStatusService : BackgroundService
    {
        private ILogger<VideoIndexStatusService> Logger { get; }
        private IServiceScopeFactory ServiceScopeFactory { get; }

        public VideoIndexStatusService(ILogger<VideoIndexStatusService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.Logger = logger;
            this.ServiceScopeFactory = serviceScopeFactory;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = this.ServiceScopeFactory.CreateScope();
            var videoService = scope.ServiceProvider.GetRequiredService<VideoService>();
            var videoIndexerService = scope.ServiceProvider.GetRequiredService<VideoIndexerService>();
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext = scope.ServiceProvider.GetRequiredService<FairplaytubeDatabaseContext>();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var videoIndexerAccountsIds = videoIndexerService.GetAllAccountIds();
            var videoIndexerBaseCallbackUrl = config["VideoIndexerCallbackUrl"];
            var videoIndexerCallbackUrl = $"{videoIndexerBaseCallbackUrl}/api/AzureVideoIndexer/OnVideoIndexed";
            //TODO: Temporaily set to show capabilities, later this needs to has business logic
            //Allowed values: "Default", "Advanced"
            var indexingPreset = "Default";
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var singleVideoIndexerAccountId in videoIndexerAccountsIds)
                {
                    //Check https://stackoverflow.com/questions/48368634/how-should-i-inject-a-dbcontext-instance-into-an-ihostedservice
                    try
                    {
                        var azureVideoIndexerService = videoIndexerService.GetByAccountId(singleVideoIndexerAccountId);
                        await CheckProcessingVideosAsync(azureVideoIndexerService.AccountId, videoService, fairplaytubeDatabaseContext, stoppingToken);
                        var pendingIndexingVideos = fairplaytubeDatabaseContext.VideoInfo.Where(p => p.VideoIndexStatusId ==
                        (short)Common.Global.Enums.VideoIndexStatus.Pending)
                            .OrderBy(p => p.VideoInfoId)
                            .Take(50);
                        foreach (var singleVideo in pendingIndexingVideos)
                        {
                            var allPersonModels = await azureVideoIndexerService.GetAllPersonModelsAsync(stoppingToken);
                            var defaultPersonModel = allPersonModels.Single(p => p.isDefault == true);
                            stoppingToken.ThrowIfCancellationRequested();
                            string encodedName = HttpUtility.UrlEncode(singleVideo.Name);
                            string encodedDescription = HttpUtility.UrlEncode(singleVideo.Description);
                            var indexVideoResponse =
                            await azureVideoIndexerService.UploadVideoAsync(new Uri(singleVideo.VideoBloblUrl),
                                encodedName, encodedDescription, singleVideo.FileName,
                                personModelId: Guid.Parse(defaultPersonModel.id), privacy: AzureVideoIndexerService.VideoPrivacy.Public,
                                callBackUri: new Uri(videoIndexerCallbackUrl),
                                language: singleVideo.VideoLanguageCode,
                                indexingPreset: indexingPreset,
                                cancellationToken: stoppingToken);
                            singleVideo.VideoId = indexVideoResponse.id;
                            singleVideo.IndexedVideoUrl = $"https://www.videoindexer.ai/embed/player/{singleVideo.AccountId}" +
                                $"/{indexVideoResponse.id}/" +
                                $"?&locale=en&location={singleVideo.Location}";
                            singleVideo.VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Processing;
                            await fairplaytubeDatabaseContext.SaveChangesAsync(stoppingToken);
                        }
                        try
                        {
                            var allVideoIndexerPersons = await videoService.GetAllPersonsAsync(azureVideoIndexerService.AccountId,stoppingToken);
                            await videoService.SavePersonsAsync(azureVideoIndexerService.AccountId, personsModels: allVideoIndexerPersons, cancellationToken: stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            fairplaytubeDatabaseContext.ChangeTracker.Clear();
                            await fairplaytubeDatabaseContext.ErrorLog.AddAsync(new ErrorLog()
                            {
                                FullException = ex.ToString(),
                                StackTrace = ex.StackTrace,
                                Message = ex.Message
                            }, stoppingToken);
                            await fairplaytubeDatabaseContext.SaveChangesAsync(stoppingToken);
                        }
                        var detailsPagePattern = Constants.PublicVideosPages.Details.Replace("{VideoId}", String.Empty);
                        var detailsPagesWithPendingVideoId =
                            fairplaytubeDatabaseContext.VisitorTracking.Where(p => p.VideoInfoId == null &&
                            p.VisitedUrl.Contains(detailsPagePattern));
                        foreach (var singleVisitedPage in detailsPagesWithPendingVideoId)
                        {
                            var pageUri = new Uri(singleVisitedPage.VisitedUrl);
                            var lastSegment = pageUri.Segments.Last().TrimEnd('/');
                            if (!String.IsNullOrWhiteSpace(lastSegment))
                            {
                                var videoInfoEntity = fairplaytubeDatabaseContext.VideoInfo.SingleOrDefault(p => p.VideoId == lastSegment);
                                if (videoInfoEntity != null)
                                {
                                    singleVisitedPage.VideoInfoId = videoInfoEntity.VideoInfoId;
                                }
                            }
                        }
                        await fairplaytubeDatabaseContext.SaveChangesAsync(stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        this.Logger?.LogError(exception: ex, message: ex.Message);
                        try
                        {
                            fairplaytubeDatabaseContext.ChangeTracker.Clear();
                            await fairplaytubeDatabaseContext.ErrorLog.AddAsync(new ErrorLog()
                            {
                                FullException = ex.ToString(),
                                StackTrace = ex.StackTrace,
                                Message = ex.Message
                            }, stoppingToken);
                            await fairplaytubeDatabaseContext.SaveChangesAsync(stoppingToken);
                        }
                        catch (Exception ex1)
                        {
                            this.Logger?.LogError(exception: ex1, message: ex1.Message);
                            //TODO: Add Email Notification
                        }
                    }
                }
                this.Logger?.LogInformation($"{nameof(VideoIndexStatusService)} waiting for 5 minutes.");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CheckProcessingVideosAsync(string accountId,VideoService videoService, FairplaytubeDatabaseContext fairplaytubeDatabaseContext, CancellationToken stoppingToken)
        {
            this.Logger?.LogInformation("Checking processing videos");
            var processingInDB = await videoService.GetDatabaseProcessingVideosIdsAsync(accountId,stoppingToken);
            string message = $"{processingInDB.Length} videos marked as processing In DB";
            this.Logger?.LogInformation(message);
            if (processingInDB.Length > 0)
            {

                this.Logger?.LogInformation("Retrieving videos status in Azure Video Indexer");
                var videosIndex = await videoService.GetVideoIndexerStatusAsync(accountId,processingInDB, stoppingToken);
                if (videosIndex.results.Length > 0)
                {
                    var indexCompleteVideos = videosIndex.results.Where(p =>
                    p.state == Common.Global.Enums.VideoIndexStatus.Processed.ToString());
                    if (indexCompleteVideos.Any())
                    {
                        var indexCompleteVideosIds = indexCompleteVideos.Select(p => p.id).ToArray();
                        await videoService.AddVideoIndexTransactionsAsync(indexCompleteVideosIds, stoppingToken);
                        var videosCompleted = fairplaytubeDatabaseContext.VideoInfo.Where(video => indexCompleteVideosIds.Contains(video.VideoId));

                        foreach (var video in videosCompleted)
                            await videoService.MarkVideoAsProcessedAsync(video, stoppingToken);
                    }
                }
            }
        }
    }
}