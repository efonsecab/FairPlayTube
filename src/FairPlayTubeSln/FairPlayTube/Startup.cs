using FairPlayTube.DataAccess.Data;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAdB2C"));

            services.AddControllersWithViews()
                .AddMicrosoftIdentityUI();

            services.AddAuthorization(options =>
            {
                // By default, all incoming requests will be authorized according to the default policy
                options.FallbackPolicy = options.DefaultPolicy;
            });

            services.AddServerSideBlazor()
                .AddMicrosoftIdentityConsentHandler();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
                            this.CreateDbContext(context.RequestServices);
                            await fairplaytubeDatabaseContext.ErrorLog.AddAsync(new FairPlayTube.DataAccess.Models.ErrorLog()
                            {
                                FullException = error.ToString(),
                                StackTrace = error.StackTrace,
                                Message = error.Message
                            });
                            await fairplaytubeDatabaseContext.SaveChangesAsync();
                        }
                        catch (Exception)
                        {

                        }
                    }
                });
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private FairplaytubeDatabaseContext CreateDbContext(IServiceProvider requestServices)
        {
            throw new NotImplementedException();
        }
    }
}
