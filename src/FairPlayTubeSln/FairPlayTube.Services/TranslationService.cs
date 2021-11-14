using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Models.AzureTranslator.Translate;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static PTI.Microservices.Library.Services.AzureTranslatorService;

namespace FairPlayTube.Services
{
    public class TranslationService
    {
        private readonly AzureTranslatorService AzureTranslatorService;
        private ILogger<TranslationService> Logger { get;}

        public TranslationService(AzureTranslatorService azureTranslatorService,
            ILogger<TranslationService> logger)
        {
            this.AzureTranslatorService = azureTranslatorService;
            this.Logger = logger;
        }
        public async Task<TranslateResponseLanguageInforomation[]> TranslateAsync(TranslateRequestTextItem[] model,
            string sourceLanguage, string destLanguage,
            CancellationToken cancellationToken)
        {
            List<TranslateResponseLanguageInforomation> result = new();
            foreach(var singleItem in model)
            {
                try
                {
                    var response = await this.AzureTranslatorService
                        .TranslateSimpleTextAsync(singleItem.Text, sourceLanguage, destLanguage, 
                        cancellationToken);
                    result.AddRange(response);
                }
                catch (Exception ex)
                {
                    this.Logger?.LogError(exception: ex, message: ex.Message);
                }
            }
            return result.ToArray();
        }
    }
}
