using PTI.Microservices.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Services.Configuration
{

    public class ExtendedAzureVideoIndexerConfiguration: AzureVideoIndexerConfiguration
    {
        public bool IsDisabledForIndexing { get; set; }
    }
}
