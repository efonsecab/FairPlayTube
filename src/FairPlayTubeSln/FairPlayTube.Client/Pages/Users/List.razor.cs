using FairPlayTube.Client.ClientServices;
using FairPlayTube.Client.Services;
using FairPlayTube.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users
{
    [Route(Common.Global.Constants.UserPagesRoutes.List)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class List
    {
        public UserModel[] AllUsers { get; private set; }
        [Inject]
        private UserProfileClientService UserProfileClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private bool IsLoading { get; set; }

        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllUsers = await this.UserProfileClientService.ListUsers();
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
