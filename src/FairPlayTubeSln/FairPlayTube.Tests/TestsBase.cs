using AutoMapper;
using FairPlayTube.Common.Configuration;
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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Tests
{
    public abstract class TestsBase
    {
        TestServer Server { get; }
        protected IMapper Mapper { get; }
        internal static IHttpContextAccessor HttpContextAccessor { get; set; }
        internal static string TestImageFilePath { get; set; }
        internal static DataStorageConfiguration DataStorageConfiguration { get; set; }
        internal static TestAzureAdB2CAuthConfiguration TestAzureAdB2CAuthConfiguration { get; set; }
        internal static AzureBlobStorageConfiguration AzureBlobStorageConfiguration { get; set; }
        private static IConfiguration Configuration { get; set; }
        public HttpClient UserRoleAuthorizedHttpClient { get; private set; }
        public HttpClient AdminRoleAuthorizedHttpClient { get; private set; }


        public TestsBase()
        {
            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder.AddJsonFile("appsettings.json")
                .AddUserSecrets("b59b6219-c1e3-49f7-bf68-a0b2c3ea122b");
            IConfiguration configuration = configurationBuilder.Build();
            Configuration = configuration;
            Server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration((hostingContext, configurationBuilder) =>
                {
                    IConfigurationRoot configurationRoot = configurationBuilder.Build();

                    var defaultConnectionString = configurationRoot.GetConnectionString("Default");
                    DbContextOptionsBuilder<FairplaytubeDatabaseContext> dbContextOptionsBuilder = new();

                    using FairplaytubeDatabaseContext FairplaytubeDatabaseContext =
                    new(dbContextOptionsBuilder.UseSqlServer(defaultConnectionString,
                    sqlServerOptionsAction: (serverOptions) => serverOptions.EnableRetryOnFailure(3)).Options);

                })
                .UseConfiguration(configuration)
                .UseStartup<Startup>());
            this.Mapper = Server.Services.GetRequiredService<IMapper>();
            HttpContextAccessor = Server.Services.GetRequiredService<IHttpContextAccessor>();
            string appDirectory = AppContext.BaseDirectory;
            string testImageFilePath = Path.Combine(appDirectory, @"ResourceFiles\Images\thumbnail_IMG_2501.jpg");
            TestImageFilePath = testImageFilePath;
            DataStorageConfiguration = Server.Services.GetRequiredService<DataStorageConfiguration>();
            AzureBlobStorageConfiguration = Server.Services.GetRequiredService<AzureBlobStorageConfiguration>();
            TestAzureAdB2CAuthConfiguration = Configuration.GetSection("TestAzureAdB2CAuthConfiguration").Get<TestAzureAdB2CAuthConfiguration>();
        }

        public static FairplaytubeDatabaseContext CreateDbContext()
        {
            var connectionString = Configuration.GetConnectionString("Default");
            DbContextOptionsBuilder<FairplaytubeDatabaseContext> dbContextOptionsBuilder =
            new();
            FairplaytubeDatabaseContext blazorRestaurantDbContext =
            new(dbContextOptionsBuilder.UseSqlServer(connectionString).Options,
            new CustomProviders.CurrentUserProvider(HttpContextAccessor));
            return blazorRestaurantDbContext;
        }

        public enum Role
        {
            Admin,
            User
        }
        protected async Task<HttpClient> CreateAuthorizedClientAsync(Role role)
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
                role == Role.User ? TestAzureAdB2CAuthConfiguration.UserRoleUsername : TestAzureAdB2CAuthConfiguration.AdminRoleUsername));
            formData.Add(new KeyValuePair<string?, string?>("password",
                role == Role.User ? TestAzureAdB2CAuthConfiguration.UserRolePassword : TestAzureAdB2CAuthConfiguration.AdminRolePassword));
            formData.Add(new KeyValuePair<string?, string?>("grant_type", "password"));
            string applicationId = TestAzureAdB2CAuthConfiguration.ApplicationId;
            formData.Add(new KeyValuePair<string?, string?>("scope", $"openid {applicationId} offline_access"));
            formData.Add(new KeyValuePair<string?, string?>("client_id", applicationId));
            formData.Add(new KeyValuePair<string?, string?>("response_type", "token id_token"));
            System.Net.Http.FormUrlEncodedContent form = new(formData);
            var response = await httpClient.PostAsync(TestAzureAdB2CAuthConfiguration.TokenUrl, form);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                var client = this.Server.CreateClient();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Access_token);
                switch (role)
                {
                    case Role.Admin:
                        this.AdminRoleAuthorizedHttpClient = client;
                        break;
                    case Role.User:
                        this.UserRoleAuthorizedHttpClient = client;
                        break;
                }
                return client;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
        }

    }

    public class AuthResponse
    {
        public string Access_token { get; set; }
        public string Token_type { get; set; }
        public string Expires_in { get; set; }
        public string Refresh_token { get; set; }
        public string Id_token { get; set; }
    }

    public class TestAzureAdB2CAuthConfiguration
    {
        public string TokenUrl { get; set; }
        public string UserRoleUsername { get; set; }
        public string UserRolePassword { get; set; }
        public string AdminRoleUsername { get; set; }
        public string AdminRolePassword { get; set; }
        public string ApplicationId { get; set; }
        public string AzureAdUserObjectId { get; set; }
    }
}
