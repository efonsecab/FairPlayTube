using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Extensions;
using FairPlayTube.Common.Global;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.VisitorTracking;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace FairPlayTube.Client.Shared
{
    public partial class MainLayout
    {
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [CascadingParameter]
        private Error Error { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private ClientServices.LocalizationClientService LocalizationClientService { get; set; }
        [Inject]
        private IStringLocalizer<MainLayout> Localizer { get; set; }
        [Inject]
        private VisitorTrackingClientService VisitorTrackingClientService { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowFooter { get; set; } = true;
        private Timer VisitsTimer { get; set; }
        private bool ShowCultureSelector { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                await LocalizationClientService.LoadDataAsync();
                await TrackVisit(createNewSession: true);
                this.NavigationManager.LocationChanged += NavigationManager_LocationChanged;
            }
            catch (Exception ex)
            {
                await Error.ProcessErrorAsync(ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task TrackVisit(bool createNewSession)
        {
            //We do not want to track authentication flow pages visits
            if (NavigationManager.Uri.Contains("/authentication/"))
                return;
            VisitorTrackingModel visitorTrackingModel = new()
            {
                VisitedUrl = this.NavigationManager.Uri
            };
            var state = await AuthenticationStateTask;
            if (state != null && state.User != null && state.User.Identity.IsAuthenticated)
            {
                var userObjectId = state.User.Claims.GetAzureAdB2CUserObjectId();
                visitorTrackingModel.UserAzureAdB2cObjectId = userObjectId;
                await this.VisitorTrackingClientService.TrackAuthenticatedVisitAsync(visitorTrackingModel,
                    createNewSession);
            }
            else
            {
                await this.VisitorTrackingClientService.TrackAnonymousVisitAsync(visitorTrackingModel,
                    createNewSession);
            }

            if (createNewSession)
            {
                this.VisitsTimer = new Timer(TimeSpan.FromSeconds(60).TotalMilliseconds);
                this.VisitsTimer.Elapsed += VisitsTimer_Elapsed;
                this.VisitsTimer.Start();
            }
        }

        private async void VisitsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                await VisitorTrackingClientService.UpdateVisitTimeElapsedAsync();
            }
            catch (Exception ex)
            {
                await Error.ProcessErrorAsync(ex);
            }
        }

        private async void NavigationManager_LocationChanged(object sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            try
            {
                await TrackVisit(createNewSession: false);
            }
            catch (Exception ex)
            {
                await Error.ProcessErrorAsync(ex);
            }
        }

        private void CloseFooter()
        {
            this.ShowFooter = false;
        }

        public void OnSearchClicked(string searchTerm)
        {
            this.NavigationManager.NavigateTo($"{Constants.RootPagesRoutes.SearchWithSearchTerm.Replace("{SearchTerm}", searchTerm)}");
        }

        private void OnShowCultureSelectorClicked()
        {
            this.ShowCultureSelector = true;
        }

        private void HideCultureSelector()
        {
            this.ShowCultureSelector = false;
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Note: Some browser extensions block you from playing videos. " +
            "If all you see is a blank screen, you will need to disable them")]
        public const string BrowsersExensionsWarningTextKey = "BrowsersExensionsWarningText";
        [ResourceKey(defaultValue: "Please fill")]
        public const string PleaseFillTextKey = "PleaseFillText";
        [ResourceKey(defaultValue: "This survey")]
        public const string ThisSurveyTextKey = "ThisSurveyText";
        [ResourceKey(defaultValue: "Designed By")]
        public const string DesignedByTextKey = "DesignedByText";
        [ResourceKey(defaultValue: "Request Demo")]
        public const string RequestDemoTextKey = "RequestDemoText";
        #endregion Resource Keys
    }
}
