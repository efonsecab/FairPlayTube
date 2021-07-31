using PTI.Microservices.Library.AzureTextAnalytics.Models.DetectLanguage;
using PTI.Microservices.Library.Models.AzureTextAnalyticsService.GetKeyPhrases;
using PTI.Microservices.Library.Models.AzureTextAnalyticsService.GetSentiment;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class TextAnalysisServices
    {
        private AzureTextAnalyticsService AzureTextAnalyticsService { get; }
        public TextAnalysisServices(AzureTextAnalyticsService azureTextAnalyticsService)
        {
            this.AzureTextAnalyticsService = azureTextAnalyticsService;
        }

        public async Task<string> DetectLanguageAsync(string text, CancellationToken cancellationToken)
        {
            DetectLanguageRequest request = new DetectLanguageRequest()
            {
                documents = new DetectLanguageRequestDocument[]
                {
                    new DetectLanguageRequestDocument()
                    {
                        id=Guid.NewGuid().ToString(),
                        text=text
                    }
                }
            };
            var response = await this.AzureTextAnalyticsService.DetectLanguageAsync(request, cancellationToken);
            if (response.errors?.Length > 0)
            {
                string allErrorsText = String.Join("*", response.errors.Select(p => p.error.message));
                throw new Exception($"Unable to detect language. Details: {allErrorsText}");
            }
            return response.documents.First().detectedLanguage.iso6391Name;
        }

        public async Task<string> GetSentimentAsync(string text, string textLanguage, CancellationToken cancellationToken)
        {
            Guid id = Guid.NewGuid();
            GetSentimentRequest request = new GetSentimentRequest()
            {
                documents = new GetSentimentRequestDocument[]
                {
                    new GetSentimentRequestDocument()
                    {
                        id=id.ToString(),
                        language=textLanguage,
                        text=text
                    }
                }
            };
            var response = await this.AzureTextAnalyticsService.GetSentimentAsync(request, cancellationToken);
            if (response.documents?.Length > 0)
            {
                return response.documents.First().sentiment;
            }
            else
            throw new Exception($"Unable to retrieve sentiment for text: '{textLanguage}' on language: {textLanguage}");
        }

        public async Task<List<string>> GetKeyPhrasesAsync(string text, string textLanguage, CancellationToken cancellationToken)
        {
            List<string> keyPhrases = new List<string>();
            Guid id = Guid.NewGuid();
            var request =
            new GetKeyPhrasesRequest()
            {
                documents = new GetKeyPhrasesRequestDocument[]
                {
                    new GetKeyPhrasesRequestDocument()
                    {
                        id=id.ToString(),
                        language=textLanguage,
                        text=text
                    }
                }
            };
            var response = await this.AzureTextAnalyticsService.GetKeyPhrasesAsync(request, cancellationToken);
            foreach (var singleDocument in response.documents)
            {
                keyPhrases.AddRange(singleDocument.keyPhrases);
            }
            return keyPhrases;
        }
    }
}
