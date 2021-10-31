using FairPlayTube.Models.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
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

        public async Task LoadData()
        {
            var anonymousHttpClient = this.HttpClientService.CreateAnonymousClient();
            this.AllResources = await anonymousHttpClient.GetFromJsonAsync<ResourceModel[]>(
                $"{ApiRoutes.LocalizationController.GetAllResources}");
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
