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
            using (var scope = this.ServiceScopeFactory.CreateScope())
            {
                var videoService = scope.ServiceProvider.GetRequiredService<VideoService>();
                var azureVideoIndexerService = scope.ServiceProvider.GetRequiredService<AzureVideoIndexerService>();
                FairplaytubeDatabaseContext fairplaytubeDatabaseContext = scope.ServiceProvider.GetRequiredService<FairplaytubeDatabaseContext>();
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var videoIndexerBaseCallbackUrl = config["VideoIndexerCallbackUrl"];
                var videoIndexerCallbackUrl = $"{videoIndexerBaseCallbackUrl}/api/AzureVideoIndexer/OnVideoIndexed";
                var indexingPreset = "Advanced"; //TODO: Temporaily set to show capabilities, later this needs to has business logic
                while (!stoppingToken.IsCancellationRequested)
                {
                    //Check https://stackoverflow.com/questions/48368634/how-should-i-inject-a-dbcontext-instance-into-an-ihostedservice
                    try
                    {
                        await CheckProcessingVideos(videoService, stoppingToken);
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
                            await fairplaytubeDatabaseContext.SaveChangesAsync();
                        }
                        try
                        {
                            var allVideoIndexerPersons = await videoService.GetAllPersonsAsync(stoppingToken);
                            await videoService.SavePersonsAsync(personsModels: allVideoIndexerPersons, cancellationToken: stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            fairplaytubeDatabaseContext.ChangeTracker.Clear();
                            await fairplaytubeDatabaseContext.ErrorLog.AddAsync(new ErrorLog()
                            {
                                FullException = ex.ToString(),
                                StackTrace = ex.StackTrace,
                                Message = ex.Message
                            });
                            await fairplaytubeDatabaseContext.SaveChangesAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            fairplaytubeDatabaseContext.ChangeTracker.Clear();
                            await fairplaytubeDatabaseContext.ErrorLog.AddAsync(new ErrorLog()
                            {
                                FullException = ex.ToString(),
                                StackTrace = ex.StackTrace,
                                Message = ex.Message
                            });
                            await fairplaytubeDatabaseContext.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            //TODO: Add Email Notification
                        }
                    }
                    await Task.Delay(TimeSpan.FromMinutes(5));
                }
            }
        }

        private async Task CheckProcessingVideos(VideoService videoService, CancellationToken stoppingToken)
        {
            var processingInDB = await videoService.GetDatabaseProcessingVideosIdsAsync(stoppingToken);
            if (processingInDB.Length > 0)
            {
                var videosIndex = await videoService.GetVideoIndexerStatus(processingInDB, stoppingToken);
                if (videosIndex.results.Length > 0)
                {
                    var indexCompleteVideos = videosIndex.results.Where(p =>
                    p.state == Common.Global.Enums.VideoIndexStatus.Processed.ToString());
                    if (indexCompleteVideos.Count() > 0)
                    {
                        var indexCompleteVideosIds = indexCompleteVideos.Select(p => p.id).ToArray();

                        await videoService.AddVideoIndexTransactionsAsync(indexCompleteVideosIds, stoppingToken);

                        foreach (var singleIndexedVideo in indexCompleteVideos)
                        {
                            await videoService.SaveIndexedVideoKeywordsAsync(singleIndexedVideo.id, stoppingToken);
                        }
                        foreach (var singleIndexedVideo in indexCompleteVideos)
                        {
                            await videoService.SaveVideoThumbnailAsync(singleIndexedVideo.id, singleIndexedVideo.thumbnailId, stoppingToken);
                        }
                        await videoService.UpdateVideoIndexStatusAsync(indexCompleteVideosIds,
                            Common.Global.Enums.VideoIndexStatus.Processed,
                            cancellationToken: stoppingToken);
                    }
                }
            }
        }
    }
}