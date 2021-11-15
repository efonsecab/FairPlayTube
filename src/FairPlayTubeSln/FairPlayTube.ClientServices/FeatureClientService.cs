using FairPlayTube.Models.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static FairPlayTube.Common.Global.Constants;

namespace FairPlayTube.ClientServices
{
    public class FeatureClientService
    {
        private HttpClientService HttpClientService { get; }
        private static FeatureModel[] AllFeatures { get; set; }

        public FeatureClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task LoadAllFeaturesAsync()
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            AllFeatures = await anonymousHttpClient.GetFromJsonAsync<FeatureModel[]>(
                ApiRoutes.FeatureController.GetAllFeatures);
        }

        public static bool IsFeatureEnabled(FairPlayTube.Common.Global.Enums.FeatureType feature)
        {
            var match = AllFeatures?.SingleOrDefault(p => p.Name == feature.ToString());
            return (match != null && match.IsEnabled == true);
        }
    }
}
