using FairPlayTube.DataAccess.Data;
using FairPlayTube.Notifications.Hubs;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Enabled the callback to be invoked from Azure Video Indexer
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AzureVideoIndexerController : ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private VideoService VideoService { get; }
        private IHubContext<NotificationHub, INotificationHub> HubContext { get; }

        /// <summary>
        /// Initializes <see cref="AzureVideoIndexerController"/>
        /// </summary>
        /// <param name="fairplaytubeDatabaseContext"></param>
        /// <param name="videoService"></param>
        /// <param name="hubContext"></param>
        public AzureVideoIndexerController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            VideoService videoService, IHubContext<NotificationHub, INotificationHub> hubContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.VideoService = videoService;
            this.HubContext = hubContext;
        }

        /// <summary>
        /// Invoked by Azure Video Indexer when a video has been indxed
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
                    await VideoService.SaveIndexedVideoKeywordsAsync(id, cancellationToken);
                    await VideoService.SaveVideoThumbnailAsync(id, videoIndex.videos.First().thumbnailId, cancellationToken);
                    videoInfoEntity.VideoIndexStatusId = (int)Common.Global.Enums.VideoIndexStatus.Processed;
                    videoInfoEntity.VideoIndexSourceClass = this.GetType().FullName;
                    videoInfoEntity.VideoDurationInSeconds = videoIndex.summarizedInsights.duration.seconds;
                    await this.FairplaytubeDatabaseContext.SaveChangesAsync();
                }
            }
        }
    }
}
