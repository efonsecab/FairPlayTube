using FairPlayTube.Controllers;
using FairPlayTube.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.SystemConfigurator.Pages
{
    public partial class Index
    {
        public string ErrorMessage { get; private set; }
        public List<Controller> Controllers = new List<Controller>();
        public string ConnectionString { get; set; }
        protected override void OnInitialized()
        {
            try
            {
                var assembly = typeof(VideoController).Assembly;
                var types = assembly.GetTypes().Where(p => p.Name.EndsWith("Controller"));
                foreach (var singleType in types)
                {
                    Controller controller = new Controller()
                    {
                        Name = singleType.Name
                    };
                    var endpoints = singleType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
                    foreach (var singleEndpoint in endpoints)
                    {
                        bool defaultValue = true;
                        var featureGateAttributes =
                        singleEndpoint.CustomAttributes.Where(p => p.AttributeType == typeof(FeatureGateAttribute));
                        foreach (var singleFeatureGateAttribute in featureGateAttributes)
                        {
                            defaultValue = false;
                        }
                        controller.Endpoints.Add(new Endpoint()
                        {
                            Name = singleEndpoint.Name,
                            DefaultValue = defaultValue
                        });
                    }
                    this.Controllers.Add(controller);
                }
            }
            catch (Exception ex)
            {
                this.ErrorMessage = ex.Message;
            }
            finally
            {

            }
        }

        private async Task GenerateFeatures()
        {
            DbContextOptionsBuilder<FairplaytubeDatabaseContext> optionsBuilder =
                new DbContextOptionsBuilder<FairplaytubeDatabaseContext>();
            optionsBuilder.UseSqlServer(this.ConnectionString);
            FairplaytubeDatabaseContext fairplaytubeDatabaseContext =
                new FairplaytubeDatabaseContext(optionsBuilder.Options);

            foreach (var singleController in this.Controllers)
            {
                foreach (var singleEndpoint in singleController.Endpoints)
                {
                    try
                    {
                        string featureName = $"{singleController.Name}.{singleEndpoint.Name}";
                        var existentEntity = await fairplaytubeDatabaseContext.GatedFeature
                            .SingleOrDefaultAsync(p => p.FeatureName == featureName);
                        if (existentEntity != null)
                        {
                            existentEntity.DefaultValue = singleEndpoint.DefaultValue;
                        }
                        else
                        {
                            await fairplaytubeDatabaseContext.GatedFeature.AddAsync(
                                new DataAccess.Models.GatedFeature()
                                {
                                    FeatureName = featureName,
                                    DefaultValue = singleEndpoint.DefaultValue
                                });
                        }
                        await fairplaytubeDatabaseContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        this.ErrorMessage = ex.Message;
                    }
                }
            }
        }
    }

    public class Controller
    {

        public string Name { get; set; }
        public List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
    }
    public class Endpoint
    {
        public string Name { get; set; }
        public bool DefaultValue { get; internal set; }
    }

}
