using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Services
{
    public class ContentModerationService
    {
        private AzureContentModeratorService AzureContentModeratorService { get; }

        public ContentModerationService(AzureContentModeratorService azureContentModeratorService)
        {
            this.AzureContentModeratorService = azureContentModeratorService;
        }
        public async Task CheckMessageModeration(string messageText)
        {
            var result = await this.AzureContentModeratorService.AnalyzeTextAsync(messageText,
                AzureContentModeratorService.TextType.PlainText);
            var isRestricted = result.IsOffensive || result.IsSexuallyExplicit ||
                result.IsSexuallySuggestive || result.HasProfanityTerms;
            if (isRestricted)
                throw new Exception("Your message cannot be sent. " +
                    "Please remove any offensive, explicity or suggestive text, and any profanity terms");

        }
    }
}
