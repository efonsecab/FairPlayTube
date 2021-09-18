using FairPlayTube.Client.Services;
using FairPlayTube.ClientServices;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages
{
    public partial class Index
    {
        private VideoInfoModel[] AllVideos { get; set; }
        private VideoInfoModel SelectedVideo { get; set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        private bool IsLoading { get; set; }
        private bool AllowDownload { get; set; } = false;
        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationStateTask { get; set; }

        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
                //var state = await this.AuthenticationStateTask;
                //if (state.User.Identity.IsAuthenticated)
                //{
                //    AllowDownload = true;
                //}
                this.AllVideos = await this.VideoClientService.GetPublicProcessedVideosAsync();
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

        public async Task OnDownload(VideoInfoModel videoInfoModel)
        {
            try
            {
                var result = await this.VideoClientService.DownloadVideo(videoInfoModel.VideoId);
                await JSRuntime.InvokeVoidAsync(
               "downloadFromByteArray",
               new
               {
                   ByteArray = result.VideoBytes,
                   FileName = videoInfoModel.FileName,
                   ContentType = System.Net.Mime.MediaTypeNames.Application.Octet
               });
            }
            catch (Exception ex)
            {
                await this.ToastifyService.DisplayErrorNotification(ex.Message);
            }
        }
    }
}
