using FairPlayTube.DataAccess.Data;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AzureVideoIndexerController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            VideoService videoService)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.VideoService = videoService;
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
                    videoInfoEntity.VideoIndexStatusId = (int)Common.Global.Enums.VideoIndexStatus.Processed;
                    await this.FairplaytubeDatabaseContext.SaveChangesAsync();
                    await VideoService.SaveIndexedVideoKeywordsAsync(id, cancellationToken);
                }
            }
        }
    }
}
