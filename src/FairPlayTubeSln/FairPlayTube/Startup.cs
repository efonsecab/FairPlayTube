using FairPlayTube.Common.Configuration;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.CustomProviders;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Models;
using FairPlayTube.Models.CustomHttpResponse;
using FairPlayTube.Services;
using FairPlayTube.Services.BackgroundServices;
using FairPlayTube.Swagger.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using PTI.Microservices.Library.Configuration;
using PTI.Microservices.Library.Interceptors;
using PTI.Microservices.Library.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace FairPlayTube
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            GlobalPackageConfiguration.EnableHttpRequestInformationLog = false;
            GlobalPackageConfiguration.RapidApiKey = Configuration.GetValue<string>("RapidApiKey");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
            services.AddScoped(serviceProvider =>
            {
                var fairplaytubeDatabaseContext = this.CreateFairPlayTubeDbContext(services);
                return fairplaytubeDatabaseContext;
            });


            services.AddScoped<CustomHttpClientHandler>();
            services.AddScoped<CustomHttpClient>();

            ConfigureAzureVideoIndexer(services);
            ConfigureAzureBlobStorage(services);

            DataStorageConfiguration dataStorageConfiguration =
                Configuration.GetSection("DataStorageConfiguration").Get<DataStorageConfiguration>();
            services.AddSingleton(dataStorageConfiguration);

            services.AddScoped<VideoService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAdB2C"));

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "Role";
                options.SaveToken = true;
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
                        claimsIdentity.AddClaim(new Claim("Role", user.ApplicationUserRole.ApplicationRole.Name));
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
                            claimsIdentity.AddClaim(new Claim("Role", user.ApplicationUserRole.ApplicationRole.Name));
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
                   c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "FairPlayTube API" });
                   c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                   {
                       Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
                       Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows()
                       {
                           Implicit = new Microsoft.OpenApi.Models.OpenApiOAuthFlow()
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

        }

        private void ConfigureAzureVideoIndexer(IServiceCollection services)
        {
            AzureVideoIndexerConfiguration azureVideoIndexerConfiguration =
                Configuration.GetSection($"AzureConfiguration:{nameof(AzureVideoIndexerConfiguration)}")
                .Get<AzureVideoIndexerConfiguration>();
            services.AddSingleton(azureVideoIndexerConfiguration);
            services.AddScoped<AzureVideoIndexerService>();
        }

        private void ConfigureAzureBlobStorage(IServiceCollection services)
        {
            AzureBlobStorageConfiguration azureBlobStorageConfiguration =
                Configuration.GetSection($"AzureConfiguration:{nameof(AzureBlobStorageConfiguration)}")
                .Get<AzureBlobStorageConfiguration>();
            services.AddSingleton(azureBlobStorageConfiguration);
            services.AddScoped<AzureBlobStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            bool useHttpsRedirection = Convert.ToBoolean(Configuration["UseHttpsRedirection"]);
            bool enableSwagger = Convert.ToBoolean(Configuration["EnableSwaggerUI"]);
            if (enableSwagger)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FairPlayTube API");
                    c.OAuthClientId(Configuration["AzureAdB2C:ClientAppClientId"]);
                    c.OAuthAdditionalQueryStringParams(new System.Collections.Generic.Dictionary<string, string>()
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
                    if (error != null)
                    {
                        try
                        {
                            FairplaytubeDatabaseContext fairplaytubeDatabaseContext =
                            this.CreateFairPlayTubeDbContext(context.RequestServices);
                            await fairplaytubeDatabaseContext.ErrorLog.AddAsync(new ErrorLog()
                            {
                                FullException = error.ToString(),
                                StackTrace = error.StackTrace,
                                Message = error.Message
                            });
                            await fairplaytubeDatabaseContext.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                        ProblemHttpResponse problemHttpResponse = new()
                        {
                            Detail = error.Message,
                        };
                        await context.Response.WriteAsJsonAsync<ProblemHttpResponse>(problemHttpResponse);
                    }
                });
            });
            //For MAUI in .NET 6 preview 4 using HTTPs is not working
            if (useHttpsRedirection)
                app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapBlazorHub();
                //endpoints.MapControllers();
                //endpoints.MapFallbackToPage("/_Host");
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
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
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext =
            new(dbContextOptionsBuilder.UseSqlServer(Configuration.GetConnectionString("Default"),
            sqlServerOptionsAction: (serverOptions) => serverOptions
            .EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)).Options,
            currentUserProvider);
            return fairplaytubeDatabaseContext;
        }
    }
}
