using System.Net.Http;

namespace FairPlayTube.ClientServices
{
    public class HttpClientService
    {
        private const string AssemblyName = "FairPlayTube";
        private IHttpClientFactory HttpClientFactory { get; }
        public HttpClientService(IHttpClientFactory httpClientFactory)
        {
            this.HttpClientFactory = httpClientFactory;
        }

        public HttpClient CreateAnonymousClient()
        {
            return this.HttpClientFactory.CreateClient($"{AssemblyName}.ServerAPI.Anonymous");
        }

        public HttpClient CreateAuthorizedClient()
        {
            return this.HttpClientFactory.CreateClient($"{AssemblyName}.ServerAPI");
        }
    }
}
