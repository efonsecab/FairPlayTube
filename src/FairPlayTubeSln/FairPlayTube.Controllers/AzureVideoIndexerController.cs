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

        /// <summary>
        /// Initializes <see cref="AzureVideoIndexerController"/>
        /// </summary>
        /// <param name="fairplaytubeDatabaseContext"></param>
        /// <param name="videoService"></param>
        public AzureVideoIndexerController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            VideoService videoService)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.VideoService = videoService;
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
                    .SingleOrDefaultAsync(cancellationToken: cancellationToken);
                if (videoInfoEntity != null)
                {
                    await this.VideoService.AddVideoIndexTransactionsAsync(new string[] { videoInfoEntity.VideoId }, cancellationToken);
                    await this.VideoService.MarkVideoAsProcessed(videoInfoEntity, cancellationToken);
                }
            }
        }
    }
}
