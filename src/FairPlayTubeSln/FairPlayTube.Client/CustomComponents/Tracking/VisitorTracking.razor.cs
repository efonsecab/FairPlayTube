using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Extensions;
using FairPlayTube.Models.VisitorTracking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.CustomComponents.Tracking
{
    public partial class VisitorTracking
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private VisitorTrackingClientService VisitorTrackingClientService { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                VisitorTrackingModel visitorTrackingModel = new VisitorTrackingModel()
                {
                    VisitedUrl = this.NavigationManager.Uri
                };
                var state = await AuthenticationStateTask;
                if (state != null && state.User != null && state.User.Identity.IsAuthenticated)
                {
                    var userObjectId = state.User.Claims.GetAzureAdB2CUserObjectId();
                    visitorTrackingModel.UserAzureAdB2cObjectId = userObjectId;
                }
                await this.VisitorTrackingClientService.TrackVisit(visitorTrackingModel);
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }
    }
}
