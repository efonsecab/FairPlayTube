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
    [Route(Common.Global.Constants.UserPagesRoutes.MyVideos)]
    [Authorize(Roles = Common.Global.Constants.Roles.User)]
    public partial class MyVideos
    {
        private VideoInfoModel[] AllVideos { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IStringLocalizer<MyVideos> Localizer { get; set; }
        private bool IsLoading { get; set; }
        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllVideos = await this.VideoClientService.GetMyProcessedVideosAsync();
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

        private async Task OnVideoDelete(VideoInfoModel videoModel)
        {
            try
            {
                IsLoading = true;
                await this.VideoClientService.DeleteVideoAsync(videoModel.VideoId);
                this.AllVideos = await this.VideoClientService.GetMyProcessedVideosAsync();
                this.ToastifyService.DisplaySuccessNotification("Your video has been deleted");
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
        [ResourceKey(defaultValue:"My Videos")]
        public const string MyVideosTextKey = "MyVideosText";
        #endregion Resource Keys
    }
}
