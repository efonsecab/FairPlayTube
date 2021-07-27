using FairPlayTube.DataAccess.Data;
using FairPlayTube.Models.Notifications;
using FairPlayTube.Notifications.Hubs;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AzureVideoIndexerController : ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private VideoService VideoService { get; }
        private IHubContext<NotificationHub, INotificationHub> HubContext { get; }

        public AzureVideoIndexerController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            VideoService videoService, IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.VideoService = videoService;
            this.HubContext = hubContext;
        }

        [HttpPost("[action]")]
        public async Task OnVideoIndexed(string id, string state, CancellationToken cancellationToken)
        {
            if (state == Common.Global.Enums.VideoIndexStatus.Processed.ToString())
            {
                var videoInfoEntity = await this.FairplaytubeDatabaseContext.VideoInfo
                    .Where(p => p.VideoId == id)
                    .SingleOrDefaultAsync();
                if (videoInfoEntity != null)
                {
                    var videoIndex = await VideoService.GetVideoIndexerStatus(id, cancellationToken);
                    videoInfoEntity.VideoIndexStatusId = (int)Common.Global.Enums.VideoIndexStatus.Processed;
                    videoInfoEntity.VideoIndexSourceClass = this.GetType().FullName;
                    videoInfoEntity.VideoDurationInSeconds = videoIndex.summarizedInsights.duration.seconds;
                    await this.FairplaytubeDatabaseContext.SaveChangesAsync();
                    await VideoService.SaveIndexedVideoKeywordsAsync(id, cancellationToken);
                    await VideoService.SaveVideoThumbnailAsync(id, videoIndex.videos.First().thumbnailId, cancellationToken);
                }
            }
        }
    }
}
