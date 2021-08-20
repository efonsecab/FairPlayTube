using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
        private bool IsLoading { get; set; }
        private VideoInfoModel SelectedVideo { get; set; }
        private bool ShowModal { get; set; } = false;
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                this.AllVideos = await this.VideoClientService.GetMyProcessedVideos();
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

        private async Task OnVideoDelete(VideoInfoModel videoModel)
        {
            try
            {
                IsLoading = true;
                await this.VideoClientService.DeleteVideoAsync(videoModel.VideoId);
                this.AllVideos = await this.VideoClientService.GetMyProcessedVideos();
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
