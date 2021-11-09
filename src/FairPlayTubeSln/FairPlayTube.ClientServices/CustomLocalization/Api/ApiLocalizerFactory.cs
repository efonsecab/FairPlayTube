using FairPlayTube.ClientServices;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomLocalization.Api
{
    public class ApiLocalizerFactory : IStringLocalizerFactory
    {
        private readonly LocalizationClientService _localizationClientService;

        public ApiLocalizerFactory(LocalizationClientService localizationClientService)
        {
            this._localizationClientService = localizationClientService;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            var localizerType = typeof(ApiLocalizer<>)
                     .MakeGenericType(resourceSource);
            var instance = Activator.CreateInstance(localizerType, args: new object[] { _localizationClientService }) as IStringLocalizer;
            return instance;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new ApiLocalizer(_localizationClientService);
        }
    }
}
