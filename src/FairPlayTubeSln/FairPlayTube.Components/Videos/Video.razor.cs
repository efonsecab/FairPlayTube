﻿using FairPlayTube.ClientServices;
using FairPlayTube.Common.Interfaces;
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
        [Parameter]
        public bool ShowTwitterShareButton { get; set; } = false;
        [Inject]
        private IVideoEditAccessTokenProvider VideoEditAccessTokenProvider { get; set; }
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
                this.EditAccessToken = await this.VideoEditAccessTokenProvider.GetVideoEditAccessToken(VideoModel.VideoId);
                this.VideoModel.EditAccessToken = this.EditAccessToken;
            }
            this.ShowInsights = true;
        }

        private string VideoInsightsUrl
        {
            get
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
