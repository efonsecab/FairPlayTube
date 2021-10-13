using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using Microsoft.Azure.CognitiveServices.Search.VideoSearch.Models;
using Microsoft.EntityFrameworkCore;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class SearchService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }

        private AzureBingSearchService AzureBingSearchService { get; }

        public SearchService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext, 
            AzureBingSearchService azureBingSearchService)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.AzureBingSearchService = azureBingSearchService;
        }

        public IQueryable<VideoInfo> SearchPublicProcessedVideos(string searchTerm)
        {
            var result = this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.VideoIndexKeyword)
                .Include(p => p.VideoJob)
                .Include(p => p.ApplicationUser)
                .Where(p =>
                p.Description.Contains(searchTerm) || p.Name.Contains(searchTerm)
                && (p.VideoIndexKeyword.Any(k => k.Keyword == searchTerm)
                || p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processed
                && p.VideoVisibilityId == (short)Common.Global.Enums.VideoVisibility.Public));
            return result;
        }

        public async Task<Videos> SearchBingVideo(string searchTerm, CancellationToken cancellationToken)
        {
            return await this.AzureBingSearchService.SearchVideosAsync(searchTerm,
                AzureBingSearchService.SafeSearchMode.Strict, itemsToRetrieve: 100, cancellationToken: cancellationToken);
        }
    }
}
