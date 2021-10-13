using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.SystemConfigurator.Configuration
{

    public class ServerConfiguration
    {
        [ValidateComplexType]
        public Azureadb2c AzureAdB2C { get; set; }
        [ValidateComplexType]
        public Azureconfiguration AzureConfiguration { get; set; }
        [ValidateComplexType]
        public Azurecontentmoderatorconfiguration AzureContentModeratorConfiguration { get; set; }
        [ValidateComplexType]
        public Azuretextanalyticsconfiguration AzureTextAnalyticsConfiguration { get; set; }
        [ValidateComplexType]
        public Connectionstrings ConnectionStrings { get; set; }
        [ValidateComplexType]
        public Datastorageconfiguration DataStorageConfiguration { get; set; }
        public bool EnableSwaggerUI { get; set; } = false;
        [ValidateComplexType]
        public Ipstackconfiguration IpStackConfiguration { get; set; }
        [ValidateComplexType]
        public Paypalconfiguration PaypalConfiguration { get; set; }
        [Required]
        public string RapidApiKey { get; set; }
        [ValidateComplexType]
        public Smtpconfiguration SmtpConfiguration { get; set; }
        public bool UseHttpsRedirection { get; set; } = true;
        [Required]
        [Url]
        public string VideoIndexerCallbackUrl { get; set; }
    }

    public class Azureadb2c
    {
        [Required]
        public string ClientAppClientId { get; set; }
        [Required]
        public string ClientAppDefaultScope { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string Domain { get; set; }
        [Required]
        public string Instance { get; set; }
        [Required]
        public string SignUpSignInPolicyId { get; set; }
    }

    public class Azureconfiguration
    {
        [ValidateComplexType]
        public Azureblobstorageconfiguration AzureBlobStorageConfiguration { get; set; }
        [ValidateComplexType]
        public Azurevideoindexerconfiguration AzureVideoIndexerConfiguration { get; set; }
    }

    public class Azureblobstorageconfiguration
    {
        [Required]
        public string ConnectionString { get; set; }
    }

    public class Azurevideoindexerconfiguration
    {
        [Required]
        public string AccountId { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Location { get; set; }
    }

    public class Azurecontentmoderatorconfiguration
    {
        [Required]
        public string Endpoint { get; set; }
        [Required]
        public string Key { get; set; }
    }

    public class Azuretextanalyticsconfiguration
    {
        [Required]
        public string Endpoint { get; set; }
        [Required]
        public string Key { get; set; }
    }

    public class Connectionstrings
    {
        [Required]
        public string Default { get; set; }
    }

    public class Datastorageconfiguration
    {
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string ContainerName { get; set; }
        [Required]
        public string UntrustedUploadsContainerName { get; set; }
    }

    public class Ipstackconfiguration
    {
        [Required]
        public string Key { get; set; }
    }

    public class Paypalconfiguration
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string Endpoint { get; set; }
        [Required]
        public string Secret { get; set; }
    }

    public class Smtpconfiguration
    {
        [Required]
        public int Port { get; set; }
        [Required]
        public string SenderDisplayName { get; set; }
        [Required]
        public string SenderEmail { get; set; }
        [Required]
        public string SenderPassword { get; set; }
        [Required]
        public string SenderUsername { get; set; }
        [Required]
        public string Server { get; set; }
        [Required]
        public bool UseSSL { get; set; }
    }

}
