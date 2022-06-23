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
        private AzureVideoIndexerService[] AzureVideoIndexerServices { get;}
        public VideoIndexerService(AzureVideoIndexerService[] azureVideoIndexerServices)
        {
            this.AzureVideoIndexerServices = azureVideoIndexerServices;
        }

        public AzureVideoIndexerService GetByAccountId(string accountId)
        {
            return this.AzureVideoIndexerServices.Single(p => p.AccountId.ToLower() == accountId.ToLower());
        }

        public string[] GetAllAccountIds()
        {
            return this.AzureVideoIndexerServices.Select(p => p.AccountId.ToLower()).ToArray();
        }
    }
}
