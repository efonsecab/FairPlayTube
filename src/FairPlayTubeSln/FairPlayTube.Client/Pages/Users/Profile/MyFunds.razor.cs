using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Profile
{
    [Route(Common.Global.Constants.UserPagesRoutes.MyFunds)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class MyFunds
    {
        [Inject]
        private UserProfileClientService UserProfileClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IStringLocalizer<MyFunds> Localizer { get; set; }
        private bool IsLoading { get; set; } = false;
        private decimal AvailableFunds { get; set; }

        protected async override Task OnInitializedAsync()
        {
            IsLoading = true;
            await LoadAvailableFundsData();
            IsLoading = false;
        }

        public async Task OnFundsAdded()
        {
            await LoadAvailableFundsData();
        }

        private async Task LoadAvailableFundsData()
        {
            try
            {
                IsLoading = true;
                this.AvailableFunds = await this.UserProfileClientService.GetMyFundsAsync();
            }
            catch (Exception ex)
            {
                this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Available Funds")]
        public const string AvailableFundsTextKey = "AvailableFundsText";
        #endregion Resource Keys
    }
}
