using AutoMapper;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models.Video;
using FairPlayTube.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all of the search functionality
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private FairplaytubeDatabaseContext FairplaytubeDatabaseContext { get; }
        private SearchService SearchService { get; }
        private IMapper Mapper { get; }

        /// <summary>
        /// Initializes <see cref="SearchController"/>
        /// </summary>
        /// <param name="fairplaytubeDatabaseContext"></param>
        /// <param name="searchService"></param>
        /// <param name="mapper"></param>
        public SearchController(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            SearchService searchService, IMapper mapper)
        {
            this.FairplaytubeDatabaseContext = fairplaytubeDatabaseContext;
            this.SearchService = searchService;
            this.Mapper = mapper;
        }

        /// <summary>
        /// Searches for public processed videos having the given searchTerm
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<VideoInfoModel[]> SearchPublicProcessedVideos(string searchTerm, CancellationToken cancellationToken)
        {
            var result = await this.SearchService.SearchPublicProcessedVideos(searchTerm)
               .Select(p => this.Mapper.Map<VideoInfo, VideoInfoModel>(p))
               .ToArrayAsync(cancellationToken: cancellationToken);
            return result;
        }
    }
}
