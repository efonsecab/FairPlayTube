using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
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
        [Inject]
        private IStringLocalizer<ValidateInviteCode> Localizer { get; set; }
        private string InviteCode { get; set; }
        private async Task OnValidateInviteCode()
        {
            try
            {
                var parsedInvitedCode = Guid.Parse(InviteCode);
                await this.UserClientService.ValidateInviteCodeAsync(parsedInvitedCode);
                await this.ToastifyService
                    .DisplaySuccessNotification(Localizer[ValidationSuccessTextKey],
                    duration:0);
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Validate Invite Code")]
        public const string ValidateInviteCodeTextKey = "ValidateInviteCodeText";
        [ResourceKey(defaultValue: "Please type your Invite Code")]
        public const string TypeInviteCodeTextKey = "TypeInviteCodeText";
        [ResourceKey(defaultValue: "Invite code has been validated please log out and log in again")]
        public const string ValidationSuccessTextKey = "ValidationSuccessText";
        #endregion Resource Keys
    }
}
