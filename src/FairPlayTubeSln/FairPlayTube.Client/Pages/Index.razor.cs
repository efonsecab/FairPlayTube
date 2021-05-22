using FairPlayTube.Client.ClientServices;
using FairPlayTube.Client.Services;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace FairPlayTube.Client.Pages
{
    public partial class Index
    {
        public VideoInfoModel[] AllVideos { get; private set; }
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private ToastifyService ToastifyService { get; set; }
        private bool IsLoading { get; set; }
        protected async override Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;
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
    }
}
