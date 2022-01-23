using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.UserMessage;
using FairPlayTube.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Conversations
{
    [Route(Common.Global.Constants.UserPagesRoutes.MyConversations)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class MyConversations
    {
        private ConversationsUserModel[] AllMyConversationsUsers;

        [Inject]
        private UserMessageClientService UserMessageClientService { get; set; }
        [Inject]
        private IStringLocalizer<MyConversations> Localizer { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private UserClientService UserClientService { get; set; }
        private bool IsLoading { get; set; } = false;
        public ConversationsUserModel SelectedUser { get; private set; }
        private UserMessageModel[] AllMyConversationsWithSelectedUser { get; set; }
        private UserMessageModel MessageToSend { get; set; } = new UserMessageModel();

        protected override async Task OnInitializedAsync()
        {

            try
            {
                IsLoading = true;
                this.AllMyConversationsUsers =
                    await this.UserMessageClientService.GetMyConversationsUsersAsync();
                if (this.AllMyConversationsUsers.Length > 0)
                {
                    this.SelectedUser = this.AllMyConversationsUsers[0];
                    this.AllMyConversationsWithSelectedUser = await
                        this.UserMessageClientService
                        .GetMyConversationsWithUserAsync(this.SelectedUser.ApplicationUserId);
                    this.MessageToSend.ToApplicationUserId = this.SelectedUser.ApplicationUserId;
                }
            }
            catch (Exception ex)
            {
                ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SendMessageAsync()
        {
            try
            {
                IsLoading = true;
                await this.UserClientService.SendMessageAsync(this.MessageToSend);
                this.AllMyConversationsWithSelectedUser = await
                                        this.UserMessageClientService
                                        .GetMyConversationsWithUserAsync(this.SelectedUser.ApplicationUserId);
            }
            catch (Exception ex)
            {
                ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #region Resources Keys
        [ResourceKey(defaultValue:"My Conversations")]
        public const string MyConversationsTitleKey = "MyConversationsTitle";
        #endregion
    }
}
