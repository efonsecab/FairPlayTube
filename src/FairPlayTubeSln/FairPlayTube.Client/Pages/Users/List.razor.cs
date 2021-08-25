using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.UserMessage;
using FairPlayTube.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users
{
    [Route(Common.Global.Constants.UserPagesRoutes.List)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class List
    {
        public UserModel[] AllUsers { get; private set; }
        [Inject]
        private UserClientService UserClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowMessageSenderModal { get; set; }
        private UserModel SelectedUser { get; set; }

        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllUsers = await this.UserClientService.ListUsers();
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

        private void OnOpenMessageSenderModal(UserModel user)
        {
            this.SelectedUser = user;
            this.ShowMessageSenderModal = true;
        }

        private void OnSendMessageCancelled()
        {
            this.ShowMessageSenderModal = false;
            this.SelectedUser = null;
        }

        private async Task OnSendMessage(UserMessageModel messageModel)
        {
            try
            {
                this.ShowMessageSenderModal = false;
                this.IsLoading = true;
                await this.UserClientService.SendMessageAsync(messageModel);
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                this.ShowMessageSenderModal = false;
                this.SelectedUser = null;
                this.IsLoading = false;
            }
        }
    }
}
