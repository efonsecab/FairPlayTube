using FairPlayTube.Client.ClientServices;
using FairPlayTube.Client.Services;
using FairPlayTube.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
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
