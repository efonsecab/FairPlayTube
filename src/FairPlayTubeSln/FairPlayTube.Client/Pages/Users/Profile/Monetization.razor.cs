using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Profile
{
    [Route(Common.Global.Constants.UserPagesRoutes.ProfileMonetization)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class Monetization
    {
        [Inject]
        private UserProfileClientService UserProfileClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private bool IsLoading { get; set; }
        private GlobalMonetizationModel GlobalMonetizationModel =
            new GlobalMonetizationModel()
            {
                MonetizationItems = new List<MonetizationItem>()
                {
                    new MonetizationItem()
                }
            };

        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.GlobalMonetizationModel = await this.UserProfileClientService.GetMyMonetizationInfo();
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

        private void RemoveItem(MonetizationItem selectedItem)
        {
            this.GlobalMonetizationModel.MonetizationItems.Remove(selectedItem);
        }

        private void AddItem()
        {
            this.GlobalMonetizationModel.MonetizationItems.Add(new MonetizationItem());
        }

        private async Task OnValidSubmit()
        {
            try
            {
                IsLoading = true;
                await this.UserProfileClientService.SaveMonetizationAsync(this.GlobalMonetizationModel);
                await this.ToastifyService.DisplaySuccessNotification("Monetization has been saved");
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

