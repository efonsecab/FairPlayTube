using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Invites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
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
        [Inject]
        private IStringLocalizer<InviteUser> Localizer { get; set; }
        private readonly InviteUserModel InviteUserModel = new();
        private bool IsLoading { get; set; } = false;

        private async Task OnValidSubmit()
        {
            try
            {
                IsLoading = true;
                await this.UserClientService.InviteUserAsync(this.InviteUserModel);
                this.ToastifyService.DisplaySuccessNotification($"Invite sent to: {this.InviteUserModel.ToEmailAddress}");
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
        [ResourceKey(defaultValue:"Invite User")]
        public const string InviteUserTextKey = "InviteUserText";
        [ResourceKey(defaultValue:"Email Address")]
        public const string EmailAddressTextKey = "EmailAddressText";
        #endregion Resource Keys
    }
}
