using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
        private bool IsLoading { get; set; } = false;
        private decimal AvailableFunds { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await LoadAvailableFundsData();
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
                this.AvailableFunds = await this.UserProfileClientService.GetMyFunds();
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
