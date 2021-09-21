using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users
{
    [Route(Common.Global.Constants.UserPagesRoutes.ValidateInviteCode)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]

    public partial class ValidateInviteCode
    {
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private UserClientService UserClientService { get; set; }
        private string InviteCode { get; set; }
        private async Task OnValidateInviteCode()
        {
            try
            {
                var parsedInvitedCode = Guid.Parse(InviteCode);
                await this.UserClientService.ValidateInviteCodeAsync(parsedInvitedCode);
                await this.ToastifyService
                    .DisplaySuccessNotification("Invite code has been validated please log out and log in again",
                    duration:0);
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }
    }
}
