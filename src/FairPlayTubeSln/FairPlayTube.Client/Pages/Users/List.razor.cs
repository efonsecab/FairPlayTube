using FairPlayTube.Client.Navigation;
using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Global;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.UserMessage;
using FairPlayTube.Models.UserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
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
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        [Inject]
        private IStringLocalizer<List> Localizer { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowMessageSenderModal { get; set; }
        private UserModel SelectedUser { get; set; }

        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllUsers = await this.UserClientService.ListUsersAsync();
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
                this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
            finally
            {
                this.ShowMessageSenderModal = false;
                this.SelectedUser = null;
                this.IsLoading = false;
            }
        }

        private static string GetUserHomePageLink(UserModel userModel)
        {
            return Common.Global.Constants.UserPagesRoutes.UserHomePage
                .Replace("{UserId:long}", userModel.ApplicationUserId.ToString());
        }

        private void NavigateToUserYouTubeVideos(long applicationUserId)
        {
            NavigationHelper.NavigateToUserYouTubeVideosPage(this.NavigationManager, applicationUserId);
        }

        #region Resource Keys
        [ResourceKey(defaultValue:"Users")]
        public const string UsersTextKey = "UsersText";
        [ResourceKey(defaultValue: "Videos")]
        public const string VideosTextKey = "VideosText";
        [ResourceKey(defaultValue: "Brands")]
        public const string BrandsTextKey = "BrandsText";
        [ResourceKey(defaultValue: "YouTube Channels")]
        public const string YouTubeChannelsTextKey = "YouTubeChannelsText";
        [ResourceKey(defaultValue:"Send Message")]
        public const string SendMessageTextKey = "SendMessageText";

        #endregion Resource Keys
    }
}
