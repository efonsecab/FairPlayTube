using FairPlayTube.Services.Configuration;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class VideoIndexerService
    {
        private AzureVideoIndexerService[] AzureVideoIndexerServices { get; }
        public ExtendedAzureVideoIndexerConfiguration[] ExtendedAzureVideoIndexerConfiguration { get; }
        private string[] AccountsDisabledForIndexing => ExtendedAzureVideoIndexerConfiguration
            .Where(p => p.IsDisabledForIndexing == true).Select(p => p.AccountId).ToArray();

        public VideoIndexerService(AzureVideoIndexerService[] azureVideoIndexerServices,
            ExtendedAzureVideoIndexerConfiguration[] extendedAzureVideoIndexerConfigurations)
        {
            this.AzureVideoIndexerServices = azureVideoIndexerServices;
            this.ExtendedAzureVideoIndexerConfiguration = extendedAzureVideoIndexerConfigurations;
        }

        public AzureVideoIndexerService GetByAccountId(string accountId)
        {
            return this.AzureVideoIndexerServices.Single(p => p.AccountId.ToLower() == accountId.ToLower());
        }

        public string[] GetAllAccountIds(bool includeDisabledForIndexing)
        {
            if (includeDisabledForIndexing)
                return this.AzureVideoIndexerServices.Select(p => p.AccountId.ToLower()).ToArray();
            else
            {
                return this.AzureVideoIndexerServices
                    .Where(p => this.AccountsDisabledForIndexing?.Contains(p.AccountId) == false)
                    .Select(p=>p.AccountId)
                    .ToArray();
            }
        }
    }
}
