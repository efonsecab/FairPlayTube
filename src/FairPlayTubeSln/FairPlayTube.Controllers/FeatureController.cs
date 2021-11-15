using FairPlayTube.Models.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Controllers
{
    /// <summary>
    /// Handles all of the data related to Features
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        private IFeatureManager FeatureManager { get; }
        /// <summary>
        /// Initializes <see cref="FeatureController"/>
        /// </summary>
        /// <param name="featureManager"></param>
        public FeatureController(IFeatureManager featureManager)
        {
            this.FeatureManager = featureManager;
        }

        /// <summary>
        /// Retrieves the list of all registered features
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<FeatureModel[]> GetAllFeatures()
        {
            List<FeatureModel> registeredFeaturesList = new();
            await foreach (var singleFeatureName in FeatureManager.GetFeatureNamesAsync())
            {
                var isEnabled = await FeatureManager.IsEnabledAsync(singleFeatureName);
                registeredFeaturesList.Add(new FeatureModel()
                {
                    Name = singleFeatureName,
                    IsEnabled = isEnabled
                });
            }
            return registeredFeaturesList.ToArray();
        }
    }
}
