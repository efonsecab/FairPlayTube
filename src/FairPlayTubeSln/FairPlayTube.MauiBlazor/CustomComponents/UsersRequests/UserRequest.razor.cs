using Blazored.Toast.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.UsersRequests;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace FairPlayTube.MauiBlazor.CustomComponents.UsersRequests
{
    public partial class UserRequest
    {
        [Inject]
        private IStringLocalizer<UserRequest> Localizer { get; set; }
        [Inject]
        private UserRequestClientService UserRequestClientService { get; set; }
        [Inject]
        private IToastService ToastService { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowModal { get; set; }
        private readonly CreateUserRequestModel CreateUserRequestModel = new()
        {
            UserRequestType = Common.Global.Enums.UserRequestType.ContentRequest
        };

        private void OnUserRequestIconClicked()
        {
            this.ShowModal = true;
        }

        private async Task OnValidSubmit()
        {
            try
            {
                this.IsLoading = true;
                await this.UserRequestClientService.AddAnonymousUserRequestAsync(this.CreateUserRequestModel);
                ToastService.ShowError(Localizer[UserRequestAddedTextKey]);
            }
            catch (Exception ex)
            {
                ToastService.ShowError(ex.Message);
            }
            finally
            {
                this.IsLoading = false;
                this.ShowModal = false;
            }
        }

        #region Resource Keys
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitTextKey = "SubmitText";
        [ResourceKey(defaultValue: "Request has been sent")]
        public const string UserRequestAddedTextKey = "UserRequestAddedText";
        #endregion
    }
}
