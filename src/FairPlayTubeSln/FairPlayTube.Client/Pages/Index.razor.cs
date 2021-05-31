using FairPlayTube.Client.ClientServices;
using FairPlayTube.Client.Services;
using FairPlayTube.Models.UserProfile;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
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
        private bool IsLoading { get; set; }
        private bool ShowInsights { get; set; }
        private bool ShowMonetizationLinks { get; set; }

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

        private void SelectVideo(VideoInfoModel videoInfoModel)
        {
            this.SelectedVideo = videoInfoModel;
            this.ShowInsights = true;
        }

        private void HideInsights()
        {
            this.ShowInsights = false;
            this.SelectedVideo = null;
        }

        private void OnMonetizationIconClicked(VideoInfoModel videoInfoModel)
        {
            this.SelectedVideo = videoInfoModel;
            this.ShowMonetizationLinks = true;
        }

        private void HideMonetizationModal()
        {
            this.SelectedVideo = null;
            this.ShowMonetizationLinks = false;
        }
    }
}
