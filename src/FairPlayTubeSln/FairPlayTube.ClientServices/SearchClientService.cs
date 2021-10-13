using FairPlayTube.Models.BingSearch;
using FairPlayTube.Models.Video;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static FairPlayTube.Common.Global.Constants;

namespace FairPlayTube.ClientServices
{
    public class SearchClientService
    {
        private HttpClientService HttpClientService { get; }
        public SearchClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task<VideoInfoModel[]> SearchPublicProcessedVideosAsync(string searchTerm)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<VideoInfoModel[]>(
                $"{ApiRoutes.SearchController.SearchPublicProcessedVideos}" +
                $"?searchTerm={WebUtility.UrlEncode(searchTerm)}");
        }

        public async Task<BingSearchVideoModel[]> SearchBingVideosAsync(string searchTerm)
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<BingSearchVideoModel[]>(
                $"{ApiRoutes.SearchController.SearchBingVideos}" +
                $"?searchTerm={WebUtility.UrlEncode(searchTerm)}");
        }
    }
}
