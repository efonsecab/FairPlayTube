using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.CustomExceptions;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.CustomLocalization.EF;
using FairPlayTube.CustomProviders;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.GatedFeatures.FeatureFilters;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Notifications.Hubs;
using FairPlayTube.Services;
using FairPlayTube.Services.BackgroundServices;
using FairPlayTube.Services.Configuration;
using FairPlayTube.SharedConfiguration;
using FairPlayTube.Swagger.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Interceptors;
using PTI.Microservices.Library.IpData.Configuration;
using PTI.Microservices.Library.IpData.Services;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube
{
    /// <summary>
    /// Used to configure system's startup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initialized <see cref="Startup"/>
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Represents the system's initial/startup configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// <summary>
        /// Configures the System Services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IStringLocalizerFactory, EFStringLocalizerFactory>();
            services.AddSingleton<IStringLocalizer, EFStringLocalizer>();
            services.AddLocalization();
            bool enablePTILibrariesLogging = Convert.ToBoolean(Configuration["EnablePTILibrariesLogging"]);
            GlobalPackageConfiguration.EnableHttpRequestInformationLog = enablePTILibrariesLogging;
            GlobalPackageConfiguration.RapidApiKey = Configuration.GetValue<string>("RapidApiKey");
            services.AddSignalR();
            services.AddHealthChecks().AddDbContextCheck<FairplaytubeDatabaseContext>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddTransient<ICurrentUserProvider, CurrentUserProvider>();
            services.AddTransient(serviceProvider =>
            {
                var fairplaytubeDatabaseContext = this.CreateFairPlayTubeDbContext(services);
                return fairplaytubeDatabaseContext;
            });


            services.AddTransient<CustomHttpClientHandler>();
            services.AddTransient<CustomHttpClient>(sp =>
            {
                var handler = sp.GetRequiredService<CustomHttpClientHandler>();
                return new CustomHttpClient(handler) { Timeout = TimeSpan.FromMinutes(30) };
            });
            ConfigureAzureTextAnalytics(services);
            ConfigureAzureContentModerator(services);
            ConfigureAzureVideoIndexer(services);
            ConfigureAzureBlobStorage(services);
            ConfigureDataStorage(services);
            ConfigurePayPal(services);
            ConfigureIpDataService(services);
            ConfigureIpStackService(services);
            ConfigureYouTube(services);
            ConfigureAzureTranslator(services);

            var smtpConfiguration = Configuration.GetSection(nameof(SmtpConfiguration)).Get<SmtpConfiguration>();
            services.AddSingleton(smtpConfiguration);
            AddPlatformServices(services);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAdB2C"));

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "Role";
                options.SaveToken = true;
                options.Events.OnMessageReceived = (context) =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken) &&
                        (path.StartsWithSegments(Common.Global.Constants.Hubs.NotificationHub)))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                };
                options.Events.OnTokenValidated = async (context) =>
                {
                    FairplaytubeDatabaseContext fairplaytubeDatabaseContext = CreateFairPlayTubeDbContext(services);
                    ClaimsIdentity claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                    var userObjectIdClaim = claimsIdentity.Claims.Single(p => p.Type == Common.Global.Constants.Claims.ObjectIdentifier);
                    var user = await fairplaytubeDatabaseContext.ApplicationUser
                    .Include(p => p.ApplicationUserRole)
                    .ThenInclude(p => p.ApplicationRole)
                    .Where(p => p.AzureAdB2cobjectId.ToString() == userObjectIdClaim.Value)
                    .SingleOrDefaultAsync();
                    var fullName = claimsIdentity.FindFirst(Common.Global.Constants.Claims.Name).Value;
                    var emailAddress = claimsIdentity.FindFirst(Common.Global.Constants.Claims.Emails).Value;
                    if (user != null && user.ApplicationUserRole != null)
                    {
                        foreach (var singleRole in user.ApplicationUserRole)
                        {
                            claimsIdentity.AddClaim(new Claim("Role", singleRole.ApplicationRole.Name));
                        }
                        user.FullName = fullName;
                        user.EmailAddress = emailAddress;
                        user.LastLogIn = DateTimeOffset.UtcNow;
                        await fairplaytubeDatabaseContext.SaveChangesAsync();
                    }
                    else
                    {
                        if (user == null)
                        {
                            var userRole = await fairplaytubeDatabaseContext.ApplicationRole.FirstAsync(p => p.Name == Common.Global.Constants.Roles.User);
                            user = new ApplicationUser()
                            {
                                LastLogIn = DateTimeOffset.UtcNow,
                                FullName = fullName,
                                EmailAddress = emailAddress,
                                AzureAdB2cobjectId = Guid.Parse(userObjectIdClaim.Value)
                            };
                            await fairplaytubeDatabaseContext.ApplicationUser.AddAsync(user);
                            await fairplaytubeDatabaseContext.SaveChangesAsync();
                            await fairplaytubeDatabaseContext.ApplicationUserRole.AddAsync(new ApplicationUserRole()
                            {
                                ApplicationUserId = user.ApplicationUserId,
                                ApplicationRoleId = userRole.ApplicationRoleId
                            });
                            await fairplaytubeDatabaseContext.SaveChangesAsync();
                            foreach (var singleRole in user.ApplicationUserRole)
                            {
                                claimsIdentity.AddClaim(new Claim("Role", singleRole.ApplicationRole.Name));
                            }
                            var adminUsers = await fairplaytubeDatabaseContext.ApplicationUser
                            .Include(p => p.ApplicationUserRole).ThenInclude(p => p.ApplicationRole)
                            .Where(p => p.ApplicationUserRole.Any(aur => aur.ApplicationRole.Name ==
                              Common.Global.Constants.Roles.Admin))
                            .ToListAsync();
                            var emailService = context.HttpContext.RequestServices
                            .GetRequiredService<EmailService>();
                            foreach (var singleAdminUser in adminUsers)
                            {
                                await emailService.SendEmailAsync(toEmailAddress:
                                    singleAdminUser.EmailAddress, subject: "FairPlayTube - New User",
                                    body: 
                                    $"<p>A new user has been created at: {context.Request.Host}</p>" +
                                    $"<p>Name: {user.FullName}</p>" +
                                    $"<p>Email: {user.EmailAddress}</p>",
                                    isBodyHtml: true, cancellationToken: CancellationToken.None);
                            }
                        }
                    }
                };
            });

            services.AddControllersWithViews();

            services.AddAutoMapper(configAction =>
            {
                configAction.AddMaps(new[] { typeof(Startup).Assembly });
            });

            services.AddRazorPages();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });
            services.AddHostedService<Translations.BackgroundTranslationService>();
            services.AddHostedService<VideoIndexStatusService>();
            bool enableSwagger = Convert.ToBoolean(Configuration["EnableSwaggerUI"]);
            if (enableSwagger)
            {
                var azureAdB2CInstance = Configuration["AzureAdB2C:Instance"];
                var azureAdB2CDomain = Configuration["AzureAdB2C:Domain"];
                var azureAdB2CClientAppClientId = Configuration["AzureAdB2C:ClientAppClientId"];
                var azureAdB2ClientAppDefaultScope = Configuration["AzureAdB2C:ClientAppDefaultScope"];
                services.AddSwaggerGen(c =>
                {
                    var basePath = AppContext.BaseDirectory;
                    var mainAppXmlFilename = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                    var modelsFileName = typeof(FairPlayTube.Models.Video.VideoInfoModel).Assembly.GetName().Name + ".xml";
                    var mainAppXmlPath = Path.Combine(basePath, mainAppXmlFilename);
                    var modelsXmlPath = Path.Combine(basePath, modelsFileName);
                    c.IncludeXmlComments(mainAppXmlPath);
                    c.IncludeXmlComments(modelsXmlPath);
                    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "FairPlayTube API" });
                    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Implicit = new OpenApiOAuthFlow()
                            {
                                AuthorizationUrl = new Uri($"{azureAdB2CInstance}/{azureAdB2CDomain}/oauth2/v2.0/authorize"),
                                TokenUrl = new Uri($"{azureAdB2CInstance}/{azureAdB2CDomain}/oauth2/v2.0/token"),
                                Scopes = new Dictionary<string, string>
                               {
                               {azureAdB2ClientAppDefaultScope, "Access APIs" }
                               }
                            },
                        }
                    });
                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                });
            }
            services.AddFeatureManagement()
                .AddFeatureFilter<PaidFeatureFilter>()
                .UseDisabledFeaturesHandler((features, context) =>
                {
                    string joinedFeaturesNames = String.Join(",", features);
                    context.Result = new ObjectResult($"Missing features: {joinedFeaturesNames}")
                    {
                        StatusCode = (int)System.Net.HttpStatusCode.Forbidden
                    };
                });
        }

        private void ConfigureAzureTranslator(IServiceCollection services)
        {
            AzureTranslatorConfiguration azureTranslatorConfiguration =
                Configuration.GetSection(nameof(AzureTranslatorConfiguration))
                .Get<AzureTranslatorConfiguration>();
            services.AddSingleton(azureTranslatorConfiguration);
            services.AddTransient<AzureTranslatorService>();
        }

        private void ConfigureYouTube(IServiceCollection services)
        {
            YoutubeConfiguration youtubeConfiguration = Configuration.GetSection(nameof(YoutubeConfiguration))
                            .Get<YoutubeConfiguration>();
            services.AddSingleton(youtubeConfiguration);
            services.AddTransient<YoutubeService>();
        }

        private void ConfigureAzureTextAnalytics(IServiceCollection services)
        {
            AzureTextAnalyticsConfiguration azureTextAnalyticsConfiguration =
                            Configuration.GetSection(nameof(AzureTextAnalyticsConfiguration))
                            .Get<AzureTextAnalyticsConfiguration>();
            services.AddSingleton(azureTextAnalyticsConfiguration);
            services.AddTransient<AzureTextAnalyticsService>();
        }

        private static void AddPlatformServices(IServiceCollection services)
        {
            services.AddTransient<EmailService>();
            services.AddTransient<VideoService>();
            services.AddTransient<PaymentService>();
            services.AddTransient<VisitorTrackingService>();
            services.AddTransient<MessageService>();
            services.AddTransient<ContentModerationService>();
            services.AddTransient<TextAnalysisService>();
            services.AddTransient<UserService>();
            services.AddTransient<VideoPlaylistService>();
            services.AddTransient<SearchService>();
            services.AddTransient<VideoCommentService>();
            services.AddTransient<VideoJobService>();
            services.AddTransient<UserYouTubeChannelService>();
            services.AddTransient<RssFeedService>();
            services.AddTransient<TranslationService>();
            services.AddTransient<PayoutService>();
            services.AddTransient<VideoJobApplicationService>();
            services.AddTransient<UserRequestService>();
            services.AddTransient<UserMessageService>();
        }

        private void ConfigureAzureContentModerator(IServiceCollection services)
        {
            AzureContentModeratorConfiguration azureContentModeratorConfiguration =
                            Configuration.GetSection(nameof(AzureContentModeratorConfiguration))
                            .Get<AzureContentModeratorConfiguration>();
            services.AddSingleton(azureContentModeratorConfiguration);
            services.AddTransient<AzureContentModeratorService>();
        }

        private void ConfigureIpDataService(IServiceCollection services)
        {
            IpDataConfiguration ipDataConfiguration =
                            Configuration.GetSection(nameof(IpDataConfiguration))
                            .Get<IpDataConfiguration>();
            services.AddSingleton(ipDataConfiguration);
            services.AddTransient<IpDataService>();
        }

        private void ConfigureIpStackService(IServiceCollection services)
        {
            IpStackConfiguration ipStackConfiguration =
                            Configuration.GetSection(nameof(IpStackConfiguration))
                            .Get<IpStackConfiguration>();
            services.AddSingleton(ipStackConfiguration);
            services.AddTransient<IpStackService>();
        }

        private void ConfigurePayPal(IServiceCollection services)
        {
            PaypalConfiguration paypalConfiguration = Configuration.GetSection(nameof(PaypalConfiguration))
                            .Get<PaypalConfiguration>();
            services.AddSingleton(paypalConfiguration);
            services.AddTransient<PaypalService>();
        }

        private void ConfigureDataStorage(IServiceCollection services)
        {
            DataStorageConfiguration dataStorageConfiguration =
                            Configuration.GetSection("DataStorageConfiguration").Get<DataStorageConfiguration>();
            services.AddSingleton(dataStorageConfiguration);
        }

        private void ConfigureAzureVideoIndexer(IServiceCollection services)
        {
            AzureVideoIndexerConfiguration azureVideoIndexerConfiguration =
                Configuration.GetSection($"AzureConfiguration:{nameof(AzureVideoIndexerConfiguration)}")
                .Get<AzureVideoIndexerConfiguration>();
            services.AddSingleton(azureVideoIndexerConfiguration);
            services.AddTransient<AzureVideoIndexerService>();
        }

        private void ConfigureAzureBlobStorage(IServiceCollection services)
        {
            AzureBlobStorageConfiguration azureBlobStorageConfiguration =
                Configuration.GetSection($"AzureConfiguration:{nameof(AzureBlobStorageConfiguration)}")
                .Get<AzureBlobStorageConfiguration>();
            services.AddSingleton(azureBlobStorageConfiguration);
            services.AddTransient<AzureBlobStorageService>(sp =>
            {
                CustomHttpClient customHttpClient = sp.GetRequiredService<CustomHttpClient>();
                customHttpClient.Timeout = TimeSpan.FromMinutes(60);
                return new AzureBlobStorageService(logger: sp.GetRequiredService<ILogger<AzureBlobStorageService>>(),
                    azureBlobStorageConfiguration: azureBlobStorageConfiguration,
                    customHttpClient: customHttpClient);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configure the Application Behavior and pipleline execution
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="sp"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IServiceProvider sp)
        {
            var dbContext = sp.GetRequiredService<FairplaytubeDatabaseContext>();

            var supportedCultures = dbContext.Culture.Select(p => new CultureInfo(p.Name)).ToList();
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(supportedCultures.First().Name),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(options);
            app.UseResponseCompression();
            bool useHttpsRedirection = Convert.ToBoolean(Configuration["UseHttpsRedirection"]);
            bool enableSwagger = Convert.ToBoolean(Configuration["EnableSwaggerUI"]);
            if (enableSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FairPlayTube API");
                    c.OAuthClientId(Configuration["AzureAdB2C:ClientAppClientId"]);
                    c.OAuthAdditionalQueryStringParams(new Dictionary<string, string>()
                    {
                    {"p", Configuration["AzureAdB2C:SignUpSignInPolicyId"] }
                    });
                });
            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                if (useHttpsRedirection)
                    app.UseHsts();
            }


            app.UseExceptionHandler(cfg =>
            {
                cfg.Run(async context =>
                {
                    var exceptionHandlerPathFeature =
                    context.Features.Get<IExceptionHandlerPathFeature>();
                    var error = exceptionHandlerPathFeature.Error;
                    long? errorId = default;
                    if (error != null)
                    {
                        try
                        {
                            FairplaytubeDatabaseContext fairplaytubeDatabaseContext =
                            this.CreateFairPlayTubeDbContext(context.RequestServices);
                            ErrorLog errorLog = new()
                            {
                                FullException = error.ToString(),
                                StackTrace = error.StackTrace,
                                Message = error.Message
                            };
                            await fairplaytubeDatabaseContext.ErrorLog.AddAsync(errorLog);
                            await fairplaytubeDatabaseContext.SaveChangesAsync();
                            errorId = errorLog.ErrorLogId;
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        ProblemHttpResponse problemHttpResponse = new();
                        if (error is CustomValidationException)
                        {
                            problemHttpResponse.Detail = error.Message;
                        }
                        else
                        {
                            string userVisibleError = "An error ocurred.";
                            if (errorId.HasValue)
                            {
                                userVisibleError += $" Error code: {errorId}";
                            }
                            problemHttpResponse.Detail = userVisibleError;
                        }
                        problemHttpResponse.Status = (int)System.Net.HttpStatusCode.BadRequest;
                        await context.Response.WriteAsJsonAsync<ProblemHttpResponse>(problemHttpResponse);
                    }
                });
            });
            //For MAUI in .NET 6 preview 4 using HTTPs is not working
            if (useHttpsRedirection)
                app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/Sitemap.xml"))
                {
                    var syncIoFeature = context.Features.Get<IHttpBodyControlFeature>();
                    if (syncIoFeature != null)
                    {
                        syncIoFeature.AllowSynchronousIO = true;
                    }
                }

                await next();
            });
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/api/health");
                endpoints.MapHub<NotificationHub>(Common.Global.Constants.Hubs.NotificationHub);
                if (env.IsProduction())
                    endpoints.MapFallbackToFile("index.html");
                else
                    endpoints.MapFallbackToFile("index.Development.html");
            });
        }

        private FairplaytubeDatabaseContext CreateFairPlayTubeDbContext(IServiceCollection services)
        {
            var sp = services.BuildServiceProvider();
            var currentUserProvider = sp.GetService<ICurrentUserProvider>();
            return ConfigureFairPlayTubeDataContext(currentUserProvider);
        }

        private FairplaytubeDatabaseContext CreateFairPlayTubeDbContext(IServiceProvider serviceProvider)
        {
            var currentUserProvider = serviceProvider.GetService<ICurrentUserProvider>();
            return ConfigureFairPlayTubeDataContext(currentUserProvider);
        }

        private FairplaytubeDatabaseContext ConfigureFairPlayTubeDataContext(ICurrentUserProvider currentUserProvider)
        {
            DbContextOptionsBuilder<FairplaytubeDatabaseContext> dbContextOptionsBuilder =
                new();
            bool useInMemoryDatabase = Convert.ToBoolean(Configuration["UseInMemoryDatabase"]);
            if (useInMemoryDatabase)
            {
                dbContextOptionsBuilder =
                    dbContextOptionsBuilder.UseInMemoryDatabase("FairPlayTubeInMemoryDb");
            }
            else
            {
                dbContextOptionsBuilder =
                    dbContextOptionsBuilder.UseSqlServer(Configuration.GetConnectionString("Default"),
            sqlServerOptionsAction: (serverOptions) => serverOptions
            .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null));
            }
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext =
            new(dbContextOptionsBuilder.Options, currentUserProvider);
            if (useInMemoryDatabase)
            {
                ConfigureInMemoryDatabase(fairplaytubeDatabaseContext);
            }
            return fairplaytubeDatabaseContext;
        }

        private static void ConfigureInMemoryDatabase(FairplaytubeDatabaseContext fairplaytubeDatabaseContext)
        {
            fairplaytubeDatabaseContext.Database.EnsureCreated();
            SeedDefaultCultures(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                cultureName: "en-US", cultureId: 1);
            SeedDefaultRoles(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                roleId: 1, roleName: Common.Global.Constants.Roles.User);
            SeedDefaultRoles(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                roleId: 2, roleName: Common.Global.Constants.Roles.Admin);
            fairplaytubeDatabaseContext.SaveChanges();
            SeedDefaultVideoIndexStatuses(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                videoIndexStatus: Common.Global.Enums.VideoIndexStatus.Pending);
            SeedDefaultVideoIndexStatuses(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                videoIndexStatus: Common.Global.Enums.VideoIndexStatus.Processing);
            SeedDefaultVideoIndexStatuses(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                videoIndexStatus: Common.Global.Enums.VideoIndexStatus.Processed);
            SeedDefaultVideoIndexStatuses(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                videoIndexStatus: Common.Global.Enums.VideoIndexStatus.Deleted);

            SeedDefaultVideoVisibility(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                visibility: Common.Global.Enums.VideoVisibility.Public);
            SeedDefaultVideoVisibility(fairplaytubeDatabaseContext: fairplaytubeDatabaseContext,
                visibility: Common.Global.Enums.VideoVisibility.Private);
        }

        private static void SeedDefaultCultures(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            string cultureName, int cultureId)
        {
            var cultureEntity = fairplaytubeDatabaseContext.Culture
                           .SingleOrDefault(p => p.Name == cultureName);
            if (cultureEntity == null)
            {
                cultureEntity = new Culture()
                {
                    CultureId = cultureId,
                    Name = cultureName
                };
                fairplaytubeDatabaseContext.Culture.Add(cultureEntity);
                fairplaytubeDatabaseContext.SaveChanges();
            }
        }

        private static void SeedDefaultVideoVisibility(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            Common.Global.Enums.VideoVisibility visibility)
        {
            var visibilityEntity = fairplaytubeDatabaseContext.VideoVisibility
                .SingleOrDefault(p => p.Name == visibility.ToString());
            if (visibilityEntity == null)
            {
                visibilityEntity = new VideoVisibility()
                {
                    VideoVisibilityId = (short)visibility,
                    Name = visibility.ToString()
                };
                fairplaytubeDatabaseContext.Add(visibilityEntity);
                fairplaytubeDatabaseContext.SaveChanges();
            }
        }

        private static void SeedDefaultVideoIndexStatuses(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            Common.Global.Enums.VideoIndexStatus videoIndexStatus)
        {

            var videoIndexStatusEntity = fairplaytubeDatabaseContext.VideoIndexStatus
                .SingleOrDefault(p => p.Name == videoIndexStatus.ToString());
            if (videoIndexStatusEntity == null)
            {
                videoIndexStatusEntity = new VideoIndexStatus()
                {
                    Name = videoIndexStatus.ToString(),
                    VideoIndexStatusId = (short)videoIndexStatus
                };
                fairplaytubeDatabaseContext.VideoIndexStatus.Add(videoIndexStatusEntity);
                fairplaytubeDatabaseContext.SaveChanges();
            }
        }

        private static void SeedDefaultRoles(FairplaytubeDatabaseContext fairplaytubeDatabaseContext,
            short roleId, string roleName)
        {
            var roleEntity = fairplaytubeDatabaseContext.ApplicationRole
                .SingleOrDefault(p => p.Name == roleName);
            if (roleEntity == null)
            {
                roleEntity = new ApplicationRole()
                {
                    ApplicationRoleId = roleId,
                    Name = roleName,
                    Description = roleName
                };
                fairplaytubeDatabaseContext.ApplicationRole.Add(roleEntity);
                fairplaytubeDatabaseContext.SaveChanges();
            }
        }
    }
}
