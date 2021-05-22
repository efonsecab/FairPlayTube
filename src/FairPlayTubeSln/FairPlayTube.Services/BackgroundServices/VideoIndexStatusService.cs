using FairPlayTube.Common.Global.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
            while (!stoppingToken.IsCancellationRequested)
            {
                //Check https://stackoverflow.com/questions/48368634/how-should-i-inject-a-dbcontext-instance-into-an-ihostedservice
                using (var scope = this.ServiceScopeFactory.CreateScope())
                {
                    var videoService = scope.ServiceProvider.GetRequiredService<VideoService>();
                    var pendingInDbVideos = await videoService.GetDatabaseProcessingVideosIdsAsync(stoppingToken);
                    if (pendingInDbVideos.Length > 0)
                    {
                        var videosIndex = await videoService.GetVideoIndexerStatus(pendingInDbVideos, stoppingToken);
                        if (videosIndex.results.Length > 0)
                        {
                            var indexCompleteVideos = videosIndex.results.Where(p =>
                            p.state == VideoIndexStatus.Processed.ToString());
                            if (indexCompleteVideos.Count() > 0)
                            {
                                await videoService.UpdateVideoIndexStatusAsync(indexCompleteVideos.Select(p => p.id).ToArray(), VideoIndexStatus.Processed,
                                    cancellationToken: stoppingToken);
                            }
                        }
                    }
                }
                await Task.Delay(TimeSpan.FromMinutes(1));
            }
        }
    }
}
