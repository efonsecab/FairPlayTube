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
        private readonly IServiceScopeFactory ServiceScopeFactory;

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
            using var scope = this.ServiceScopeFactory.CreateScope();
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

            var servicesAssembly = typeof(FairPlayTube.Services.TranslationService).Assembly;
            var servicesTypes = servicesAssembly.GetTypes();

            var commonAssembly = typeof(FairPlayTube.Common.Global.Constants).Assembly;
            var commonTypes = commonAssembly.GetTypes();

            List<Type> typesToCheck = new();
            typesToCheck.AddRange(clientAppTypes);
            typesToCheck.AddRange(componentsTypes);
            typesToCheck.AddRange(modelsTypes);
            typesToCheck.AddRange(servicesTypes);
            typesToCheck.AddRange(commonTypes);

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
                    if (resourceKeyAttributes != null && resourceKeyAttributes.Any())
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

            var additionalSupportedCultures = await fairplaytubeDatabaseContext.Culture
                .Where(p => p.Name != "en-US").ToListAsync(cancellationToken: stoppingToken);
            foreach (var singleAdditionalCulture in additionalSupportedCultures)
            {
                var cultureTranslations = await
                    translationService.TranslateAsync(translateRequestItems,
                    "en",
                    singleAdditionalCulture.Name, stoppingToken);
                var cultureEntity = await fairplaytubeDatabaseContext
                    .Culture.SingleAsync(p => p.Name == singleAdditionalCulture.Name, cancellationToken: stoppingToken);
                for (int iPos = 0; iPos < cultureTranslations.Length; iPos++)
                {
                    var singleEnglishUSKey = allEnglishUSKeys[iPos];
                    var translatedValue = cultureTranslations[iPos].translations.First().text;
                    Resource resourceEntity = await fairplaytubeDatabaseContext.Resource
                        .SingleOrDefaultAsync(p => p.Key == singleEnglishUSKey.Key &&
                        p.Type == singleEnglishUSKey.Type && 
                        p.CultureId == cultureEntity.CultureId, cancellationToken: stoppingToken);

                    if (resourceEntity is null)
                    {
                        resourceEntity = new()
                        {
                            Key = singleEnglishUSKey.Key,
                            Type = singleEnglishUSKey.Type,
                            Value = translatedValue,
                            CultureId = cultureEntity.CultureId
                        };
                        await fairplaytubeDatabaseContext.Resource.AddAsync(resourceEntity, stoppingToken);
                    }
                }
                if (fairplaytubeDatabaseContext.ChangeTracker.HasChanges())
                    await fairplaytubeDatabaseContext.SaveChangesAsync(stoppingToken);
            }
        }
    }
}