using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.VideoJobApplications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Route(Common.Global.Constants.UserPagesRoutes.ReceivedApplications)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class ReceivedApplications
    {
        [Inject]
        private VideoJobApplicationClientService VideoJobApplicationClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IStringLocalizer<ReceivedApplications> Localizer { get; set; }
        private VideoJobApplicationModel[] VideoJobApplications { get; set; }
        private bool IsLoading { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.VideoJobApplications = await this.VideoJobApplicationClientService
                    .GetNewReceivedVideoJobApplicationsAsync();
            }
            catch (Exception ex)
            {
                ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Received Applications")]
        public const string ReceivedApplicationsTextKey = "ReceivedApplicationsText";
        #endregion Resource Keys
    }
}
