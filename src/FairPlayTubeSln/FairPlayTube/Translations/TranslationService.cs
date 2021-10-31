using FairPlayTube.Common.Localization;
using FairPlayTube.DataAccess.Data;
using FairPlayTube.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FairPlayTube.Translations
{
    /// <summary>
    /// 
    /// </summary>
    public class TranslationService : BackgroundService
    {
        private IServiceScopeFactory ServiceScopeFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceScopeFactory"></param>
        public TranslationService(IServiceScopeFactory serviceScopeFactory)
        {
            this.ServiceScopeFactory = serviceScopeFactory;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            using (var scope = this.ServiceScopeFactory.CreateScope())
            {
                FairplaytubeDatabaseContext fairplaytubeDatabaseContext =
                    scope.ServiceProvider.GetRequiredService<FairplaytubeDatabaseContext>();
                var clientAppAssembly = typeof(Client.Program).Assembly;
                var clientAppTypes = clientAppAssembly.GetTypes();

                var componentsAssembly = typeof(Components._Imports).Assembly;
                var componentsTypes = componentsAssembly.GetTypes();

                List<Type> typesToCheck = new List<Type>();
                typesToCheck.AddRange(clientAppTypes);
                typesToCheck.AddRange(componentsTypes);
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
                            ResourceKeyAttribute keyAttribute = resourceKeyAttributes.Single() as ResourceKeyAttribute;
                            var defaultValue = keyAttribute.DefaultValue;
                            string key = singleField.GetRawConstantValue()?.ToString();
                            var entity =
                                await fairplaytubeDatabaseContext.Resource
                                .SingleOrDefaultAsync(p => p.CultureId == 1 && 
                                p.Key == key &&
                                p.Type == p.Type, stoppingToken);
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
            }
        }
    }
}