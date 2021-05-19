using PTI.Microservices.Library.Models.AzureVideoIndexerService.GetAllVideos;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class VideoService
    {
        private AzureVideoIndexerService AzureVideoIndexerService { get; }
        public VideoService(AzureVideoIndexerService azureVideoIndexerService)
        {
            this.AzureVideoIndexerService = azureVideoIndexerService;
        }

        public async Task<VideoInfo[]> GetPublicProcessedVideosAsync(CancellationToken cancellationToken = default)
        {
            var allVideos = await this.AzureVideoIndexerService.GetAllVideosAsync(cancellationToken: cancellationToken);
            return allVideos.results.Where(p => p.privacyMode == "Public" && p.state == "Processed").ToArray();
        }
    }
}
