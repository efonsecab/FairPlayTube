using AutoMapper;
using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Global;
using FairPlayTube.DataAccess.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PTI.Microservices.Library.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Http;
using FairPlayTube.ClientServices;
using System.Threading;

namespace FairPlayTube.Tests
{
    public abstract class TestsBase
    {
        internal static TestServer? Server { get; set; }
        protected IMapper Mapper { get; }
        private static IHttpContextAccessor? HttpContextAccessor { get; set; }
        internal static string? TestVideoBloblUrl { get; set; }
        public TestsHttpClientFactory TestsHttpClientFactory { get; }
        internal static DataStorageConfiguration? DataStorageConfiguration { get; set; }
        internal static TestAzureAdB2CAuthConfiguration? TestAzureAdB2CAuthConfiguration { get; set; }
        internal static AzureVideoIndexerConfiguration? AzureVideoIndexerConfiguration { get; set; }
        internal static AzureBlobStorageConfiguration? AzureBlobStorageConfiguration { get; set; }
        protected static IConfiguration? Configuration { get; set; }
        private HttpClient? UserRoleAuthorizedHttpClient { get; set; }
        private HttpClient? AdminRoleAuthorizedHttpClient { get; set; }
        internal static string? UserBearerToken { get; set; }


        public TestsBase()
        {
            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder.AddJsonFile("appsettings.json")
                .AddUserSecrets("74135dc8-e371-4439-8744-4493c94df36e");
            var configRoot = configurationBuilder.Build();
            configurationBuilder.AddAzureAppConfiguration(options =>
            {
                var azureAppConfigConnectionString =
                    configRoot[Constants.ConfigurationKeysNames.AzureAppConfigConnectionString];
                options.Connect(azureAppConfigConnectionString);
            });
            IConfiguration configuration = configurationBuilder.Build();
            Server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    IConfigurationRoot configurationRoot = configurationBuilder.Build();

                    var defaultConnectionString = configurationRoot.GetConnectionString(
                        Common.Global.Constants.ConfigurationKeysNames.DefaultConnectionString);
                    DbContextOptionsBuilder<FairplaytubeDatabaseContext> dbContextOptionsBuilder = new();

                    using FairplaytubeDatabaseContext FairplaytubeDatabaseContext =
                    new(dbContextOptionsBuilder.UseSqlServer(defaultConnectionString,
                    sqlServerOptionsAction: (serverOptions) => serverOptions.EnableRetryOnFailure(3)).Options);

                })
                .UseConfiguration(configuration)
                .UseStartup<Startup>());
            Configuration = Server.Services.GetRequiredService<IConfiguration>();
            this.Mapper = Server.Services.GetRequiredService<IMapper>();
            HttpContextAccessor = Server.Services.GetRequiredService<IHttpContextAccessor>();
            DataStorageConfiguration = Server.Services.GetRequiredService<DataStorageConfiguration>();
            AzureBlobStorageConfiguration = Server.Services.GetRequiredService<AzureBlobStorageConfiguration>();
            TestAzureAdB2CAuthConfiguration = Configuration.GetSection("TestAzureAdB2CAuthConfiguration").Get<TestAzureAdB2CAuthConfiguration>();
            AzureVideoIndexerConfiguration = Server.Services.GetRequiredService<AzureVideoIndexerConfiguration>();
            TestVideoBloblUrl = configRoot.GetValue<string>("TestVideoBloblUrl");
            this.TestsHttpClientFactory = new TestsHttpClientFactory();
        }


        public static FairplaytubeDatabaseContext CreateDbContext()
        {
            var dbContext = Server!.Services.GetRequiredService<FairplaytubeDatabaseContext>();
            return dbContext;
        }

        public enum Role
        {
            Admin,
            User
        }

        protected HttpClient CreateAnonymousClient()
        {
            return Server!.CreateClient();
        }

        protected async Task<HttpClient> SignIn(Role role)
        {
            var authorizedHttpClient = await CreateAuthorizedClientAsync(role);
            var userRole = await authorizedHttpClient.GetStringAsync(Constants.ApiRoutes.UserController.GetMyRole);
            return authorizedHttpClient;
        }

        private async Task<HttpClient> CreateAuthorizedClientAsync(Role role)
        {

            switch (role)
            {
                case Role.Admin:
                    if (this.AdminRoleAuthorizedHttpClient != null)
                        return this.AdminRoleAuthorizedHttpClient;
                    break;
                case Role.User:
                    if (this.UserRoleAuthorizedHttpClient != null)
                        return this.UserRoleAuthorizedHttpClient;
                    break;
            }
            HttpClient httpClient = new();
            List<KeyValuePair<string?, string?>> formData = new();
            formData.Add(new KeyValuePair<string?, string?>("username",
                role == Role.User ? TestAzureAdB2CAuthConfiguration!.UserRoleUsername : TestAzureAdB2CAuthConfiguration!.AdminRoleUsername));
            formData.Add(new KeyValuePair<string?, string?>("password",
                role == Role.User ? TestAzureAdB2CAuthConfiguration.UserRolePassword : TestAzureAdB2CAuthConfiguration.AdminRolePassword));
            formData.Add(new KeyValuePair<string?, string?>("grant_type", "password"));
            string? applicationId = TestAzureAdB2CAuthConfiguration.ApplicationId;
            formData.Add(new KeyValuePair<string?, string?>("scope", $"openid {applicationId} offline_access"));
            formData.Add(new KeyValuePair<string?, string?>("client_id", applicationId));
            formData.Add(new KeyValuePair<string?, string?>("response_type", "token id_token"));
            System.Net.Http.FormUrlEncodedContent form = new(formData);
            var response = await httpClient.PostAsync(TestAzureAdB2CAuthConfiguration.TokenUrl, form);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                var client = Server!.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result!.Access_token);
                switch (role)
                {
                    case Role.Admin:
                        this.AdminRoleAuthorizedHttpClient = client;
                        break;
                    case Role.User:
                        this.UserRoleAuthorizedHttpClient = client;
                        break;
                }
                UserBearerToken = result!.Access_token;
                return client;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

        private HttpClientService CreateHttpClientService()
        {
            HttpClientService httpClientService = new HttpClientService(this.TestsHttpClientFactory);
            return httpClientService;
        }

        protected UserClientService CreateUserClientService()
        {
            UserClientService userClientService = new UserClientService(CreateHttpClientService());
            return userClientService;
        }

        protected VideoClientService CreateVideoClientService()
        {
            VideoClientService videoClientService = new VideoClientService(CreateHttpClientService());
            return videoClientService;
        }
    }

    public class TestsHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            string assemblyName = "FairPlayTube";
            var serverApiClientName = $"{assemblyName}.ServerAPI";
            var client = TestsBase.Server!.CreateClient();
            if (name == serverApiClientName)
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", TestsBase.UserBearerToken);
                return client;
            }
            else
                return client;
        }
    }

    //public class TestsMessageHandler : System.Net.Http.HttpClientHandler
    //{
    //    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        return base.Send(request, cancellationToken);
    //    }

    //    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        return await base.SendAsync(request, cancellationToken);
    //    }
    //}

    public class AuthResponse
    {
        public string? Access_token { get; set; }
        public string? Token_type { get; set; }
        public string? Expires_in { get; set; }
        public string? Refresh_token { get; set; }
        public string? Id_token { get; set; }
    }

    public class TestAzureAdB2CAuthConfiguration
    {
        public string? TokenUrl { get; set; }
        public string? UserRoleUsername { get; set; }
        public string? UserRolePassword { get; set; }
        public string? AdminRoleUsername { get; set; }
        public string? AdminRolePassword { get; set; }
        public string? ApplicationId { get; set; }
        public string? AzureAdUserObjectId { get; set; }
    }
}
