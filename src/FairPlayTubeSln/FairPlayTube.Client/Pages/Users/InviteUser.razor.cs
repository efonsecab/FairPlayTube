using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.Invites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users
{
    [Route(Common.Global.Constants.UserPagesRoutes.InviteUser)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class InviteUser
    {
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private UserClientService UserClientService { get; set; }
        private InviteUserModel InviteUserModel = new Models.Invites.InviteUserModel();
        private bool IsLoading { get; set; } = false;

        private async Task OnValidSubmit()
        {
            try
            {
                IsLoading = true;
                await this.UserClientService.InviteUserAsync(this.InviteUserModel);
                await this.ToastifyService.DisplaySuccessNotification($"Invite sent to: {this.InviteUserModel.ToEmailAddress}");
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
