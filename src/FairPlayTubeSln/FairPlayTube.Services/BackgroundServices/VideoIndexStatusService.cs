using FairPlayTube.Common.Global.Enums;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                            var indexVideoResponse =
                            await azureVideoIndexerService.UploadVideoAsync(new Uri(singleVideo.VideoBloblUrl),
                                singleVideo.Name, singleVideo.Description, singleVideo.FileName,
                                personModelId: Guid.Parse(defaultPersonModel.id), privacy: AzureVideoIndexerService.VideoPrivacy.Public,
                                callBackUri: new Uri("https://fairplaytube.com"), cancellationToken: stoppingToken);
                            singleVideo.VideoId = indexVideoResponse.id;
                            singleVideo.IndexedVideoUrl = $"https://www.videoindexer.ai/embed/player/{singleVideo.AccountId}" +
                                $"/{indexVideoResponse.id}/" +
                                $"?&locale=en&location={singleVideo.Location}";
                            singleVideo.VideoIndexStatusId = (short)Common.Global.Enums.VideoIndexStatus.Processing;
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
                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }

        private static async Task CheckProcessingVideos(VideoService videoService, CancellationToken stoppingToken)
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
                        await videoService.UpdateVideoIndexStatusAsync(indexCompleteVideos.Select(p => p.id).ToArray(),
                            Common.Global.Enums.VideoIndexStatus.Processed,
                            cancellationToken: stoppingToken);
                        //foreach (var singleIndexedVideo in indexCompleteVideos)
                        //{
                        //    await videoService.SaveIndexedVideoKeywordsAsync(singleIndexedVideo.id, stoppingToken);
                        //}
                    }
                }
            }
        }
    }
}