using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Common.Localization;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages.Users.Videos
{
    [Route(Common.Global.Constants.UserPagesRoutes.AddVideoJob)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class AddVideoJob
    {

        [Parameter]
        public string VideoId { get; set; }
        [Inject]
        public IStringLocalizer<AddVideoJob> Localizer { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private VideoJobClientService VideoJobClientService { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private bool IsLoading { get; set; }
        private VideoJobModel VideoJobModel { get; set; } = new();
        private VideoInfoModel VideoModel { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (!FeatureClientService.IsFeatureEnabled(Common.Global.Enums.FeatureType.VideoJobSystem))
                {
                    NavigationManager.NavigateTo("/");
                    return;
                }
                this.VideoJobModel.VideoId = VideoId;
                IsLoading = true;
                this.VideoModel = await VideoClientService.GetVideoAsync(VideoId);
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

        private async Task OnValidSubmit()
        {
            try
            {
                IsLoading = true;
                await VideoJobClientService.AddVideoJobAsync(this.VideoJobModel);
                ToastifyService.DisplaySuccessNotification(Localizer[VideoJobCreatedTextKey]);
                NavigationManager.NavigateTo(Common.Global.Constants.UserPagesRoutes.MyVideos);
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

        #region Resource Keys
        [ResourceKey(defaultValue: "Video job has been created")]
        public const string VideoJobCreatedTextKey = "VideoJobCreatedText";
        [ResourceKey(defaultValue: "Video Name")]
        public const string VideoNameTextKey = "VideoNameText";
        [ResourceKey(defaultValue: "Add Video Job")]
        public const string AddVideoJobTitleTextKey = "AddVideoJobTitleText";
        [ResourceKey(defaultValue: "Submit")]
        public const string SubmitTextKey = "SubmitText";
        #endregion Resource Keys
    }
}
