using FairPlayTube.Models.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static FairPlayTube.Common.Global.Constants;

namespace FairPlayTube.ClientServices
{
    public class LocalizationClientService
    {
        private HttpClientService HttpClientService { get; }
        private ResourceModel[] AllResources { get; set; }
        public LocalizationClientService(HttpClientService httpClientService)
        {
            this.HttpClientService = httpClientService;
        }

        public async Task LoadDataAsync()
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            this.AllResources = await anonymousHttpClient.GetFromJsonAsync<ResourceModel[]>(
                $"{ApiRoutes.LocalizationController.GetAllResources}");
        }

        public async Task<CultureModel[]> GetSupportedCulturesAsync()
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            return await anonymousHttpClient.GetFromJsonAsync<CultureModel[]>(
                $"{ApiRoutes.LocalizationController.GetSupportedCultures}");
        }

        public IEnumerable<LocalizedString> GetAllStrings()
        {
            return this.AllResources?.Select(p => new LocalizedString(p.Key, p.Value));
        }
        public string GetString(string typeName, string key)
        {
            return this.AllResources?.SingleOrDefault(p => p.Type == typeName &&
            p.Key == key)?.Value;
        }
    }
}
