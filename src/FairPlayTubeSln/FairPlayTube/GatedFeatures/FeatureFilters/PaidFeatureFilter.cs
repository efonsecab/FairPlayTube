using FairPlayTube.Common.Extensions;
using FairPlayTube.Common.Interfaces;
using FairPlayTube.DataAccess.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.GatedFeatures.FeatureFilters
{
    /// <summary>
    /// 
    /// </summary>
    [FilterAlias("PaidFilter")]
    public class PaidFeatureFilter : IFeatureFilter
    {
        private IActionContextAccessor ActionContextAccessor { get; }
        private ICurrentUserProvider CurrentUserProvider { get; }
        private FairplaytubeDatabaseContext FairplaytubeDatabase { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionContextAccessor"></param>
        /// <param name="currentUserProvider"></param>
        /// <param name="fairplaytubeDatabase"></param>
        public PaidFeatureFilter(IActionContextAccessor actionContextAccessor, 
            ICurrentUserProvider currentUserProvider, 
            FairplaytubeDatabaseContext fairplaytubeDatabase)
        {
            this.ActionContextAccessor = actionContextAccessor;
            this.CurrentUserProvider = currentUserProvider;
            this.FairplaytubeDatabase = fairplaytubeDatabase;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var rd = this.ActionContextAccessor.ActionContext.RouteData;
            string currentController = rd.Values["controller"].ToString() + "Controller";
            string currentAction = rd.Values["action"].ToString();
            string featureName = $"{currentController}.{currentAction}";
            bool shouldGrantAccess= await ShouldGrantAccess(featureName:featureName);
            return shouldGrantAccess;
        }

        private async Task<bool> ShouldGrantAccess(string featureName)
        {
            bool shouldGrantAccess = false;
            var userAzureAdB2cObjctId = this.CurrentUserProvider.GetObjectId();
            bool? userConfiguredPermission = null;
            //TODO: Get userConfiguredPermission value from table with user assigned permissions
            var applicationUserFeatureEntity = await this.FairplaytubeDatabase
                .ApplicationUserFeature.Include(p=>p.GatedFeature)
                .Include(p=>p.ApplicationUser)
                .SingleOrDefaultAsync(p=>p.GatedFeature.FeatureName == featureName && 
                p.ApplicationUser.AzureAdB2cobjectId.ToString() == userAzureAdB2cObjctId);
            if (applicationUserFeatureEntity != null)
                userConfiguredPermission = applicationUserFeatureEntity.Enabled;
            if (!userConfiguredPermission.HasValue)
            {
                var gatedFeatureEntity = await this.FairplaytubeDatabase
                    .GatedFeature.SingleOrDefaultAsync(p => p.FeatureName == featureName);
                if (gatedFeatureEntity != null)
                {
                    shouldGrantAccess = gatedFeatureEntity.DefaultValue.Value;
                }
                //TODO:Evalute if we should CheckFunds here to centralize logic;
            }
            else
            {
                shouldGrantAccess = userConfiguredPermission.Value;
            }
            return shouldGrantAccess;
        }
    }
}
