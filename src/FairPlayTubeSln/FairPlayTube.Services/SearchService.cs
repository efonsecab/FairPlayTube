using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class SearchService
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }

        public SearchService(FairplaytubeDatabaseContext fairplaytubeDatabaseContext)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
        }

        public IQueryable<VideoInfo> SearchPublicProcessedVideos(string searchTerm)
        {
            var result = this.FairplaytubeDatabaseContext.VideoInfo
                .Include(p => p.VideoIndexKeyword)
                .Include(p => p.VideoJob)
                .Include(p => p.ApplicationUser)
                .Where(p =>
                (p.VideoIndexStatusId == (short)Common.Global.Enums.VideoIndexStatus.Processed)
                && 
                (p.Description.Contains(searchTerm) || 
                p.Name.Contains(searchTerm) && 
                (p.VideoIndexKeyword.Any(k => k.Keyword == searchTerm)
                && p.VideoVisibilityId == 
                (short)Common.Global.Enums.VideoVisibility.Public)));
            return result;
        }
    }
}
