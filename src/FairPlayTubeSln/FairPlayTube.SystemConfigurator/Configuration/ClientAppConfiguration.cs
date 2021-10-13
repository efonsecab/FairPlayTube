using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.SystemConfigurator.Configuration
{

    public class ClientAppConfiguration
    {
        [ValidateComplexType]
        public Azureadb2cscopes AzureAdB2CScopes { get; set; }
        [ValidateComplexType]
        public ClientAppAzureadb2c AzureAdB2C { get; set; }
        [ValidateComplexType]
        public Azureqnabotconfiguration AzureQnABotConfiguration { get; set; }
        [ValidateComplexType]
        public Displayresponsiveadconfiguration DisplayResponsiveAdConfiguration { get; set; }
        [ValidateComplexType]
        public Facebooklikebuttonconfiguration FaceBookLikeButtonConfiguration { get; set; }
    }

    public class Azureadb2cscopes
    {
        [Required]
        public string DefaultScope { get; set; }
    }

    public class ClientAppAzureadb2c
    {
        [Required]
        public string Authority { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public bool ValidateAuthority { get; set; }
    }

    public class Azureqnabotconfiguration
    {
        [Required]
        public string Key { get; set; }
    }

    public class Displayresponsiveadconfiguration
    {
        [Required]
        public string AdClient { get; set; }
        [Required]
        public string AdSlot { get; set; }
    }

    public class Facebooklikebuttonconfiguration
    {
        [Required]
        public string AppId { get; set; }
    }

}
