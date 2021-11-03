using FairPlayTube.Common.Localization;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using FairPlayTube.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PTI.Microservices.Library.Models.AzureTranslator.Translate;
using PTI.Microservices.Library.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static PTI.Microservices.Library.Services.AzureTranslatorService;

namespace FairPlayTube.Translations
{
    /// <summary>
    /// 
    /// </summary>
    public class BackgroundTranslationService : BackgroundService
    {
        private IServiceScopeFactory ServiceScopeFactory;

        private ILogger<BackgroundTranslationService> Logger { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        /// <param name="logger"></param>
        public BackgroundTranslationService(IServiceScopeFactory serviceScopeFactory,
            ILogger<BackgroundTranslationService> logger)
        {
            this.ServiceScopeFactory = serviceScopeFactory;
            this.Logger = logger;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            try
            {
                await Process(stoppingToken);
            }
            catch (Exception ex)
            {
                this.Logger?.LogError(exception: ex, message: ex.Message);
            }
        }

        private async Task Process(CancellationToken stoppingToken)
        {
            using (var scope = this.ServiceScopeFactory.CreateScope())
            {
                FairplaytubeDatabaseContext fairplaytubeDatabaseContext =
                    scope.ServiceProvider.GetRequiredService<FairplaytubeDatabaseContext>();
                var translationService =
                    scope.ServiceProvider.GetRequiredService<TranslationService>();
                var clientAppAssembly = typeof(Client.Program).Assembly;
                var clientAppTypes = clientAppAssembly.GetTypes();

                var componentsAssembly = typeof(Components._Imports).Assembly;
                var componentsTypes = componentsAssembly.GetTypes();

                var modelsAssembly = typeof(Models.Video.UploadVideoModel).Assembly;
                var modelsTypes = modelsAssembly.GetTypes();

                List<Type> typesToCheck = new List<Type>();
                typesToCheck.AddRange(clientAppTypes);
                typesToCheck.AddRange(componentsTypes);
                typesToCheck.AddRange(modelsTypes);

                foreach (var singleTypeToCheck in typesToCheck)
                {
                    string typeFullName = singleTypeToCheck.FullName;
                    var fields = singleTypeToCheck.GetFields(
                        BindingFlags.Public |
                        BindingFlags.Static |
                        BindingFlags.FlattenHierarchy
                        );
                    foreach (var singleField in fields)
                    {
                        var resourceKeyAttributes =
                            singleField.GetCustomAttributes<ResourceKeyAttribute>();
                        if (resourceKeyAttributes != null && resourceKeyAttributes.Count() > 0)
                        {
                            ResourceKeyAttribute keyAttribute = resourceKeyAttributes.Single();
                            var defaultValue = keyAttribute.DefaultValue;
                            string key = singleField.GetRawConstantValue().ToString();
                            var entity =
                                await fairplaytubeDatabaseContext.Resource
                                .SingleOrDefaultAsync(p => p.CultureId == 1 &&
                                p.Key == key &&
                                p.Type == typeFullName, stoppingToken);
                            if (entity is null)
                            {
                                entity = new Resource()
                                {
                                    CultureId = 1,
                                    Key = key,
                                    Type = typeFullName,
                                    Value = keyAttribute.DefaultValue
                                };
                                await fairplaytubeDatabaseContext.Resource.AddAsync(entity, stoppingToken);
                            }
                        }
                    }
                }
                if (fairplaytubeDatabaseContext.ChangeTracker.HasChanges())
                    await fairplaytubeDatabaseContext.SaveChangesAsync(stoppingToken);
                var allEnglishUSKeys =
                    await fairplaytubeDatabaseContext.Resource
                    .Include(p => p.Culture)
                    .Where(p => p.Culture.Name == "en-US")
                    .ToListAsync(stoppingToken);
                TranslateRequestTextItem[] translateRequestItems =
                    allEnglishUSKeys.Select(p => new TranslateRequestTextItem() 
                    {
                        Text = p.Value
                    }).ToArray();
                var spanishTranslations = await
                    translationService.TranslateAsync(translateRequestItems,
                    AzureTranslatorLanguage.English,
                    AzureTranslatorLanguage.Spanish, stoppingToken);

                for (int iPos = 0; iPos < spanishTranslations.Length; iPos++)
                {
                    var singleEnglishUSKey = allEnglishUSKeys[iPos];
                    var translatedValue = spanishTranslations[iPos].translations.First().text;

                    Resource resource = new Resource()
                    {
                        Key = singleEnglishUSKey.Key,
                        Type = singleEnglishUSKey.Type,
                        Value = translatedValue,
                        CultureId = 2
                    };
                    await fairplaytubeDatabaseContext.Resource.AddAsync(resource, stoppingToken);
                }
                if (fairplaytubeDatabaseContext.ChangeTracker.HasChanges())
                    await fairplaytubeDatabaseContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}