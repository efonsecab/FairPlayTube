using FairPlayTube.ClientServices;
using FairPlayTube.Models.Video;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayTube.Components.Videos
{
    public partial class Video
    {
        [Parameter]
        public VideoInfoModel VideoModel { get; set; }
        [Parameter]
        public bool AllowEdit { get; set; } = false;
        [Parameter]
        public bool ShowDetailsLink { get; set; } = false;
        [Inject]
        private VideoClientService VideoClientService { get; set; }
        [Inject]
        private NavigationManager NavigationManager { get; set; }
        private bool IsLoading { get; set; }
        private bool ShowInsights { get; set; }
        private bool ShowMonetizationLinks { get; set; }
        private bool ShowVideoDescription { get; set; }
        private string EditAccessToken { get; set; }

        private async Task SelectVideo()
        {
            if (AllowEdit)
            {
                this.EditAccessToken = await this.VideoClientService.GetVideoEditAccessToken(VideoModel.VideoId);
            }
            this.ShowInsights = true;
        }

        private void HideInsights()
        {
            this.ShowInsights = false;

        }

        private void OnMonetizationIconClicked()
        {
            this.ShowMonetizationLinks = true;
        }

        private void OnVideoDescriptionClicked()
        {
            this.ShowVideoDescription = true;
        }

        private void HideMonetizationModal()
        {
            this.ShowMonetizationLinks = false;
        }
        private void HideVideoDescriptionModal()
        {
            this.ShowVideoDescription = false;
        }

        private string GetVideoInsightsUrl()
        {
            if (this.AllowEdit)
            {
                return VideoModel.PrivateInsightsUrl;
            }
            else
            {
                return VideoModel.PublicInsightsUrl;
            }
        }

        private void ShowVideoPlayer()
        {
            VideoModel.ShowPlayerWidget = true;
            StateHasChanged();
        }

        private void ViewDetails()
        {
            string formattedUrl = Common.Global.Constants.PublicVideosPages.Details
                .Replace("{VideoId}", VideoModel.VideoId);
            this.NavigationManager.NavigateTo(formattedUrl);
        }

    }
}
